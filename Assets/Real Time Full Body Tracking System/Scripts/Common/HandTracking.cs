using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using TrackerPro.MetaHuman;
using TrackerPro.Tasks.Vision.FaceLandmarker;
using TrackerPro.Tasks.Vision.HolisticLandmarker;
using TrackerPro.Unity;
using TrackerPro.Unity.CoordinateSystem;
using TrackerPro.Unity.Sample.Holistic;
using UnityEngine;
using System;
using TrackerPro.Tasks.Vision.HandLandmarker;
namespace TrackerPro
{
    [RequireComponent(typeof(Animator))]
    public class HandTracking : TrackingEventHandler
    {
        public bool isTrackingEnable = true;

        private Transform _bonePrefab;
        public RectTransform skeletonParent;

        public RectTransform screen;

        public HandBones leftHandBones;
        public HandBones rightHandBones;
        private Animator avatar;



        public float smoothness = .5f;
        [Range(0, 1)]
        public float visibilityThreshold = .6f;
        public bool isMirror = false;

        private List<BoneMapper> left_boneMappers = new List<BoneMapper>();
        public List<LandmarkData> left_bones = new List<LandmarkData>();
        private List<BoneMapper> right_boneMappers = new List<BoneMapper>();
        public List<LandmarkData> right_bones = new List<LandmarkData>();
        private int _numberOfBones = 21;
        HolisticTrackingResult holisticTrackingResult;
        private Quaternion leftWristInitialRotation;
        private Quaternion rightWristInitialRotation;
        private void Awake()
        {
        }
        private void Start()
        {
            _bonePrefab = new GameObject().transform;
            _bonePrefab.name = "BonePrefab";
            avatar = GetComponent<Animator>();
            leftWristInitialRotation = leftHandBones.wrist.rotation;
            rightWristInitialRotation = rightHandBones.wrist.rotation;

            MapBones(left_bones, left_boneMappers, isMirror ? rightHandBones : leftHandBones);
            MapBones(right_bones, right_boneMappers, isMirror ? leftHandBones : rightHandBones);

        }
        private void MapBones(List<LandmarkData> bones, List<BoneMapper> boneMappers, HandBones hand)
        {

            for (int i = 0; i < _numberOfBones; i++)
            {
                Transform trans = Instantiate(_bonePrefab, skeletonParent);
                LandmarkData landmarkData = new LandmarkData { transform = trans, visibility = 0 };
                trans.name = ((Hand)i).ToString();
                bones.Add(landmarkData);
            }


            BoneMapper boneMapper = new BoneMapper(hand.thumb1, hand.thumb2);
            boneMapper.refParent = bones[(int)Hand.THUMB_CMC];
            boneMapper.refChild = bones[(int)Hand.THUMB_MCP];
            boneMappers.Add(boneMapper);

            boneMapper = new BoneMapper(hand.thumb3, hand.thumb4);
            boneMapper.refParent = bones[(int)Hand.THUMB_IP];
            boneMapper.refChild = bones[(int)Hand.THUMB_TIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.index1, hand.index2);
            boneMapper.refParent = bones[(int)Hand.INDEX_FINGER_MCP];
            boneMapper.refChild = bones[(int)Hand.INDEX_FINGER_PIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.index3, hand.index4);
            boneMapper.refParent = bones[(int)Hand.INDEX_FINGER_DIP];
            boneMapper.refChild = bones[(int)Hand.INDEX_FINGER_TIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.middle1, hand.middle2);
            boneMapper.refParent = bones[(int)Hand.MIDDLE_FINGER_MCP];
            boneMapper.refChild = bones[(int)Hand.MIDDLE_FINGER_PIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.middle3, hand.middle4);
            boneMapper.refParent = bones[(int)Hand.MIDDLE_FINGER_DIP];
            boneMapper.refChild = bones[(int)Hand.MIDDLE_FINGER_TIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.ring1, hand.ring2);
            boneMapper.refParent = bones[(int)Hand.RING_FINGER_MCP];
            boneMapper.refChild = bones[(int)Hand.RING_FINGER_PIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.ring3, hand.ring4);
            boneMapper.refParent = bones[(int)Hand.RING_FINGER_DIP];
            boneMapper.refChild = bones[(int)Hand.RING_FINGER_TIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.pinky1, hand.pinky2);
            boneMapper.refParent = bones[(int)Hand.PINKY_MCP];
            boneMapper.refChild = bones[(int)Hand.PINKY_PIP];
            boneMappers.Add(boneMapper);


            boneMapper = new BoneMapper(hand.pinky3, hand.pinky4);
            boneMapper.refParent = bones[(int)Hand.PINKY_DIP];
            boneMapper.refChild = bones[(int)Hand.PINKY_TIP];
            boneMappers.Add(boneMapper);

            // boneMapper = new BoneMapper(hand.wrist, hand.middle1);
            // boneMapper.refParent = bones[(int)Hand.WRIST];
            // boneMapper.refChild = bones[(int)Hand.MIDDLE_FINGER_MCP];
            // boneMappers.Add(boneMapper);


        }
        public override void OnPoseUpdate(ITrackingResult solution)
        {
            if (solution is HolisticTrackingResult holisticResult)
            {
                holisticTrackingResult = holisticResult;

                //normalize landmark point
                if (!isTracking()||!isTrackingEnable)
                {

                    return;
                }
                if (holisticTrackingResult.leftHandLandmarks != null) UpdateBonePosition(holisticTrackingResult.leftHandLandmarks, left_bones, true);
                if (holisticTrackingResult.rightHandLandmarks != null) UpdateBonePosition(holisticTrackingResult.rightHandLandmarks, right_bones, false);
            }

        }
        private void UpdateBonePosition(NormalizedLandmarkList handLandmarkers, List<LandmarkData> bones, bool isLeft)
        {
            for (int i = 0; i < _numberOfBones; i++)
            {
                var position = new Vector3(handLandmarkers.Landmark[i].X, -handLandmarkers.Landmark[i].Y, -handLandmarkers.Landmark[i].Z * 2);
                bones[i].visibility = handLandmarkers.Landmark[i].Visibility;
                // bones[i].handType = GetHandType(handLandmarkerResult.handLandmarksAnnotationController.currentHandedness[0].Classification);
                bones[i].transform.position = position;
            }
        }

