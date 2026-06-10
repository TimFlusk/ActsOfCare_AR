using TrackerPro.Tasks.Vision.FaceLandmarker;
using TrackerPro.Unity;
using TrackerPro.Unity.CoordinateSystem;
using UnityEngine;
namespace TrackerPro
{
    [RequireComponent(typeof(Animator))]
    public class FaceTracking : TrackingEventHandler
    {
        public bool isTrackingEnable = true;
        public float smoothness = .5f;
        private Animator avatar;
        private SkinnedMeshRenderer faceRenderer;

        public RectTransform screen;
        private Transform headBone;
        private Transform neckBone;
        private Transform chestBone;

        //public int _emotion = 0;
        //private Renderer rend;

    
        void Start()
        {

            avatar = GetComponent<Animator>();
            headBone = avatar.GetBoneTransform(HumanBodyBones.Head);
            neckBone = avatar.GetBoneTransform(HumanBodyBones.Neck);
            chestBone = avatar.GetBoneTransform(HumanBodyBones.Chest);
            faceRenderer = avatar.GetComponentInChildren<SkinnedMeshRenderer>();
           // rend = avatar.GetComponentInChildren<Renderer>();
        
        }
        public override void OnPoseUpdate(ITrackingResult solution)
        {
            if (solution is FaceLandmarkerResult faceLandmarkResult)
            {
                if (!isTrackingEnable)
                {
                    return;
                }
                var faceBlendshapes = faceLandmarkResult.faceBlendshapes;
                
                if (faceBlendshapes != null && faceRenderer != null)
                {
                    for(int i=0;i< faceBlendshapes.Count;i++)
                    {
                        for(int j=0;j< faceBlendshapes[i].categories.Count;j++)
                        {
                            string blendShapeName = faceBlendshapes[i].categories[j].categoryName;
                            float weight = faceBlendshapes[i].categories[j].score * 100f; // Unity uses 0–100

                            int index = faceRenderer.sharedMesh.GetBlendShapeIndex(blendShapeName);


                            if (index >= 0)
                            {
                                faceRenderer.SetBlendShapeWeight(index, weight);

                                // Compare one or two blend shapes for emotion texture switching
                              
                            }
                          
                        }
                        var rotation = faceLandmarkResult.facialTransformationMatrixes[i].rotation;
                        rotation.x= -rotation.x;
                        rotation.y= -rotation.y;
                        headBone.rotation = Quaternion.Slerp(headBone.rotation, rotation, smoothness);
                        neckBone.rotation = Quaternion.Slerp(neckBone.rotation, rotation, smoothness);
                        chestBone.rotation = Quaternion.Slerp(chestBone.rotation, rotation, smoothness);

                        

                        
                    }

                    
                }
                else
                {
                    // Debug.Log("No Face Landmarks detected.");
                }
            }
            else
            {
                // Debug.Log("Tracking result is not a FaceLandmarkTrackingResult.");
            }

             //rend.material.SetInt("_Emotion", _emotion); //Set emotion value to shader
        }
        public Vector3 GetScreenPoint(TrackerPro.Tasks.Components.Containers.NormalizedLandmark landmark)
        {
            var relative_position = screen.rect.GetPoint(landmark, 0, false);
            var local_position = screen.TransformPoint(relative_position);
            return local_position;
        }
    }
}

