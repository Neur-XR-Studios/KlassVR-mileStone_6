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
using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace XR.Interaction
{
    public class XRRayInteractorPinchVisual : MonoBehaviour
    {
        [SerializeField, Interface(typeof(IHand))]
        private MonoBehaviour _hand;

        private IHand Hand;

    

        [SerializeField]
        private SkinnedMeshRenderer _skinnedMeshRenderer;

        [SerializeField]
        AnimationCurve _remapCurve;

        [SerializeField]
        Vector2 _alphaRange = new Vector2(.1f, .4f);

        #region Properties

        public AnimationCurve RemapCurve
        {
            get
            {
                return _remapCurve;
            }
            set
            {
                _remapCurve = value;
            }
        }

        public Vector2 AlphaRange
        {
            get
            {
                return _alphaRange;
            }
            set
            {
                _alphaRange = value;
            }
        }

        #endregion

        protected bool _started = false;

        protected virtual void Awake()
        {
            Hand = _hand as IHand;
        }

        protected virtual void Start()
        {
            this.BeginStart(ref _started);
            Assert.IsNotNull(Hand);
            Assert.IsNotNull(_skinnedMeshRenderer);
            Assert.IsNotNull(_remapCurve);
           
            this.EndStart(ref _started);
        }
        protected virtual void OnEnable()
        {
            if (_started)
            {

                UpdateVisual();
            }
        }

        private void Update()
        {
            if (_started)
            {

                UpdateVisual();
            }
        }

 
        private void UpdateVisual()
        {
            if (!Hand.IsTrackedDataValid)
            {
                if (_skinnedMeshRenderer.enabled) _skinnedMeshRenderer.enabled = false;
                return;
            }

            if (!_skinnedMeshRenderer.enabled) _skinnedMeshRenderer.enabled = true;

            if (!Hand.GetJointPose(HandJointId.HandIndex3, out var poseIndex3)) return;
            if (!Hand.GetJointPose(HandJointId.HandThumb3, out var poseThumb3)) return;

            var isPinching = Hand.GetIndexFingerIsPinching();
            Vector3 midIndexThumb = Vector3.Lerp(poseThumb3.position, poseIndex3.position, 0.5f);
            Pose PointerPose = Pose.identity;
            Hand.GetPointerPose(out PointerPose);

            var thisTransform = transform;
            var deltaTarget = PointerPose.rotation * Vector3.forward;

            thisTransform.position = midIndexThumb;
            thisTransform.rotation = Quaternion.LookRotation(deltaTarget, Vector3.up);
            thisTransform.localScale = Vector3.one * Hand.Scale;

            var mappedPinchStrength = _remapCurve.Evaluate(Hand.GetFingerPinchStrength(HandFinger.Index));

            _skinnedMeshRenderer.material.color = isPinching ? Color.white : new Color(1f, 1f, 1f, Mathf.Lerp(_alphaRange.x, _alphaRange.y, mappedPinchStrength));
            _skinnedMeshRenderer.SetBlendShapeWeight(0, mappedPinchStrength * 100f);
            _skinnedMeshRenderer.SetBlendShapeWeight(1, mappedPinchStrength * 100f);
        }

        private void UpdateVisualState(InteractorStateChangeArgs args) => UpdateVisual();

        #region Inject

         
        public void InjectHand(IHand hand)
        {
            _hand = hand as MonoBehaviour;
            Hand = hand;
        }

 

        public void InjectSkinnedMeshRenderer(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            _skinnedMeshRenderer = skinnedMeshRenderer;
        }

        #endregion
    }
}