        private void LateUpdate()
        {
            if (!isTracking())
            {
                return;
            }
            ResetBone(left_boneMappers);
            ResetBone(right_boneMappers);


            SolveTracking(left_bones, left_boneMappers, leftHandBones, HandType.LEFT_HAND);
            SolveTracking(right_bones, right_boneMappers, rightHandBones, HandType.RIGHT_HAND);
           
            //if (scaleMatch) FixScale();

        }
        private void ResetBone(List<BoneMapper> boneMappers)
        {
            foreach (BoneMapper boneMapper in boneMappers)
            {
                boneMapper.parent.localRotation = boneMapper.initialRotation;
            }
            

        }
       
        private void SolveTracking(List<LandmarkData> bones, List<BoneMapper> boneMappers, HandBones hand, HandType handType)
        {

            //rotate wrist
            Transform wrist = hand.wrist;

            // ----- X-AXIS TARGET (RIGHT) -----
            Vector3 xAxis = handType == HandType.RIGHT_HAND ? (bones[(int)Hand. INDEX_FINGER_MCP].transform.position - bones[(int)Hand.PINKY_MCP].transform.position).normalized
                : (bones[(int)Hand. PINKY_MCP].transform.position - bones[(int)Hand. INDEX_FINGER_MCP].transform.position).normalized;

            // ----- Y-AXIS TARGET (UP) -----
            Vector3 yAxis = (bones[(int)Hand.MIDDLE_FINGER_MCP].transform.position -
                             bones[(int)Hand.WRIST].transform.position).normalized;

            // Orthogonalize the axes (to avoid twisting)
            Vector3 zAxis = Vector3.Cross(xAxis, yAxis);   // compute forward
            yAxis = Vector3.Cross(zAxis, xAxis);           // recompute clean up axis

            xAxis.Normalize();
            yAxis.Normalize();
            zAxis.Normalize();

            // Build rotation matrix
            Quaternion targetRot = Quaternion.LookRotation(zAxis, yAxis);
            // Smooth apply
            // wrist.rotation = Quaternion.Slerp(
            //     wrist.rotation,
            //     targetRot,
            //     smoothness
            // );
            foreach (BoneMapper boneMapper in boneMappers)
            {
                //if (boneMapper.refChild.visibility < visibilityThreshold) continue;
                Transform refParent = boneMapper.refParent.transform;       // mocap upper arm
                Transform refChild = boneMapper.refChild.transform;         // mocap hand
                Transform avatarParentBone = boneMapper.parent;   // avatar upper arm
                Transform avatarChildBone = boneMapper.child;     // avatar hand

                if (refParent == null || refChild == null || avatarParentBone == null || avatarChildBone == null)
                    continue;

                // Get current avatar limb direction in local space of the parent bone
                Vector3 vOld = (avatarChildBone.position - avatarParentBone.position).normalized;
                vOld = avatarParentBone.InverseTransformDirection(vOld);

                // Get reference limb direction (from mocap) in local space of the parent bone
                Vector3 vNew = (refChild.position - refParent.position).normalized;
                vNew = avatarParentBone.InverseTransformDirection(vNew);

                if (vOld.sqrMagnitude < 0.0001f || vNew.sqrMagnitude < 0.0001f)
                    continue;

                Quaternion rotOld = boneMapper.previousRotation;
                Quaternion rotNew = avatarParentBone.localRotation * Quaternion.FromToRotation(vOld, vNew).normalized;
                avatarParentBone.localRotation = Quaternion.Slerp(rotOld, rotNew, smoothness);
                boneMapper.previousRotation = avatarParentBone.localRotation;
            }





        }
        private bool isTracking()
        {
            if (holisticTrackingResult.leftHandLandmarks == null && holisticTrackingResult.rightHandLandmarks == null)
            {
                return false;
            }
            return true;
        }
        public Vector3 GetScreenPoint(NormalizedLandmark landmark)
        {
            var relative_position = screen.rect.GetPoint(landmark, 0, false);
            var local_position = screen.TransformPoint(relative_position);
            return local_position;
        }
    }
    [Serializable]
    public class HandBones
    {
        public Transform wrist;

