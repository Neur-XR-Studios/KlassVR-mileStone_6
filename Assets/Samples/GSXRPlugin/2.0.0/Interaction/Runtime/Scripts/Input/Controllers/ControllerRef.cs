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
    /// <summary>
    /// ControllerRef is a utility component that delegates all of its IController implementation
    /// to the provided Controller object.
    /// </summary>
    public class ControllerRef : MonoBehaviour, IController, IActiveState
    {
        [SerializeField, Interface(typeof(IController))]
        private MonoBehaviour _controller;
        private IController Controller;

        protected virtual void Awake()
        {
            Controller = _controller as IController;
        }

        protected virtual void Start()
        {
            Assert.IsNotNull(Controller);
        }

        public Handedness Handedness => Controller.Handedness;

        public bool IsConnected => Controller.IsConnected;

        public bool IsPoseValid => Controller.IsPoseValid;

        public event Action WhenUpdated
        {
            add => Controller.WhenUpdated += value;
            remove => Controller.WhenUpdated -= value;
        }

        public bool Active => IsConnected;

        public bool TryGetPose(out Pose pose)
        {
            return Controller.TryGetPose(out pose);
        }

        public bool TryGetPointerPose(out Pose pose)
        {
            return Controller.TryGetPointerPose(out pose);
        }

        public float Scale => Controller.Scale;

        public bool IsButtonUsageAnyActive(ControllerButtonUsage buttonUsage)
        {
            return Controller.IsButtonUsageAnyActive(buttonUsage);
        }

        public bool IsButtonUsageAllActive(ControllerButtonUsage buttonUsage)
        {
            return Controller.IsButtonUsageAllActive(buttonUsage);
        }

        public bool TryGetAspect<TAspect>(out TAspect aspect) where TAspect : class
        {
            return Controller.TryGetAspect(out aspect);
        }

        #region Inject
        public void InjectAllControllerRef(IController controller)
        {
            InjectController(controller);
        }

        public void InjectController(IController controller)
        {
            _controller = controller as MonoBehaviour;
            Controller = controller;
        }

        #endregion
    }
}
