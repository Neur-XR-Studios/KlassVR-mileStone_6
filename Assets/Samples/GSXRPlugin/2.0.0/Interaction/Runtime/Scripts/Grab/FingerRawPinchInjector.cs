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

namespace XR.Interaction.GrabAPI
{
    /// <summary>
    /// Use this component with a HandGrabAPI so it uses the Raw pinch detector
    /// instead of the standard Pinch and Palm finger APIS. Specially useful for
    /// ControllersAsHands since it uses the same value as the trigger presses.
    /// </summary>
    public class FingerRawPinchInjector : MonoBehaviour
    {
        [SerializeField]
        private HandGrabAPI _handGrabAPI;

        protected virtual void Awake()
        {
            _handGrabAPI.InjectOptionalFingerPinchAPI(new FingerRawPinchAPI());
            _handGrabAPI.InjectOptionalFingerGrabAPI(new FingerRawPinchAPI());
        }
    }
}
