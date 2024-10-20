﻿/*
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

using UnityEngine;
using UnityEngine.Assertions;


namespace XR.Interaction.Input
{
    public class FromHandDataSource : DataSource<HandDataAsset>
    {
        [Header(" Data Source")]
        [SerializeField, Interface(typeof(ICameraRigRef))]
        private MonoBehaviour _cameraRigRef;

        [SerializeField]
        private bool _processLateUpdates = false;

        [Header("Shared Configuration")]
        [SerializeField]
        private Handedness _handedness;

        [SerializeField, Interface(typeof(ITrackingToWorldTransformer))]
        private MonoBehaviour _trackingToWorldTransformer;
        private ITrackingToWorldTransformer TrackingToWorldTransformer;

        [SerializeField, Interface(typeof(IHandSkeletonProvider))]
        private MonoBehaviour _handSkeletonProvider;
        private IHandSkeletonProvider HandSkeletonProvider;

        [SerializeField, Interface(typeof(IDataSource<HmdDataAsset>))]
        private MonoBehaviour _hmdData;
        private IDataSource<HmdDataAsset> HmdData;

        public bool ProcessLateUpdates
        {
            get
            {
                return _processLateUpdates;
            }
            set
            {
                _processLateUpdates = value;
            }
        }

        private HandDataAsset _handDataAsset = new HandDataAsset();
        private XRHand _Hand;
        private XRInput.Controller _Controller;
        private float _lastHandScale;
        private HandDataSourceConfig _config;

        private ICameraRigRef CameraRigRef;

        protected override HandDataAsset DataAsset => _handDataAsset;

        // Wrist rotations that come from  need correcting.
        public static Quaternion WristFixupRotation { get; } =
            new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

        protected virtual void Awake()
        {
            TrackingToWorldTransformer = _trackingToWorldTransformer as ITrackingToWorldTransformer;
            HmdData = _hmdData as IDataSource<HmdDataAsset>;
            CameraRigRef = _cameraRigRef as ICameraRigRef;
            HandSkeletonProvider = _handSkeletonProvider as IHandSkeletonProvider;

            UpdateConfig();
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(TrackingToWorldTransformer);
            Assert.IsNotNull(HandSkeletonProvider);
            Assert.IsNotNull(HmdData);
            if (_handedness == Handedness.Left)
            {
                _Hand = CameraRigRef.LeftHand;
                _Controller = XRInput.Controller.LHand;
            }
            else
            {
                _Hand = CameraRigRef.RightHand;
                _Controller = XRInput.Controller.RHand;
            }

            UpdateConfig();

            this.EndStart(ref _started);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_started)
            {
                CameraRigRef.WhenInputDataDirtied += HandleInputDataDirtied;
            }
        }

        protected override void OnDisable()
        {
            if (_started)
            {
                CameraRigRef.WhenInputDataDirtied -= HandleInputDataDirtied;
            }

            base.OnDisable();
        }

        private void HandleInputDataDirtied(bool isLateUpdate)
        {
            if (isLateUpdate && !_processLateUpdates)
            {
                return;
            }
            MarkInputDataRequiresUpdate();
        }


        private HandDataSourceConfig Config
        {
            get
            {
                if (_config != null)
                {
                    return _config;
                }

                _config = new HandDataSourceConfig()
                {
                    Handedness = _handedness
                };

                return _config;
            }
        }

        private void UpdateConfig()
        {
            Config.Handedness = _handedness;
            Config.TrackingToWorldTransformer = TrackingToWorldTransformer;
            Config.HandSkeleton = HandSkeletonProvider[_handedness];
            Config.HmdData = HmdData;
        }

        protected override void UpdateData()
        {
            _handDataAsset.Config = Config;
            _handDataAsset.IsDataValid = true;
            _handDataAsset.IsConnected = (XRInput.GetConnectedControllers() & _Controller) > 0;

            if (_Hand != null)
            {
                XRSkeleton.ISkeletonDataProvider skeletonProvider = _Hand;
                XRSkeleton.SkeletonPoseData poseData = skeletonProvider.GetSkeletonPoseData();
                if (poseData.IsDataValid && poseData.RootScale <= 0.0f)
                {
                    if (_lastHandScale <= 0.0f)
                    {
                        poseData.IsDataValid = false;
                    }
                    else
                    {
                        poseData.RootScale = _lastHandScale;
                    }
                }
                else
                {
                    _lastHandScale = poseData.RootScale;
                }

                if (poseData.IsDataValid && _handDataAsset.IsConnected)
                {
                    UpdateDataPoses(poseData);
                    return;
                }
            }

            // revert state fields to their defaults
            _handDataAsset.IsConnected = default;
            _handDataAsset.IsTracked = default;
            _handDataAsset.RootPoseOrigin = default;
            _handDataAsset.PointerPoseOrigin = default;
            _handDataAsset.IsHighConfidence = default;
            for (var fingerIdx = 0; fingerIdx < Constants.NUM_FINGERS; fingerIdx++)
            {
                _handDataAsset.IsFingerPinching[fingerIdx] = default;
                _handDataAsset.IsFingerHighConfidence[fingerIdx] = default;
            }
        }

        private void UpdateDataPoses(XRSkeleton.SkeletonPoseData poseData)
        {
            _handDataAsset.HandScale = poseData.RootScale;
            _handDataAsset.IsTracked = _Hand.IsTracked;
            _handDataAsset.IsHighConfidence = poseData.IsDataHighConfidence;
            _handDataAsset.IsDominantHand = _Hand.IsDominantHand;
            _handDataAsset.RootPoseOrigin = _handDataAsset.IsTracked
                ? PoseOrigin.RawTrackedPose
                : PoseOrigin.None;

            for (var fingerIdx = 0; fingerIdx < Constants.NUM_FINGERS; fingerIdx++)
            {
                var FingerIdx = (HandFinger)fingerIdx;
                bool isPinching = _Hand.GetFingerIsPinching(FingerIdx);
                _handDataAsset.IsFingerPinching[fingerIdx] = isPinching;

                bool isHighConfidence =
                    _Hand.GetFingerConfidence(FingerIdx) == TrackingConfidence.High;
                _handDataAsset.IsFingerHighConfidence[fingerIdx] = isHighConfidence;

                float fingerPinchStrength = _Hand.GetFingerPinchStrength(FingerIdx);
                _handDataAsset.FingerPinchStrength[fingerIdx] = fingerPinchStrength;
            }

            // Read the poses directly from the poseData, so it isn't in conflict with
            // any modifications that the application makes to Skeleton
            _handDataAsset.Root = new Pose()
            {
                position = poseData.RootPose.Position.FromFlippedZVector3f(),
                rotation = poseData.RootPose.Orientation.FromFlippedZQuatf()
            };
          if (_Hand.IsPointerPoseValid)
            {
                _handDataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;
                _handDataAsset.PointerPose = new Pose(_Hand.PointerPose.localPosition,
                        _Hand.PointerPose.localRotation);
            }
            else
            {
                _handDataAsset.PointerPoseOrigin = PoseOrigin.None;
            }

            // Hand joint rotations X axis needs flipping to get to Unity's coordinate system.
            var bones = poseData.BoneRotations;
            for (int i = 0; i < bones.Length; i++)
            {
                // When using Link in the Unity Editor, the first frame of hand data
                // sometimes contains bad joint data.
                _handDataAsset.Joints[i] = float.IsNaN(bones[i].w)
                    ? Config.HandSkeleton.joints[i].pose.rotation
                    : bones[i].FromFlippedXQuatf();
            }

            _handDataAsset.Joints[0] = WristFixupRotation;
        }

        #region Inject

        public void InjectAllFromHandDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            Handedness handedness, ITrackingToWorldTransformer trackingToWorldTransformer,
            IHandSkeletonProvider handSkeletonProvider, IDataSource<HmdDataAsset> hmdData)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectHandedness(handedness);
            InjectTrackingToWorldTransformer(trackingToWorldTransformer);
            InjectHandSkeletonProvider(handSkeletonProvider);
            InjectHmdData(hmdData);
        }

        public void InjectHandedness(Handedness handedness)
        {
            _handedness = handedness;
        }

        public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            _trackingToWorldTransformer = trackingToWorldTransformer as MonoBehaviour;
            TrackingToWorldTransformer = trackingToWorldTransformer;
        }

        public void InjectHandSkeletonProvider(IHandSkeletonProvider handSkeletonProvider)
        {
            _handSkeletonProvider = handSkeletonProvider as MonoBehaviour;
            HandSkeletonProvider = handSkeletonProvider;
        }

        public void InjectHmdData(IDataSource<HmdDataAsset> hmdData)
        {
            _hmdData = hmdData as MonoBehaviour;
            HmdData = hmdData;
        }

        #endregion
    }
}
