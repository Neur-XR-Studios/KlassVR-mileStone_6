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

using UnityEngine;
using UnityEngine.Assertions;

namespace XR.Interaction.Input
{
    struct UsageMapping
    {
        public UsageMapping(ControllerButtonUsage usage, XRInput.Touch touch)
        {
            Usage = usage;
            Touch = touch;
            Button = XRInput.Button.None;
        }

        public UsageMapping(ControllerButtonUsage usage, XRInput.Button button)
        {
            Usage = usage;
            Touch = XRInput.Touch.None;
            Button = button;
        }

        public bool IsTouch => Touch != XRInput.Touch.None;
        public bool IsButton => Button != XRInput.Button.None;
        public ControllerButtonUsage Usage { get; }
        public XRInput.Touch Touch { get; }
        public XRInput.Button Button { get; }
    }

    /// <summary>
    /// Returns the Pointer Pose for the active controller model
    /// as found in the official prefabs.
    /// This point is usually located at the front tip of the controller.
    /// </summary>
    struct PointerPoseSelector
    {
     
        private static readonly Pose[] DEFAULT_POINTERS = new Pose[2]
        {
            new Pose(new Vector3(0.00899999961f, -0.00321028521f, 0.030869998f),
                Quaternion.Euler(359.209534f, 6.45196056f, 6.95544577f)),
            new Pose(new Vector3(-0.00899999961f, -0.00321028521f, 0.030869998f),
                Quaternion.Euler(359.209534f, 353.548035f, 353.044556f))
        };

        public Pose LocalPointerPose { get; private set; }

        public PointerPoseSelector(Handedness handedness)
        {
                LocalPointerPose = DEFAULT_POINTERS[(int)handedness];
        }
    }

    public class FromControllerDataSource : DataSource<ControllerDataAsset>
    {
        [Header(" Data Source")]
        [SerializeField, Interface(typeof(ICameraRigRef))]
        private MonoBehaviour _cameraRigRef;
        public ICameraRigRef CameraRigRef { get; private set; }

        [SerializeField]
        private bool _processLateUpdates = false;

        [Header("Shared Configuration")]
        [SerializeField]
        private Handedness _handedness;

        [SerializeField, Interface(typeof(ITrackingToWorldTransformer))]
        private MonoBehaviour _trackingToWorldTransformer;
        private ITrackingToWorldTransformer TrackingToWorldTransformer;

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

        private readonly ControllerDataAsset _controllerDataAsset = new ControllerDataAsset();
        private XRInput.Controller _Controller;
        private Transform _ControllerAnchor;
        private ControllerDataSourceConfig _config;

        private PointerPoseSelector _pointerPoseSelector;

        #region  Controller Mappings

        // Mappings from Unity XR CommonUsage to GSXR UnityXR Button/Touch.
        private static readonly UsageMapping[] ControllerUsageMappings =
        {
            new UsageMapping(ControllerButtonUsage.PrimaryButton, XRInput.Button.One),
            new UsageMapping(ControllerButtonUsage.PrimaryTouch, XRInput.Touch.One),
            new UsageMapping(ControllerButtonUsage.SecondaryButton, XRInput.Button.Two),
            new UsageMapping(ControllerButtonUsage.SecondaryTouch, XRInput.Touch.Two),
            new UsageMapping(ControllerButtonUsage.GripButton,
                XRInput.Button.PrimaryHandTrigger),
            new UsageMapping(ControllerButtonUsage.TriggerButton,
                XRInput.Button.PrimaryIndexTrigger),
            new UsageMapping(ControllerButtonUsage.MenuButton, XRInput.Button.Start),
            new UsageMapping(ControllerButtonUsage.Primary2DAxisClick,
                XRInput.Button.PrimaryThumbstick),
            new UsageMapping(ControllerButtonUsage.Primary2DAxisTouch,
                XRInput.Touch.PrimaryThumbstick),
            new UsageMapping(ControllerButtonUsage.Thumbrest, XRInput.Touch.PrimaryThumbRest)
        };

        #endregion

