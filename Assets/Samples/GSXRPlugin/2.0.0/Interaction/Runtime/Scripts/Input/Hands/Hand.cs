/*
 * Copyright (c) NoloVR Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the GSXR UnityXR SDK License Agreement (the "License");
 * you may not use the GSXR UnityXR SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://www.gsxr.org.cn/
 *
 * Unless required by applicable law or agreed to in writing, the GSXR UnityXR SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XR.Interaction.Input
{
    // A top level component that provides hand pose data, pinch states, and more.
    // Rather than sourcing data directly from the runtime layer, provides one
    // level of abstraction so that the aforementioned data can be injected
    // from other sources.
    public class Hand : DataModifier<HandDataAsset>, IHand
    {
        [SerializeField]
        [Tooltip(
            "Provides access to additional functionality on top of what the IHand interface provides." +
            "For example, this list can be used to provide access to the SkinnedMeshRenderer through " +
            "the IHand.GetHandAspect method.")]
        private Component[] _aspects;

        public IReadOnlyList<Component> Aspects => _aspects;

        public Handedness Handedness => GetData().Config.Handedness;

        public ITrackingToWorldTransformer TrackingToWorldTransformer =>
            GetData().Config.TrackingToWorldTransformer;

        public HandSkeleton HandSkeleton => GetData().Config.HandSkeleton;
        public IDataSource<HmdDataAsset> HmdData => GetData().Config.HmdData;

        private HandJointCache _jointPosesCache;

        public event Action WhenHandUpdated = delegate { };

        public bool IsConnected => GetData().IsDataValidAndConnected;
        public bool IsHighConfidence => GetData().IsHighConfidence;
        public bool IsDominantHand => GetData().IsDominantHand;

        public float Scale => GetData().HandScale * (TrackingToWorldTransformer != null
            ? TrackingToWorldTransformer.Transform.localScale.x
            : 1);

        private static readonly Vector3 PALM_LOCAL_OFFSET = new Vector3(0.08f, -0.01f, 0.0f);


 
        protected override void Apply(HandDataAsset data)
        {
          

           
            // Default implementation does nothing, to allow instantiation of this modifier directly
        }

        public override void MarkInputDataRequiresUpdate()
        {
            base.MarkInputDataRequiresUpdate();

            if (Started)
            {
                InitializeJointPosesCache();
                WhenHandUpdated.Invoke();
            }
        }

        private void InitializeJointPosesCache()
        {
            if (_jointPosesCache == null && GetData().IsDataValidAndConnected)
            {
                _jointPosesCache = new HandJointCache(HandSkeleton);
            }
        }

        private void CheckJointPosesCacheUpdate()
        {
            if (_jointPosesCache != null
                && CurrentDataVersion != _jointPosesCache.LocalDataVersion)
            {
                _jointPosesCache.Update(GetData(), CurrentDataVersion);
            }
        }

        #region IHandState implementation
        /// <summary>
        /// 抓取阈值设置
        /// </summary>
        private float grabbingThreshold = 0.5f;
        public float GrabbingThreshold { get => grabbingThreshold; set => grabbingThreshold = value; }

        public bool GetFingerIsGrabbing(HandFinger finger)
        {
            HandDataAsset currentData = GetData();
            return currentData.IsConnected && currentData.FingerPinchStrength[(int)finger]>= GrabbingThreshold;
        }
        public bool GetFingerIsPinching(HandFinger finger)
        {
            HandDataAsset currentData = GetData();
            return currentData.IsConnected && currentData.IsFingerPinching[(int)finger];
        }

        public bool GetIndexFingerIsPinching()
        {
            return GetFingerIsPinching(HandFinger.Index);
        }

        public bool IsPointerPoseValid => IsPoseOriginAllowed(GetData().PointerPoseOrigin);
        Pose PoseHandWristRoot = new Pose();
        Pose PoseHandThumb0 = new Pose();
        Pose PoseHandThumb1 = new Pose();
        Pose PoseHandThumb2 = new Pose();
        Pose PoseHandIndex1 = new Pose();

        private float lerpspeed = 8f;
        Pose sourcePose = new Pose();
        Pose CalculationPointerPose = Pose.identity;
        public bool GetPointerPose(out Pose pose)
        {
            //HandDataAsset currentData = GetData();
            //return ValidatePose(currentData.PointerPose, currentData.PointerPoseOrigin,
            //    out pose);

            GetJointPose(HandJointId.HandThumb0, out PoseHandThumb0);
            GetJointPose(HandJointId.HandThumb1, out PoseHandThumb1);
            GetJointPose(HandJointId.HandThumb2, out PoseHandThumb2);

            GetJointPose(HandJointId.HandIndex1, out PoseHandIndex1);

            Vector3 PalmPose = (PoseHandThumb0.position + PoseHandThumb1.position + PoseHandThumb2.position + PoseHandIndex1.position) / 4.0f;
            Vector3 Arm = Vector3.zero;

            if (Handedness == Handedness.Left)
            {
                Arm = XRHandSkeleton.LeftArm;
            }
            else
            {
                Arm = XRHandSkeleton.RightArm;
            }
            Pose ArmPose = Pose.identity;
            ArmPose.position = Arm;

            Pose TargetStart = TrackingToWorldTransformer != null
                  ? TrackingToWorldTransformer.ToWorldPose(ArmPose)
                  : sourcePose;


            Vector3 mDeltaTarget = -(TargetStart.position - PalmPose).normalized;

            sourcePose.position = PalmPose;
            sourcePose.rotation = Quaternion.LookRotation(mDeltaTarget, Vector3.up);


            Vector3 hitPoint = Vector3.zero;


            Camera m_Camera = Camera.main;

            Vector3 handPalmDirection = m_Camera.transform.forward;
            
            GetJointPose(HandJointId.HandWristRoot, out PoseHandWristRoot);
            if (Handedness == Handedness.Right)
            {
                handPalmDirection = -PoseHandWristRoot.up;
            }
            else
            {
                handPalmDirection = PoseHandWristRoot.up;

            }
            if (handPalmDirection == Vector3.zero)
            {
                handPalmDirection = m_Camera.transform.forward;
            }
            else
            {
                handPalmDirection = Vector3.ProjectOnPlane(handPalmDirection, m_Camera.transform.right);
            }

            Plane PlaneNear = new Plane(handPalmDirection, m_Camera.transform.position + m_Camera.transform.forward * 0.3f);

                Vector3 PointerPosePointNearTarget = PlaneNear.ClosestPointOnPlane(sourcePose.position + m_Camera.transform.up * 0.21f);

            if (Handedness == Handedness.Right)
            {
                PointerPosePointNearTarget -= m_Camera.transform.right * 0.15f;
            }
            else
            {
                PointerPosePointNearTarget += m_Camera.transform.right * 0.15f;
            }
            Ray ray = m_Camera.ViewportPointToRay(m_Camera.WorldToViewportPoint(PointerPosePointNearTarget));

                CalculationPointerPose.position = Vector3.LerpUnclamped(CalculationPointerPose.position, TargetStart.position, Time.deltaTime * lerpspeed);
                CalculationPointerPose.rotation = Quaternion.LerpUnclamped(CalculationPointerPose.rotation, Quaternion.LookRotation(ray.direction, Vector3.up), Time.deltaTime * lerpspeed);
                pose = CalculationPointerPose;

                return true;
            
           
        }

        public bool GetJointPose(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;

            if (!IsTrackedDataValid
                || _jointPosesCache == null
                || !GetRootPose(out Pose rootPose))
            {
                return false;
            }
            CheckJointPosesCacheUpdate();
            pose = _jointPosesCache.WorldJointPose(handJointId, rootPose, Scale);
            return true;
        }

        public bool GetJointPoseLocal(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;
            if (!GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses))
            {
                return false;
            }

            pose = localJointPoses[(int)handJointId];
            return true;
        }

        public bool GetJointPosesLocal(out ReadOnlyHandJointPoses localJointPoses)
        {
            if (!IsTrackedDataValid || _jointPosesCache == null)
            {
                localJointPoses = ReadOnlyHandJointPoses.Empty;
                return false;
            }
            CheckJointPosesCacheUpdate();
            return _jointPosesCache.GetAllLocalPoses(out localJointPoses);
        }

        public bool GetJointPoseFromWrist(HandJointId handJointId, out Pose pose)
        {
            pose = Pose.identity;
            if (!GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist))
            {
                return false;
            }

            pose = jointPosesFromWrist[(int)handJointId];
            return true;
        }

        public bool GetJointPosesFromWrist(out ReadOnlyHandJointPoses jointPosesFromWrist)
        {
            if (!IsTrackedDataValid || _jointPosesCache == null)
            {
                jointPosesFromWrist = ReadOnlyHandJointPoses.Empty;
                return false;
            }
            CheckJointPosesCacheUpdate();
            return _jointPosesCache.GetAllPosesFromWrist(out jointPosesFromWrist);
        }

        public bool GetPalmPoseLocal(out Pose pose)
        {
            Quaternion rotationQuat = Quaternion.identity;
            Vector3 offset = PALM_LOCAL_OFFSET;
            if (Handedness == Handedness.Left)
            {
                offset = -offset;
            }
            pose = new Pose(offset * Scale, rotationQuat);
            return true;
        }

        public bool GetFingerIsHighConfidence(HandFinger finger)
        {
            return GetData().IsFingerHighConfidence[(int)finger];
        }

        public float GetFingerPinchStrength(HandFinger finger)
        {
            return GetData().FingerPinchStrength[(int)finger];
        }

        public bool IsTrackedDataValid => GetData().IsTracked && GetData().IsDataValid;

        public bool GetRootPose(out Pose pose)
        {
            HandDataAsset currentData = GetData();
            return ValidatePose(currentData.Root, currentData.RootPoseOrigin, out pose);
        }

        public bool IsCenterEyePoseValid => HmdData.GetData().IsTracked;

        public bool GetCenterEyePose(out Pose pose)
        {
            HmdDataAsset hmd = HmdData.GetData();

            if (hmd == null || !hmd.IsTracked)
            {
                pose = Pose.identity;
                return false;
            }

            pose = TrackingToWorldTransformer.ToWorldPose(hmd.Root);
            return true;
        }

        #endregion


        public Transform TrackingToWorldSpace
        {
            get
            {
                if (TrackingToWorldSpace == null)
                {
                    return null;
                }
                return TrackingToWorldTransformer.Transform;
            }
        }

  
        private bool ValidatePose(in Pose sourcePose, PoseOrigin sourcePoseOrigin, out Pose pose)
        {
            if (IsPoseOriginDisallowed(sourcePoseOrigin))
            {
                pose = Pose.identity;
                return false;
            }

            pose = TrackingToWorldTransformer != null
                ? TrackingToWorldTransformer.ToWorldPose(sourcePose)
                : sourcePose;

            return true;
        }

        private bool IsPoseOriginAllowed(PoseOrigin poseOrigin)
        {
            return poseOrigin != PoseOrigin.None;
        }

        private bool IsPoseOriginDisallowed(PoseOrigin poseOrigin)
        {
            return poseOrigin == PoseOrigin.None;
        }

        public bool TryGetAspect<TAspect>(out TAspect foundComponent) where TAspect : class
        {
            foreach (Component aspect in _aspects)
            {
                foundComponent = aspect as TAspect;
                if (foundComponent != null)
                {
                    return true;
                }
            }

            foundComponent = null;
            return false;
        }

        #region Inject

        public void InjectAllHand(UpdateModeFlags updateMode, IDataSource updateAfter,
            DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier,
            Component[] aspects)
        {
            base.InjectAllDataModifier(updateMode, updateAfter, modifyDataFromSource, applyModifier);
            InjectAspects(aspects);
        }

        public void InjectAspects(Component[] aspects)
        {
            _aspects = aspects;
        }

        #endregion
    }
}
