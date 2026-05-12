using System.Collections;
using TrackerPro;
using TrackerPro.Unity;
using TrackerPro.Unity.Sample;
using TrackerPro.Unity.Sample.Holistic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TrackerPro.MetaHuman
{
    public class CoreHolisticSolution : LegacySolutionRunner<HolisticTrackingGraph>
    {

        private TrackerPro.Unity.Experimental.TextureFramePool _textureFramePool;
        private Detection poseDetection;
        private NormalizedLandmarkList poseLandmarks;
        private NormalizedLandmarkList faceLandmarks;
        private NormalizedLandmarkList leftHandLandmarks;
        private NormalizedLandmarkList rightHandLandmarks;
        private LandmarkList poseWorldLandmarks;
        private ImageFrame segmentationMask;
        private NormalizedRect poseRoi; 
        public HolisticTrackingGraph.ModelComplexity modelComplexity
        {
            get => graphRunner.modelComplexity;
            set => graphRunner.modelComplexity = value;
        }

        public bool smoothLandmarks
        {
            get => graphRunner.smoothLandmarks;
            set => graphRunner.smoothLandmarks = value;
        }

        public bool refineFaceLandmarks
        {
            get => graphRunner.refineFaceLandmarks;
            set => graphRunner.refineFaceLandmarks = value;
        }

        public bool enableSegmentation
        {
            get => graphRunner.enableSegmentation;
            set => graphRunner.enableSegmentation = value;
        }

        public bool smoothSegmentation
        {
            get => graphRunner.smoothSegmentation;
            set => graphRunner.smoothSegmentation = value;
        }

        public float minDetectionConfidence
        {
            get => graphRunner.minDetectionConfidence;
            set => graphRunner.minDetectionConfidence = value;
        }

        public float minTrackingConfidence
        {
            get => graphRunner.minTrackingConfidence;
            set => graphRunner.minTrackingConfidence = value;
        }

        public override void Stop()
        {
            base.Stop();
            _textureFramePool?.Dispose();
            _textureFramePool = null;
        }

        protected override IEnumerator Run()
        {
            var graphInitRequest = graphRunner.WaitForInit(runningMode);
            var imageSource = ImageSourceProvider.ImageSource;

            yield return imageSource.Play();

            if (!imageSource.isPrepared)
            {
                Debug.LogError("Failed to start ImageSource, exiting...");
                yield break;
            }

            // Use RGBA32 as the input format.
            // TODO: When using GpuBuffer, MediaPipe assumes that the input format is BGRA, so the following code must be fixed.
            _textureFramePool = new TrackerPro.Unity.Experimental.TextureFramePool(imageSource.textureWidth, imageSource.textureHeight, TextureFormat.RGBA32, 10);

            // NOTE: The screen will be resized later, keeping the aspect ratio.
            screen.Initialize(imageSource);

            yield return graphInitRequest;
            if (graphInitRequest.isError)
            {
                Debug.LogError(graphInitRequest.error);
                yield break;
            }

            if (!runningMode.IsSynchronous())
            {
                graphRunner.OnPoseDetectionOutput += OnPoseDetectionOutput;
                graphRunner.OnFaceLandmarksOutput += OnFaceLandmarksOutput;
                graphRunner.OnPoseLandmarksOutput += OnPoseLandmarksOutput;
                graphRunner.OnLeftHandLandmarksOutput += OnLeftHandLandmarksOutput;
                graphRunner.OnRightHandLandmarksOutput += OnRightHandLandmarksOutput;
                graphRunner.OnPoseWorldLandmarksOutput += OnPoseWorldLandmarksOutput;
                graphRunner.OnSegmentationMaskOutput += OnSegmentationMaskOutput;
                graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;
                graphRunner.OnPoseRoiOutput += OnPoseRoiOutput;

            }
            graphRunner.StartRun(imageSource);

            AsyncGPUReadbackRequest req = default;
            var waitUntilReqDone = new WaitUntil(() => req.done);

            // NOTE: we can share the GL context of the render thread with MediaPipe (for now, only on Android)
            var canUseGpuImage = graphRunner.configType == GraphRunner.ConfigType.OpenGLES && GpuManager.GpuResources != null;
            using var glContext = canUseGpuImage ? GpuManager.GetGlContext() : null;

            while (true)
            {
                if (isPaused)
                {
                    yield return new WaitWhile(() => isPaused);
                }

                if (!_textureFramePool.TryGetTextureFrame(out var textureFrame))
                {
                    yield return new WaitForEndOfFrame();
                    continue;
                }

                // Copy current image to TextureFrame
                if (canUseGpuImage)
                {
                    yield return new WaitForEndOfFrame();
                    textureFrame.ReadTextureOnGPU(imageSource.GetCurrentTexture());
                }
                else
                {
                    req = textureFrame.ReadTextureAsync(imageSource.GetCurrentTexture(), false, imageSource.isVerticallyFlipped);
                    yield return waitUntilReqDone;

                    if (req.hasError)
                    {
                        Debug.LogWarning($"Failed to read texture from the image source");
                        yield return new WaitForEndOfFrame();
                        continue;
                    }
                }

                graphRunner.AddTextureFrameToInputStream(textureFrame, glContext);
                if (runningMode.IsSynchronous())
                {
                    screen.ReadSync(textureFrame);

                    var task = graphRunner.WaitNextAsync();
                    yield return new WaitUntil(() => task.IsCompleted);

                    var result = task.Result;
                    
                    foreach (var trackingEventHandler in trackingEventHandlers)
                    {
                        trackingEventHandler.OnPoseUpdateEvent.Invoke(result);
                    }

                    result.segmentationMask?.Dispose();
                }
                else
                {
                    var result=new HolisticTrackingResult(poseDetection, poseLandmarks, faceLandmarks, leftHandLandmarks, rightHandLandmarks, poseWorldLandmarks, segmentationMask, poseRoi);
                    foreach (var trackingEventHandler in trackingEventHandlers)
                    {
                        trackingEventHandler.OnPoseUpdateEvent.Invoke(result);
                    }

                    Debug.Log("Running in asynchronous mode, skipping WaitNextAsync. Need running mode sync to get OnPoseUpdateEvent.");
                }
            }
        }

        private void OnPoseDetectionOutput(object stream, OutputStream<Detection>.OutputEventArgs eventArgs)
        {
            Debug.Log("Added texture frame to input stream");
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(Detection.Parser);
            poseDetection = value;
        }

        private void OnFaceLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);
            faceLandmarks = value;
        }

        private void OnPoseLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);
            poseLandmarks = value;
        }

        private void OnLeftHandLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);
            leftHandLandmarks = value;
        }

        private void OnRightHandLandmarksOutput(object stream, OutputStream<NormalizedLandmarkList>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(NormalizedLandmarkList.Parser);
            rightHandLandmarks = value;
        }

        private void OnPoseWorldLandmarksOutput(object stream, OutputStream<LandmarkList>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(LandmarkList.Parser);
            poseWorldLandmarks = value;
        }

        private void OnSegmentationMaskOutput(object stream, OutputStream<ImageFrame>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get();
            value?.Dispose();
        }

        private void OnPoseRoiOutput(object stream, OutputStream<NormalizedRect>.OutputEventArgs eventArgs)
        {
            var packet = eventArgs.packet;
            var value = packet == null ? default : packet.Get(NormalizedRect.Parser);
            poseRoi = value;
        }
    }
}