        protected void Awake()
        {
            TrackingToWorldTransformer = _trackingToWorldTransformer as ITrackingToWorldTransformer;
            HmdData = _hmdData as IDataSource<HmdDataAsset>;
            CameraRigRef = _cameraRigRef as ICameraRigRef;

            UpdateConfig();
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            Assert.IsNotNull(CameraRigRef);
            Assert.IsNotNull(TrackingToWorldTransformer);
            Assert.IsNotNull(HmdData);
            if (_handedness == Handedness.Left)
            {
                Assert.IsNotNull(CameraRigRef.LeftController);
                _ControllerAnchor = CameraRigRef.LeftController;
                _Controller = XRInput.Controller.LTouch;
            }
            else
            {
                Assert.IsNotNull(CameraRigRef.RightController);
                _ControllerAnchor = CameraRigRef.RightController;
                _Controller = XRInput.Controller.RTouch;
            }
            _pointerPoseSelector = new PointerPoseSelector(_handedness);

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

        private ControllerDataSourceConfig Config
        {
            get
            {
                if (_config != null)
                {
                    return _config;
                }

                _config = new ControllerDataSourceConfig()
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
            Config.HmdData = HmdData;
        }

        protected override void UpdateData()
        {
            _controllerDataAsset.Config = Config;
            var worldToTrackingSpace = CameraRigRef.CameraRig.transform.worldToLocalMatrix;
            Transform Controller = _ControllerAnchor;

            _controllerDataAsset.IsDataValid = true;
            _controllerDataAsset.IsConnected =
                (XRInput.GetConnectedControllers() & _Controller) > 0;
            if (!_controllerDataAsset.IsConnected)
            {
                // revert state fields to their defaults
                _controllerDataAsset.IsTracked = default;
                _controllerDataAsset.ButtonUsageMask = default;
                _controllerDataAsset.RootPoseOrigin = default;
                return;
            }

            _controllerDataAsset.IsTracked = true;

            // Update button usages
            _controllerDataAsset.ButtonUsageMask = ControllerButtonUsage.None;
            XRInput.Controller controllerMask = _Controller;
            foreach (UsageMapping mapping in ControllerUsageMappings)
            {
                bool usageActive;
                if (mapping.IsTouch)
                {
                    // usageActive = XRInput.Get(mapping.Touch, controllerMask);
                    usageActive = false;
                }
                else
                {
                    Assert.IsTrue(mapping.IsButton);
                    // usageActive = XRInput.Get(mapping.Button, controllerMask);
                    usageActive = false;
                }

                if (usageActive)
                {
                    _controllerDataAsset.ButtonUsageMask |= mapping.Usage;
                }
            }

            // Update poses

            // Convert controller pose from world to tracking space.
            Pose worldRoot = new Pose(Controller.position, Controller.rotation);
            _controllerDataAsset.RootPose.position = worldToTrackingSpace.MultiplyPoint3x4(worldRoot.position);
            _controllerDataAsset.RootPose.rotation = worldToTrackingSpace.rotation * worldRoot.rotation;
            _controllerDataAsset.RootPoseOrigin = PoseOrigin.RawTrackedPose;


            // Convert controller pointer pose from local to tracking space.
            Pose pointerPose =
                new Pose(Controller.transform.TransformPoint(_pointerPoseSelector.LocalPointerPose.position),
                    worldRoot.rotation * _pointerPoseSelector.LocalPointerPose.rotation);
            _controllerDataAsset.PointerPose.position = worldToTrackingSpace.MultiplyPoint3x4(pointerPose.position);
            _controllerDataAsset.PointerPose.rotation = worldToTrackingSpace.rotation * pointerPose.rotation;
            _controllerDataAsset.PointerPoseOrigin = PoseOrigin.RawTrackedPose;

        }

        protected override ControllerDataAsset DataAsset => _controllerDataAsset;

        #region Inject

        public void InjectAllFromControllerDataSource(UpdateModeFlags updateMode, IDataSource updateAfter,
            Handedness handedness, ITrackingToWorldTransformer trackingToWorldTransformer,
            IDataSource<HmdDataAsset> hmdData)
        {
            base.InjectAllDataSource(updateMode, updateAfter);
            InjectHandedness(handedness);
            InjectTrackingToWorldTransformer(trackingToWorldTransformer);
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

        public void InjectHmdData(IDataSource<HmdDataAsset> hmdData)
        {
            _hmdData = hmdData as MonoBehaviour;
            HmdData = hmdData;
        }

        #endregion
    }
}
