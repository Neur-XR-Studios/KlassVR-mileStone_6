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

using XR.Interaction.Grab;
using XR.Interaction.HandGrab;
using XR.Interaction.Input;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace XR.Interaction.DistanceReticles
{
    public class ReticleGhostDrawer : InteractorReticle<ReticleDataGhost>
    {
        [SerializeField, Interface(typeof(IHandGrabber),typeof(IHandGrabState), typeof(IInteractorView))]
        private MonoBehaviour _handGrabber;
        private IHandGrabber HandGrabber { get; set; }
        private IHandGrabState HandGrabSource { get; set; }

        [FormerlySerializedAs("_modifier")]
        [SerializeField]
        private SyntheticHand _syntheticHand;

        [SerializeField]
        private HandVisual _visualHand;

        private bool _areFingersFree = true;
        private bool _isWristFree = true;

        protected override IInteractorView Interactor { get; set; }
        protected override Component InteractableComponent => HandGrabber.TargetInteractable as Component;

        private ITrackingToWorldTransformer Transformer;

        protected virtual void Awake()
        {
            HandGrabber = _handGrabber as IHandGrabber;
            HandGrabSource = _handGrabber as IHandGrabState;
            Interactor = _handGrabber as IInteractorView;
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            Assert.IsNotNull(HandGrabber, "Associated HandGrabber Hand can not be null");
            Assert.IsNotNull(Interactor, "Associated Interactor Hand can not be null");
            Assert.IsNotNull(HandGrabSource, "Associated HandGrabSource can not be null");
            Assert.IsNotNull(_visualHand, "Associated Visual Hand can not be null");
            Assert.IsNotNull(_syntheticHand, "Associated Synthetic hand can not be null");
            Transformer = _syntheticHand.GetData().Config.TrackingToWorldTransformer;
            this.EndStart(ref _started);
        }

        private void UpdateHandPose(IHandGrabState snapper)
        {
            HandGrabTarget snap = snapper.HandGrabTarget;

            if (snap == null)
            {
                FreeFingers();
                FreeWrist();
                return;
            }

            if (snap.HandPose != null)
            {
                UpdateFingers(snap.HandPose, snapper.GrabbingFingers());
                _areFingersFree = false;
            }
            else
            {
                FreeFingers();
            }

            Pose wristLocalPose = GetWristPose(snap.WorldGrabPose, snapper.WristToGrabPoseOffset);
            Pose wristPose = Transformer != null
                ? Transformer.ToTrackingPose(wristLocalPose)
                : wristLocalPose;
            _syntheticHand.LockWristPose(wristPose, 1f);
            _isWristFree = false;
        }

        private void UpdateFingers(HandPose handPose, HandFingerFlags grabbingFingers)
        {
            Quaternion[] desiredRotations = handPose.JointRotations;
            _syntheticHand.OverrideAllJoints(desiredRotations, 1f);

            for (int fingerIndex = 0; fingerIndex < Constants.NUM_FINGERS; fingerIndex++)
            {
                int fingerFlag = 1 << fingerIndex;
                JointFreedom fingerFreedom = handPose.FingersFreedom[fingerIndex];
                if (fingerFreedom == JointFreedom.Constrained
                    && ((int)grabbingFingers & fingerFlag) != 0)
                {
                    fingerFreedom = JointFreedom.Locked;
                }
                _syntheticHand.SetFingerFreedom((HandFinger)fingerIndex, fingerFreedom);
            }
        }

        private Pose GetWristPose(Pose gripPoint, Pose offset)
        {
            offset.Invert();
            gripPoint.Premultiply(offset);
            return gripPoint;
        }

        private bool FreeFingers()
        {
            if (!_areFingersFree)
            {
                _syntheticHand.FreeAllJoints();
                _areFingersFree = true;
                return true;
            }
            return false;
        }

        private bool FreeWrist()
        {
            if (!_isWristFree)
            {
                _syntheticHand.FreeWrist();
                _isWristFree = true;
                return true;
            }
            return false;
        }

        protected override void Align(ReticleDataGhost data)
        {
            UpdateHandPose(HandGrabSource);
            _syntheticHand.MarkInputDataRequiresUpdate();
        }

        protected override void Draw(ReticleDataGhost data)
        {
            _visualHand.ForceOffVisibility = false;
        }

        protected override void Hide()
        {
            _visualHand.ForceOffVisibility = true;
        }

        #region Inject

        public void InjectAllReticleGhostDrawer(IHandGrabber handGrabber,
            SyntheticHand syntheticHand, HandVisual visualHand)
        {
            InjectHandGrabber(handGrabber);
            InjectSyntheticHand(syntheticHand);
            InjectVisualHand(visualHand);
        }

        public void InjectHandGrabber(IHandGrabber handGrabber)
        {
            _handGrabber = handGrabber as MonoBehaviour;
            HandGrabber = handGrabber;
            Interactor = handGrabber as IInteractorView;
            HandGrabSource = handGrabber as IHandGrabState;
        }

        public void InjectSyntheticHand(SyntheticHand syntheticHand)
        {
            _syntheticHand = syntheticHand;
        }

        public void InjectVisualHand(HandVisual visualHand)
        {
            _visualHand = visualHand;
        }
        #endregion
    }
}
