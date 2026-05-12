using TrackerPro.Tasks.Vision.HandLandmarker;
using TrackerPro.Unity;
using TrackerPro.Unity.CoordinateSystem;
using UnityEngine;

namespace TrackerPro.MetaHuman
{
    public class HandLandmarkData : TrackingEventHandler
    {
        public RectTransform screen;
        public override void OnPoseUpdate(ITrackingResult solution)
        {
            Debug.Log("HandLandmarkData received pose update.");
            if (solution is HandLandmarkerResult handLandmarkerResult)
            {
                var handLandmarks = handLandmarkerResult.handLandmarks;
                if (handLandmarks != null)
                {
                    Debug.Log($"Hand Landmarks Count: {handLandmarks.Count}");
                    for (int i = 0; i < handLandmarks.Count; i++)
                    {
                        for (int j = 0; j < handLandmarks[i].landmarks.Count; j++)
                        {
                            Debug.Log(handLandmarks[i].landmarks[j]);
                            Debug.Log(GetScreenPoint(handLandmarks[i].landmarks[j]));
                        }
                    }
                }
                else
                {
                    Debug.Log("No Hand Landmarks detected.");
                }
            }
            else
            {
                Debug.Log("Tracking result is not a HandLandmarkerResult.");
            }
        }
        public Vector3 GetScreenPoint(TrackerPro.Tasks.Components.Containers.NormalizedLandmark landmark)
        {
            var relative_position = screen.rect.GetPoint(landmark, 0, false);
            var local_position = screen.TransformPoint(relative_position);
            return local_position;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created

    }
}
