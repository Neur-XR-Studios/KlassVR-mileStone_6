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

namespace XR.Interaction.Input.Visuals
{
    public class ControllerVisual : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IController))]
        private MonoBehaviour _controller;

        public IController Controller;

        [SerializeField]
        private XRControllerHelper _ControllerHelper;

        public bool ForceOffVisibility { get; set; }

        private bool _started = false;

        protected virtual void Awake()
        {
            Controller = _controller as IController;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Controller);
            Assert.IsNotNull(_ControllerHelper);
            switch (Controller.Handedness)
            {
                case Handedness.Left:
                    _ControllerHelper.m_controller = XRInput.Controller.LTouch;
                    break;
                case Handedness.Right:
                    _ControllerHelper.m_controller = XRInput.Controller.RTouch;
                    break;
            }
            this.EndStart(ref _started);
        }

        protected virtual void OnEnable()
        {
            if (_started)
            {
                Controller.WhenUpdated += HandleUpdated;
            }
        }

        protected virtual void OnDisable()
        {
            if (_started && _controller != null)
            {
                Controller.WhenUpdated -= HandleUpdated;
            }
        }

        private void HandleUpdated()
        {
            if (!Controller.IsConnected ||
                ForceOffVisibility ||
                !Controller.TryGetPose(out Pose rootPose))
            {
                _ControllerHelper.enabled = false;
                return;
            }

            _ControllerHelper.enabled = true;
            transform.position = rootPose.position;
            transform.rotation = rootPose.rotation;
            float parentScale = transform.parent != null ? transform.parent.lossyScale.x : 1f;
            transform.localScale = Controller.Scale / parentScale * Vector3.one;
        }

        #region Inject

        public void InjectAllControllerVisual(IController controller, XRControllerHelper ControllerHelper)
        {
            InjectController(controller);
            InjectAllControllerHelper(ControllerHelper);
        }

        public void InjectController(IController controller)
        {
            _controller = controller as MonoBehaviour;
            Controller = controller;
        }

        public void InjectAllControllerHelper(XRControllerHelper ControllerHelper)
        {
            _ControllerHelper = ControllerHelper;
        }

        #endregion
    }
}
