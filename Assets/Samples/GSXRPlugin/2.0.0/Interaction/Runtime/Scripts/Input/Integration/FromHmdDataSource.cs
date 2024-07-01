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
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.XR;

namespace XR.Interaction.Input
{
    public class FromHmdDataSource : DataSource<HmdDataAsset>
    {
        [Header(" Data Source")]
        [SerializeField, Interface(typeof(ICameraRigRef))]
        private MonoBehaviour _cameraRigRef;

        public ICameraRigRef CameraRigRef { get; private set; }

        [SerializeField]
        private bool _processLateUpdates = false;

        [SerializeField]
        [Tooltip("If true, uses Manager.headPoseRelativeOffset rather than sensor data for " +
                 "HMD pose.")]
        private bool _useManagerEmulatedPose = false;

        [Header("Shared Configuration")]
        [SerializeField, Interface(typeof(ITrackingToWorldTransformer))]
        private MonoBehaviour _trackingToWorldTransformer;
        private ITrackingToWorldTransformer TrackingToWorldTransformer;

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

        private HmdDataAsset _hmdDataAsset = new HmdDataAsset();
        private HmdDataSourceConfig _config;

        protected void Awake()
        {
            CameraRigRef = _cameraRigRef as ICameraRigRef;
            TrackingToWorldTransformer = _trackingToWorldTransformer as ITrackingToWorldTransformer;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(TrackingToWorldTransformer);
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

        private HmdDataSourceConfig Config
        {
            get
            {
                if (_config != null)
                {
                    return _config;
                }

                _config = new HmdDataSourceConfig()
                {
                    TrackingToWorldTransformer = TrackingToWorldTransformer
                };

                return _config;
            }
        }

        protected override void UpdateData()
        {
            _hmdDataAsset.Config = Config;
            bool hmdPresent = true;
            ref var centerEyePose = ref _hmdDataAsset.Root;
            if (_useManagerEmulatedPose)
            {
               
                centerEyePose.rotation =Camera.main.transform.rotation;
                centerEyePose.position = Camera.main.transform.position;
                hmdPresent = true;
            }
            else
            {
                var previousEyePose = Pose.identity;

                if (_hmdDataAsset.IsTracked)
                {
                    previousEyePose = _hmdDataAsset.Root;
                }

                if (hmdPresent)
                {
                    centerEyePose.rotation = Camera.main.transform.rotation;
                    centerEyePose.position = Camera.main.transform.position;
                }
                else
                {
                    centerEyePose = previousEyePose;
                }
            }

            _hmdDataAsset.IsTracked = hmdPresent;
            _hmdDataAsset.FrameId = Time.frameCount;
        }

        protected override HmdDataAsset DataAsset => _hmdDataAsset;

        #region Inject

        public void InjectAllFromHmdDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            bool useManagerEmulatedPose, ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectUseManagerEmulatedPose(useManagerEmulatedPose);
            InjectTrackingToWorldTransformer(trackingToWorldTransformer);
        }

        public void InjectUseManagerEmulatedPose(bool useManagerEmulatedPose)
        {
            _useManagerEmulatedPose = useManagerEmulatedPose;
        }

        public void InjectTrackingToWorldTransformer(ITrackingToWorldTransformer trackingToWorldTransformer)
        {
            _trackingToWorldTransformer = trackingToWorldTransformer as MonoBehaviour;
            TrackingToWorldTransformer = trackingToWorldTransformer;
        }

        #endregion
    }
}