        public Transform thumb1;
        public Transform thumb2;
        public Transform thumb3;
        public Transform thumb4;

        public Transform index1;
        public Transform index2;
        public Transform index3;
        public Transform index4;

        public Transform middle1;
        public Transform middle2;
        public Transform middle3;
        public Transform middle4;

        public Transform pinky1;
        public Transform pinky2;
        public Transform pinky3;
        public Transform pinky4;

        public Transform ring1;
        public Transform ring2;
        public Transform ring3;
        public Transform ring4;

    }
    public enum Hand
    {
        WRIST,
        THUMB_CMC,
        THUMB_MCP,
        THUMB_IP,
        THUMB_TIP,
        INDEX_FINGER_MCP,
        INDEX_FINGER_PIP,
        INDEX_FINGER_DIP,
        INDEX_FINGER_TIP,
        MIDDLE_FINGER_MCP,
        MIDDLE_FINGER_PIP,
        MIDDLE_FINGER_DIP,
        MIDDLE_FINGER_TIP,
        RING_FINGER_MCP,
        RING_FINGER_PIP,
        RING_FINGER_DIP,
        RING_FINGER_TIP,
        PINKY_MCP,
        PINKY_PIP,
        PINKY_DIP,
        PINKY_TIP
    }
    public enum HandType
    {
        LEFT_HAND,
        RIGHT_HAND
    }

}