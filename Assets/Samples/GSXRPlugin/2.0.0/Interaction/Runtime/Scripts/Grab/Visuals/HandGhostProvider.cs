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

using XR.Interaction.Input;
using UnityEngine;

namespace XR.Interaction.HandGrab.Visuals
{
    /// <summary>
    /// Holds references to the prefabs for Ghost-Hands, so they can be instantiated
    /// in runtime to represent static poses.
    /// </summary>
    [CreateAssetMenu(menuName = "XR/Interaction/SDK/Pose Authoring/Hand Ghost Provider")]
    public class HandGhostProvider : ScriptableObject
    {
        /// <summary>
        /// The prefab for the left hand ghost.
        /// </summary>
        [SerializeField]
        private HandGhost _leftHand;
        /// <summary>
        /// The prefab for the right hand ghost.
        /// </summary>
        [SerializeField]
        private HandGhost _rightHand;

        /// <summary>
        /// Helper method to obtain the prototypes
        /// The result is to be instanced, not used directly.
        /// </summary>
        /// <param name="handedness">The desired handedness of the ghost prefab</param>
        /// <returns>A Ghost prefab</returns>
        public HandGhost GetHand(Handedness handedness)
        {
            return handedness == Handedness.Left ? _leftHand : _rightHand;
        }
    }
}
