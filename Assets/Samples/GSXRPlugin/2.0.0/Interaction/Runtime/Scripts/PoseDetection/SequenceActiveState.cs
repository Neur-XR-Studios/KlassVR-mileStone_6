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

using UnityEngine;
using UnityEngine.Assertions;

namespace XR.Interaction.PoseDetection
{
    public class SequenceActiveState : MonoBehaviour, IActiveState
    {
        [SerializeField]
        private Sequence _sequence;

        [SerializeField]
        private bool _activateIfStepsStarted;

        [SerializeField]
        private bool _activateIfStepsComplete = true;

        protected virtual void Start()
        {
            Assert.IsNotNull(_sequence);
        }

        public bool Active
        {
            get
            {
                return (_activateIfStepsStarted && _sequence.CurrentActivationStep > 0 && !_sequence.Active) ||
                       (_activateIfStepsComplete && _sequence.Active);
            }
        }

        #region Inject

        public void InjectAllSequenceActiveState(Sequence sequence,
            bool activateIfStepsStarted, bool activateIfStepsComplete)
        {
            InjectSequence(sequence);
            InjectActivateIfStepsStarted(activateIfStepsStarted);
            InjectActivateIfStepsComplete(activateIfStepsComplete);
        }

        public void InjectSequence(Sequence sequence)
        {
            _sequence = sequence;
        }

        public void InjectActivateIfStepsStarted(bool activateIfStepsStarted)
        {
            _activateIfStepsStarted = activateIfStepsStarted;
        }

        public void InjectActivateIfStepsComplete(bool activateIfStepsComplete)
        {
            _activateIfStepsComplete = activateIfStepsComplete;
        }

        #endregion
    }
}
