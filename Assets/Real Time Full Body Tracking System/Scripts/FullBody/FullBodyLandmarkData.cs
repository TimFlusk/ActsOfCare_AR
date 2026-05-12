using TrackerPro;
using TrackerPro.Unity;
using TrackerPro.Unity.CoordinateSystem;
using TrackerPro.Unity.Sample.Holistic;
using UnityEngine;
namespace TrackerPro.MetaHuman
{
    public class FullBodyLandmarkData : TrackingEventHandler
    {
        public RectTransform screen;
        public override void OnPoseUpdate(ITrackingResult solution)
        {
            Debug.Log("FullBodyLandmarkData received pose update.");
            Debug.Log("FullBodyLandmarkData received pose update.");
            if (solution is HolisticTrackingResult holisticResult)
            {
                var poseLandmarks = holisticResult.poseLandmarks;
                if (poseLandmarks != null)
                {
                    Debug.Log($"Pose Landmarks Count: {poseLandmarks.Landmark.Count}");
                    // for (int i = 0; i <  poseLandmarks.Landmark.Count; i++)
                    // {
                    //     Debug.Log(GetScreenPoint(poseLandmarks.Landmark[i]));
                    // }
                }
                else
                {
                    Debug.Log("No Pose Landmarks detected.");
                }
                //print other landmark data as needed
                if (holisticResult.faceLandmarks != null)
                {
                    Debug.Log($"Face Landmarks Count: {holisticResult.faceLandmarks.Landmark.Count}");
                }
                else
                {
                    Debug.Log("No Face Landmarks detected.");
                }
            }
            else
            {
                Debug.Log("Tracking result is not a HolisticTrackingResult.");
            }

        }
        public Vector3 GetScreenPoint(NormalizedLandmark landmark)
        {
            var relative_position = screen.rect.GetPoint(landmark, 0, false);
            var local_position = screen.TransformPoint(relative_position);
            return local_position;
        }
    }
}
