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

namespace XR.Interaction.Input
{
    public interface ICameraRigRef
    {
        XRCameraRig CameraRig { get; }
        /// <summary>
        /// Returns a valid Hand object representing the left hand, if one exists on the
        /// CameraRig. If none is available, returns null.
        /// </summary>
        XRHand LeftHand { get; }
        /// <summary>
        /// Returns a valid Hand object representing the right hand, if one exists on the
        /// CameraRig. If none is available, returns null.
        /// </summary>
        XRHand RightHand { get; }
        Transform LeftController { get; }
        Transform RightController { get; }

        event Action<bool> WhenInputDataDirtied;
    }

    /// <summary>
    /// Points to an CameraRig instance. This level of indirection provides a single
    /// configuration point on the root of a prefab.
    /// Must execute before all other  related classes so that the fields are
    /// initialized correctly and ready to use.
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class CameraRigRef : MonoBehaviour, ICameraRigRef
    {
        [Header("Configuration")]
        [SerializeField]
        private XRCameraRig _CameraRig;

        [SerializeField]
        private bool _requireHands = true;

        public XRCameraRig CameraRig => _CameraRig;

        private XRHand _leftHand;
        private XRHand _rightHand;
        public XRHand LeftHand => GetHandCached(ref _leftHand, _CameraRig.leftHandAnchor);
        public XRHand RightHand => GetHandCached(ref _rightHand, _CameraRig.rightHandAnchor);

        public Transform LeftController => _CameraRig.leftControllerAnchor;
        public Transform RightController => _CameraRig.rightControllerAnchor;

        public event Action<bool> WhenInputDataDirtied = delegate { };

        protected bool _started = false;

        private bool _isLateUpdate;

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(_CameraRig);
            this.EndStart(ref _started);
        }

        protected virtual void FixedUpdate()
        {
            _isLateUpdate = false;
        }

        protected virtual void Update()
        {
            _isLateUpdate = false;
         
        }

        protected virtual void LateUpdate()
        {
            _isLateUpdate = true;
        
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                CameraRig.UpdatedAnchors += HandleInputDataDirtied;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started)
            {
                CameraRig.UpdatedAnchors -= HandleInputDataDirtied;
            }
        }

        private XRHand GetHandCached(ref XRHand cachedValue, Transform handAnchor)
        {
            if (cachedValue != null)
            {
                return cachedValue;
            }

            cachedValue = handAnchor.GetComponentInChildren<XRHand>(true);
            if (_requireHands)
            {
                Assert.IsNotNull(cachedValue);
            }

            return cachedValue;
        }

        private void HandleInputDataDirtied(XRCameraRig cameraRig)
        {
            WhenInputDataDirtied(_isLateUpdate);
        }

        #region Inject
        public void InjectAllCameraRigRef(XRCameraRig CameraRig, bool requireHands)
        {
            InjectInteractionCameraRig(CameraRig);
            InjectRequireHands(requireHands);
        }

        public void InjectInteractionCameraRig(XRCameraRig CameraRig)
        {
            _CameraRig = CameraRig;
            // Clear the cached values to force new values to be read on next access
            _leftHand = null;
            _rightHand = null;
        }

        public void InjectRequireHands(bool requireHands)
        {
            _requireHands = requireHands;
        }
        #endregion
    }
}
