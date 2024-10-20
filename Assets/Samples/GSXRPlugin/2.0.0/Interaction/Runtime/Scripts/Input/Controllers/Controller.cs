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

namespace XR.Interaction.Input
{
    public class Controller :
        DataModifier<ControllerDataAsset>,
        IController
    {

        [SerializeField]
        [Tooltip("Provides access to additional functionality on top of what the IController interface provides.")]
        private Component[] _aspects;

        public Handedness Handedness => GetData().Config.Handedness;

        public bool IsConnected
        {
            get
            {
                var currentData = GetData();
                return currentData.IsDataValid && currentData.IsConnected;
            }
        }

        public bool IsPoseValid
        {
            get
            {
                var currentData = GetData();
                return currentData.IsDataValid &&
                       currentData.RootPoseOrigin != PoseOrigin.None;
            }
        }

        public bool IsPointerPoseValid
        {
            get
            {
                var currentData = GetData();
                return currentData.IsDataValid &&
                       currentData.PointerPoseOrigin != PoseOrigin.None;
            }
        }

        public event Action WhenUpdated = delegate { };

        private ITrackingToWorldTransformer TrackingToWorldTransformer =>
            GetData().Config.TrackingToWorldTransformer;

        public float Scale => TrackingToWorldTransformer != null
            ? TrackingToWorldTransformer.Transform.localScale.x
            : 1;

        public bool IsButtonUsageAnyActive(ControllerButtonUsage buttonUsage)
        {
            var currentData = GetData();
            return
                currentData.IsDataValid &&
                (buttonUsage & currentData.ButtonUsageMask) != 0;
        }

        public bool IsButtonUsageAllActive(ControllerButtonUsage buttonUsage)
        {
            var currentData = GetData();
            return currentData.IsDataValid &&
                   (buttonUsage & currentData.ButtonUsageMask) == buttonUsage;
        }

        /// <summary>
        /// Retrieves the current controller pose, in world space.
        /// </summary>
        /// <param name="pose">Set to current pose if `IsPoseValid`; Pose.identity otherwise</param>
        /// <returns>Value of `IsPoseValid`</returns>
        public bool TryGetPose(out Pose pose)
        {
            if (!IsPoseValid)
            {
                pose = Pose.identity;
                return false;
            }

            pose = GetData().Config.TrackingToWorldTransformer.ToWorldPose(GetData().RootPose);
            return true;
        }

        /// <summary>
        /// Retrieves the current controller pointer pose, in world space.
        /// </summary>
        /// <param name="pose">Set to current pose if `IsPoseValid`; Pose.identity otherwise</param>
        /// <returns>Value of `IsPoseValid`</returns>
        public bool TryGetPointerPose(out Pose pose)
        {
            if (!IsPointerPoseValid)
            {
                pose = Pose.identity;
                return false;
            }

            pose = GetData().Config.TrackingToWorldTransformer.ToWorldPose(GetData().PointerPose);
            return true;
        }

        public override void MarkInputDataRequiresUpdate()
        {
            base.MarkInputDataRequiresUpdate();

            if (Started)
            {
                WhenUpdated();
            }
        }

        protected override void Apply(ControllerDataAsset data)
        {
            // Default implementation does nothing, to allow instantiation of this modifier directly
        }

        public bool TryGetAspect<TAspect>(out TAspect foundAspect) where TAspect : class
        {
            foreach (Component aspect in _aspects)
            {
                foundAspect = aspect as TAspect;
                if (foundAspect != null)
                {
                    return true;
                }
            }

            foundAspect = null;
            return false;
        }
    }
}
