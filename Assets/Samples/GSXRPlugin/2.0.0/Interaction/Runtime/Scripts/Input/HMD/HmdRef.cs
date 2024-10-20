﻿/*
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
    /// A set of constants that are passed to each child of a Hand modifier tree from the root DataSource.
    /// </summary>
    public class HmdRef : MonoBehaviour, IHmd
    {
        [SerializeField, Interface(typeof(Hmd))]
        private MonoBehaviour _hmd;
        private IHmd Hmd;

        public event Action WhenUpdated
        {
            add => Hmd.WhenUpdated += value;
            remove => Hmd.WhenUpdated -= value;
        }

        protected virtual void Awake()
        {
            Hmd = _hmd as IHmd;
        }

        protected virtual void Start()
        {
            Assert.IsNotNull(Hmd);
        }

        public bool GetRootPose(out Pose pose)
        {
            return Hmd.GetRootPose(out pose);
        }

        #region Inject
        public void InjectAllHmdRef(IHmd hmd)
        {
            InjectHmd(hmd);
        }

        public void InjectHmd(IHmd hmd)
        {
            _hmd = hmd as MonoBehaviour;
            Hmd = hmd;
        }
        #endregion
    }
}
