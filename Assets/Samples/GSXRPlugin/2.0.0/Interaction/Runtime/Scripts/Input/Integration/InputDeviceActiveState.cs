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

using System.Collections.Generic;
using UnityEngine;

namespace XR.Interaction.Input
{
    /// <summary>
    /// Returns the active status of an Input device based on whether
    /// Input's current active controller matches any of the controller
    /// types set up in the inspector. Input `Controllers` include
    /// types like Touch, L Touch, R TouchR, Hands, L Hand, R Hand
    /// </summary>
    public class InputDeviceActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private List<XRInput.Controller> _controllerTypes;

        public bool Active
        {
            get
            {
                foreach (XRInput.Controller controllerType in _controllerTypes)
                {
                    if (XRInput.GetConnectedControllers() == controllerType) return true;
                }
                return false;
            }
        }

        #region Inject

        public void InjectAllInputDeviceActiveState(List<XRInput.Controller> controllerTypes)
        {
            InjectControllerTypes(controllerTypes);
        }

        public void InjectControllerTypes(List<XRInput.Controller> controllerTypes)
        {
            _controllerTypes = controllerTypes;
        }

        #endregion
    }
}
