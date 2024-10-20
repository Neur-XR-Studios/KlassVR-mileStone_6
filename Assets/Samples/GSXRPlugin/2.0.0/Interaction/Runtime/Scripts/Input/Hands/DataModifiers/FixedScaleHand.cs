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

namespace XR.Interaction.Input
{
    public class FixedScaleHand : Hand
    {
        [SerializeField]
        private float _scale = 1f;

        protected override void Apply(HandDataAsset data)
        {
            Pose rootToPointer = PoseUtils.Delta(data.Root, data.PointerPose);
            rootToPointer.position = (rootToPointer.position / data.HandScale) * _scale;
            PoseUtils.Multiply(data.Root, rootToPointer, ref data.PointerPose);

            data.HandScale = _scale;
        }

        #region Inject
        public void InjectAllFixedScaleDataModifier(UpdateModeFlags updateMode, IDataSource updateAfter,
            DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier,
            Component[] aspects, float scale)
        {
            base.InjectAllHand(updateMode, updateAfter, modifyDataFromSource, applyModifier, aspects);
            InjectScale(scale);
        }

        public void InjectScale(float scale)
        {
            _scale = scale;
        }
        #endregion
    }
}
