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

namespace XR.Interaction
{
    public class PointProximityField : MonoBehaviour, IProximityField
    {
        [SerializeField]
        private Transform _centerPoint;

        protected virtual void Start()
        {
            Assert.IsNotNull(_centerPoint);
        }

        public Vector3 ComputeClosestPoint(Vector3 point)
        {
            return _centerPoint.position;
        }

        #region Inject

        public void InjectAllPointProximityField(Transform centerPoint)
        {
            InjectCenterPoint(centerPoint);
        }

        public void InjectCenterPoint(Transform centerPoint)
        {
            _centerPoint = centerPoint;
        }

        #endregion
    }
}
