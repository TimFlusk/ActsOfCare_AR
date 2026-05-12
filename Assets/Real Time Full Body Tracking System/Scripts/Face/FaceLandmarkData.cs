using TrackerPro.Tasks.Vision.FaceLandmarker;
using TrackerPro.Unity.CoordinateSystem;
using UnityEngine;
namespace TrackerPro.Unity.Sample.FaceLandmarkDetection
{
    public class FaceLandmarkData : TrackingEventHandler
    {
        public RectTransform screen;
        public override void OnPoseUpdate(ITrackingResult solution)
        {
            Debug.Log("FaceLandmarkData received pose update.");
            if (solution is FaceLandmarkerResult faceLandmarkResult)
            {
                var faceLandmarks = faceLandmarkResult.faceLandmarks;
                if (faceLandmarks != null)
                {
                    Debug.Log($"Face Landmarks Count: {faceLandmarks.Count}");
                    // for (int i = 0; i < faceLandmarks.Count; i++)
                    // {
                    //     for(int j=0;j< faceLandmarks[i].landmarks.Count;j++)
                    //     {
                    //         Debug.Log(faceLandmarks[i].landmarks[j]);
                    //         Debug.Log(GetScreenPoint(faceLandmarks[i].landmarks[j]));
                    //     }
                    // }
                }
                else
                {
                    Debug.Log("No Face Landmarks detected.");
                }
            }
            else
            {
                Debug.Log("Tracking result is not a FaceLandmarkTrackingResult.");
            }
        }
        public Vector3 GetScreenPoint(TrackerPro.Tasks.Components.Containers.NormalizedLandmark landmark)
        {
            var relative_position = screen.rect.GetPoint(landmark, 0, false);
            var local_position = screen.TransformPoint(relative_position);
            return local_position;
        }
    }
}
