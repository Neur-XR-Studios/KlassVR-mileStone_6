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
using System.Collections.Generic;
using Unity.XR.GSXR;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace XR.Interaction.Input
{
    public class XRHandSkeleton : MonoBehaviour, IHandSkeletonProvider
    {
        public static Vector3 RightArm = new Vector3(0.1700262f, -0.2708168f, -0.001440672f);
        public static Vector3 LeftArm = new Vector3(-0.1700262f, -0.2708168f, -0.001440672f);


        private readonly HandSkeleton[] _skeletons = { new HandSkeleton(), new HandSkeleton() };

        public HandSkeleton this[Handedness handedness] => _skeletons[(int)handedness];


        protected void Awake()
        {
            ApplyToSkeleton(SkeletonData.LeftSkeleton, _skeletons[0]);
            ApplyToSkeleton(SkeletonData.RightSkeleton, _skeletons[1]);
        }

        public static HandSkeleton CreateSkeletonData(Handedness handedness)
        {
            HandSkeleton handSkeleton = new HandSkeleton();

            // When running in the editor, the call to load the skeleton from Plugin may fail. Use baked skeleton
            // data.
            if (handedness == Handedness.Left)
            {
                ApplyToSkeleton(SkeletonData.LeftSkeleton, handSkeleton);
            }
            else
            {
                ApplyToSkeleton(SkeletonData.RightSkeleton, handSkeleton);
            }

            return handSkeleton;
        }

        private static void ApplyToSkeleton(in Skeleton Skeleton, HandSkeleton handSkeleton)
        {
            int numJoints = handSkeleton.joints.Length;
            Assert.AreEqual(Skeleton.NumBones, numJoints);

            for (int i = 0; i < numJoints; ++i)
            {
                ref var srcPose = ref Skeleton.Bones[i].Pose;
                handSkeleton.joints[i] = new HandSkeletonJoint()
                {
                    pose = new Pose()
                    {
                        position = srcPose.Position.FromFlippedXVector3f(),
                        rotation = srcPose.Orientation.FromFlippedXQuatf()
                    },
                    parent = Skeleton.Bones[i].ParentBoneIndex
                };
            }
        }



        public static bool GetHandState(Step step, Handedness handedness, ref HandState handState)
        {
            if (!Application.isEditor)
            {
                // attempt to avoid allocations if client provides appropriately pre-initialized HandState
                if (handState.BoneRotations == null || handState.BoneRotations.Length != (int)SkeletonConstants.MaxHandBones)
                {
                    handState.BoneRotations = new Quatf[(int)SkeletonConstants.MaxHandBones];
                }
                if (handState.PinchStrength == null || handState.PinchStrength.Length != (int)HandFinger.Max)
                {
                    handState.PinchStrength = new float[(int)HandFinger.Max];
                }
                if (handState.FingerConfidences == null || handState.FingerConfidences.Length != (int)HandFinger.Max)
                {
                    handState.FingerConfidences = new TrackingConfidence[(int)HandFinger.Max];
                }

                // unrolling the arrays is necessary to avoid per-frame allocations during marshaling
                bool TrackedState = GSXR_Plugin.GetHandTrackedState((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand));
                handState.Status = TrackedState ? (HandStatus.HandTracked | HandStatus.InputStateValid) : HandStatus.DominantHand;
                Pose[] contextposes = GSXR_Plugin.GetHandBonePoses((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand));


                float ThumbTouch = GSXR_Plugin.GetHandThumbTouch((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand));
                float IndexTouch = GSXR_Plugin.GetHandIndexTouch((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand));
                float IndexFinger = GSXR_Plugin.GetHandFingerBend((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand), UnityEngine.XR.HandFinger.Index);
                float MiddleFinger = GSXR_Plugin.GetHandFingerBend((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand), UnityEngine.XR.HandFinger.Middle);
                float RingFinger = GSXR_Plugin.GetHandFingerBend((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand), UnityEngine.XR.HandFinger.Ring);
                float PinkyFinger = GSXR_Plugin.GetHandFingerBend((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand), UnityEngine.XR.HandFinger.Pinky);


                handState.RootPose = new Posef(contextposes[0].position.FromFlippedZ(), contextposes[0].rotation.FromFlippedZ());

                handState.BoneRotations[0] = contextposes[0].rotation.FromFlippedX();
                handState.BoneRotations[1] = contextposes[0].rotation.FromFlippedX();

                handState.BoneRotations[2] = contextposes[1].rotation.FromFlippedX();
                handState.BoneRotations[3] = contextposes[2].rotation.FromFlippedX();
                handState.BoneRotations[4] = contextposes[3].rotation.FromFlippedX();
                handState.BoneRotations[5] = contextposes[4].rotation.FromFlippedX();
                handState.BoneRotations[19] = contextposes[5].rotation.FromFlippedX();

                handState.BoneRotations[6] = contextposes[7].rotation.FromFlippedX();
                handState.BoneRotations[7] = contextposes[8].rotation.FromFlippedX();
                handState.BoneRotations[8] = contextposes[9].rotation.FromFlippedX();
                handState.BoneRotations[20] = contextposes[10].rotation.FromFlippedX();

                handState.BoneRotations[9] = contextposes[12].rotation.FromFlippedX();
                handState.BoneRotations[10] = contextposes[13].rotation.FromFlippedX();
                handState.BoneRotations[11] = contextposes[14].rotation.FromFlippedX();
                handState.BoneRotations[21] = contextposes[15].rotation.FromFlippedX();

                handState.BoneRotations[12] = contextposes[17].rotation.FromFlippedX();
                handState.BoneRotations[13] = contextposes[18].rotation.FromFlippedX();
                handState.BoneRotations[14] = contextposes[19].rotation.FromFlippedX();
                handState.BoneRotations[22] = contextposes[20].rotation.FromFlippedX();

                handState.BoneRotations[15] = contextposes[21].rotation.FromFlippedX();
                handState.BoneRotations[16] = contextposes[22].rotation.FromFlippedX();
                handState.BoneRotations[17] = contextposes[23].rotation.FromFlippedX();
                handState.BoneRotations[18] = contextposes[24].rotation.FromFlippedX();
                handState.BoneRotations[23] = contextposes[25].rotation.FromFlippedX();


                handState.PinchStrength[0] = ThumbTouch;
                handState.PinchStrength[1] = IndexTouch;
                handState.PinchStrength[2] = MiddleFinger;
                handState.PinchStrength[3] = RingFinger;
                handState.PinchStrength[4] = PinkyFinger;


                //Debug.Log("handState.PinchStrength[0]:" + handState.PinchStrength[0]
                //    + " handState.PinchStrength[1]:" + handState.PinchStrength[1] 
                //    + " handState.PinchStrength[2]:" + handState.PinchStrength[2] 
                //    + " handState.PinchStrength[3]:" + handState.PinchStrength[3] 
                //    + " handState.PinchStrength[4]:" + handState.PinchStrength[4]);

                float PinchThreshold = GSXR_Plugin.GSXR_HANDTRACKINGAIRTIP_ThRESHOLD;

                handState.Pinches = HandFingerPinch.None;
                if (IndexTouch >= PinchThreshold)
                {
                    handState.Pinches = HandFingerPinch.Thumb | HandFingerPinch.Index;

                    handState.PinchStrength[0] = handState.PinchStrength[1];
                }
                else
                {
                    handState.PinchStrength[1] = IndexFinger;
                }

                if (MiddleFinger >= PinchThreshold)
                {
                    handState.Pinches |= HandFingerPinch.Middle;

                }
                if (RingFinger >= PinchThreshold)
                {
                    handState.Pinches |= HandFingerPinch.Ring;
                }
                if (PinkyFinger >= PinchThreshold)
                {
                    handState.Pinches |= HandFingerPinch.Pinky;
                }


                handState.PointerPose = Posef.identity;
                handState.HandScale = 1.0f;
                handState.HandConfidence = TrackingConfidence.High;
                handState.FingerConfidences[0] = TrackingConfidence.High;
                handState.FingerConfidences[1] = TrackingConfidence.High;
                handState.FingerConfidences[2] = TrackingConfidence.High;
                handState.FingerConfidences[3] = TrackingConfidence.High;
                handState.FingerConfidences[4] = TrackingConfidence.High;
                handState.RequestedTimeStamp = 0.008f;
                handState.SampleTimeStamp = 0.008f;
                return true;
            }
            else
            {
                // attempt to avoid allocations if client provides appropriately pre-initialized HandState
                if (handState.BoneRotations == null || handState.BoneRotations.Length != (int)SkeletonConstants.MaxHandBones)
                {
                    handState.BoneRotations = new Quatf[(int)SkeletonConstants.MaxHandBones];
                }
                if (handState.PinchStrength == null || handState.PinchStrength.Length != (int)HandFinger.Max)
                {
                    handState.PinchStrength = new float[(int)HandFinger.Max];
                }
                if (handState.FingerConfidences == null || handState.FingerConfidences.Length != (int)HandFinger.Max)
                {
                    handState.FingerConfidences = new TrackingConfidence[(int)HandFinger.Max];
                }
                handState.Status = (HandStatus.HandTracked | HandStatus.InputStateValid);

                return true;
            }
        }

        public static bool GetHandTrackedState( Handedness handedness)
        {
            bool TrackedState = GSXR_Plugin.GetHandTrackedState((handedness == Handedness.Right ? UnityEngine.XR.XRNode.RightHand : UnityEngine.XR.XRNode.LeftHand));
            return TrackedState;
        }
    }
}
