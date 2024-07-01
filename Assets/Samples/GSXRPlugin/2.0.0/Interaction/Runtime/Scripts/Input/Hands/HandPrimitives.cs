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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Primitive type serialization
/// </summary>
namespace XR.Interaction.Input
{

    public enum Step
    {
        Render = -1,
        Physics = 0,  
    }
    public static class Constants
    {
        public const int NUM_HAND_JOINTS = (int)HandJointId.HandEnd;
        public const int NUM_FINGERS = 5;
    }
    public enum TrackingConfidence
    {
        Low = 0,
        High = 0x3f800000,
    }
    [Flags]
    public enum HandStatus
    {
        HandTracked = (1 << 0), // if this is set the hand pose and bone rotations data is usable
        InputStateValid = (1 << 1), // if this is set the pointer pose and pinch data is usable
        SystemGestureInProgress = (1 << 6), // if this is set the hand is currently processing a system gesture
        DominantHand = (1 << 7), // if this is set the hand is currently the dominant hand
        MenuPressed = (1 << 8) // if this is set the hand performed a menu press
    }
    [Flags]
    public enum HandFingerPinch
    {
        None=0,
        Thumb = (1 << HandFinger.Thumb),
        Index = (1 << HandFinger.Index),
        Middle = (1 << HandFinger.Middle),
        Ring = (1 << HandFinger.Ring),
        Pinky = (1 << HandFinger.Pinky),
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HandState
    {
        public HandStatus Status;
        public Posef RootPose;
        public Quatf[] BoneRotations;
        public HandFingerPinch Pinches;
        public float[] PinchStrength;
        public Posef PointerPose;
        public float HandScale;
        public TrackingConfidence HandConfidence;
        public TrackingConfidence[] FingerConfidences;
        public double RequestedTimeStamp;
        public double SampleTimeStamp;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct HandStateInternal
    {
        public HandStatus Status;
        public Posef RootPose;
        public Quatf BoneRotations_0;
        public Quatf BoneRotations_1;
        public Quatf BoneRotations_2;
        public Quatf BoneRotations_3;
        public Quatf BoneRotations_4;
        public Quatf BoneRotations_5;
        public Quatf BoneRotations_6;
        public Quatf BoneRotations_7;
        public Quatf BoneRotations_8;
        public Quatf BoneRotations_9;
        public Quatf BoneRotations_10;
        public Quatf BoneRotations_11;
        public Quatf BoneRotations_12;
        public Quatf BoneRotations_13;
        public Quatf BoneRotations_14;
        public Quatf BoneRotations_15;
        public Quatf BoneRotations_16;
        public Quatf BoneRotations_17;
        public Quatf BoneRotations_18;
        public Quatf BoneRotations_19;
        public Quatf BoneRotations_20;
        public Quatf BoneRotations_21;
        public Quatf BoneRotations_22;
        public Quatf BoneRotations_23;
        public HandFingerPinch Pinches;
        public float PinchStrength_0;
        public float PinchStrength_1;
        public float PinchStrength_2;
        public float PinchStrength_3;
        public float PinchStrength_4;
        public Posef PointerPose;
        public float HandScale;
        public TrackingConfidence HandConfidence;
        public TrackingConfidence FingerConfidences_0;
        public TrackingConfidence FingerConfidences_1;
        public TrackingConfidence FingerConfidences_2;
        public TrackingConfidence FingerConfidences_3;
        public TrackingConfidence FingerConfidences_4;
        public double RequestedTimeStamp;
        public double SampleTimeStamp;
    }

    public enum Handedness
    {
        Left = 0,
        Right = 1,
    }

    public enum HandFinger
    {
        Invalid = -1,
        Thumb = 0,
        Index = 1,
        Middle = 2,
        Ring = 3,
        Pinky = 4,
        Max = 5
    }

    [Flags]
    public enum HandFingerFlags
    {
        None = 0,
        Thumb = 1 << 0,
        Index = 1 << 1,
        Middle = 1 << 2,
        Ring = 1 << 3,
        Pinky = 1 << 4,
        All = (1 << 5) - 1
    }

    [Flags]
    public enum HandFingerJointFlags
    {
        None = 0,
        Thumb0 = 1 << HandJointId.HandThumb0,
        Thumb1 = 1 << HandJointId.HandThumb1,
        Thumb2 = 1 << HandJointId.HandThumb2,
        Thumb3 = 1 << HandJointId.HandThumb3,
        Index1 = 1 << HandJointId.HandIndex1,
        Index2 = 1 << HandJointId.HandIndex2,
        Index3 = 1 << HandJointId.HandIndex3,
        Middle1 = 1 << HandJointId.HandMiddle1,
        Middle2 = 1 << HandJointId.HandMiddle2,
        Middle3 = 1 << HandJointId.HandMiddle3,
        Ring1 = 1 << HandJointId.HandRing1,
        Ring2 = 1 << HandJointId.HandRing2,
        Ring3 = 1 << HandJointId.HandRing3,
        Pinky0 = 1 << HandJointId.HandPinky0,
        Pinky1 = 1 << HandJointId.HandPinky1,
        Pinky2 = 1 << HandJointId.HandPinky2,
        Pinky3 = 1 << HandJointId.HandPinky3,
    }

    public static class HandFingerUtils
    {
        public static HandFingerFlags ToFlags(HandFinger handFinger)
        {
            return (HandFingerFlags)(1 << (int)handFinger);
        }
    }

    public enum HandJointId
    {
        Invalid = -1,

        // hand bones
        HandStart = 0,
        HandWristRoot = HandStart + 0, // root frame of the hand, where the wrist is located
        HandForearmStub = HandStart + 1, // frame for user's forearm
        HandThumb0 = HandStart + 2, // thumb trapezium bone
        HandThumb1 = HandStart + 3, // thumb metacarpal bone
        HandThumb2 = HandStart + 4, // thumb proximal phalange bone
        HandThumb3 = HandStart + 5, // thumb distal phalange bone
        HandIndex1 = HandStart + 6, // index proximal phalange bone
        HandIndex2 = HandStart + 7, // index intermediate phalange bone
        HandIndex3 = HandStart + 8, // index distal phalange bone
        HandMiddle1 = HandStart + 9, // middle proximal phalange bone
        HandMiddle2 = HandStart + 10, // middle intermediate phalange bone
        HandMiddle3 = HandStart + 11, // middle distal phalange bone
        HandRing1 = HandStart + 12, // ring proximal phalange bone
        HandRing2 = HandStart + 13, // ring intermediate phalange bone
        HandRing3 = HandStart + 14, // ring distal phalange bone
        HandPinky0 = HandStart + 15, // pinky metacarpal bone
        HandPinky1 = HandStart + 16, // pinky proximal phalange bone
        HandPinky2 = HandStart + 17, // pinky intermediate phalange bone
        HandPinky3 = HandStart + 18, // pinky distal phalange bone
        HandMaxSkinnable = HandStart + 19,
        // Bone tips are position only.
        // They are not used for skinning but are useful for hit-testing.
        // NOTE: HandThumbTip == HandMaxSkinnable since the extended tips need to be contiguous
        HandThumbTip = HandMaxSkinnable + 0, // tip of the thumb
        HandIndexTip = HandMaxSkinnable + 1, // tip of the index finger
        HandMiddleTip = HandMaxSkinnable + 2, // tip of the middle finger
        HandRingTip = HandMaxSkinnable + 3, // tip of the ring finger
        HandPinkyTip = HandMaxSkinnable + 4, // tip of the pinky
        HandEnd = HandMaxSkinnable + 5,

        // body bones
        Body_Start = 0,
        Body_Root = Body_Start + 0,
        Body_Hips = Body_Start + 1,
        Body_SpineLower = Body_Start + 2,
        Body_SpineMiddle = Body_Start + 3,
        Body_SpineUpper = Body_Start + 4,
        Body_Chest = Body_Start + 5,
        Body_Neck = Body_Start + 6,
        Body_Head = Body_Start + 7,
        Body_LeftShoulder = Body_Start + 8,
        Body_LeftScapula = Body_Start + 9,
        Body_LeftArmUpper = Body_Start + 10,
        Body_LeftArmLower = Body_Start + 11,
        Body_LeftHandWristTwist = Body_Start + 12,
        Body_RightShoulder = Body_Start + 13,
        Body_RightScapula = Body_Start + 14,
        Body_RightArmUpper = Body_Start + 15,
        Body_RightArmLower = Body_Start + 16,
        Body_RightHandWristTwist = Body_Start + 17,
        Body_LeftHandPalm = Body_Start + 18,
        Body_LeftHandWrist = Body_Start + 19,
        Body_LeftHandThumbMetacarpal = Body_Start + 20,
        Body_LeftHandThumbProximal = Body_Start + 21,
        Body_LeftHandThumbDistal = Body_Start + 22,
        Body_LeftHandThumbTip = Body_Start + 23,
        Body_LeftHandIndexMetacarpal = Body_Start + 24,
        Body_LeftHandIndexProximal = Body_Start + 25,
        Body_LeftHandIndexIntermediate = Body_Start + 26,
        Body_LeftHandIndexDistal = Body_Start + 27,
        Body_LeftHandIndexTip = Body_Start + 28,
        Body_LeftHandMiddleMetacarpal = Body_Start + 29,
        Body_LeftHandMiddleProximal = Body_Start + 30,
        Body_LeftHandMiddleIntermediate = Body_Start + 31,
        Body_LeftHandMiddleDistal = Body_Start + 32,
        Body_LeftHandMiddleTip = Body_Start + 33,
        Body_LeftHandRingMetacarpal = Body_Start + 34,
        Body_LeftHandRingProximal = Body_Start + 35,
        Body_LeftHandRingIntermediate = Body_Start + 36,
        Body_LeftHandRingDistal = Body_Start + 37,
        Body_LeftHandRingTip = Body_Start + 38,
        Body_LeftHandLittleMetacarpal = Body_Start + 39,
        Body_LeftHandLittleProximal = Body_Start + 40,
        Body_LeftHandLittleIntermediate = Body_Start + 41,
        Body_LeftHandLittleDistal = Body_Start + 42,
        Body_LeftHandLittleTip = Body_Start + 43,
        Body_RightHandPalm = Body_Start + 44,
        Body_RightHandWrist = Body_Start + 45,
        Body_RightHandThumbMetacarpal = Body_Start + 46,
        Body_RightHandThumbProximal = Body_Start + 47,
        Body_RightHandThumbDistal = Body_Start + 48,
        Body_RightHandThumbTip = Body_Start + 49,
        Body_RightHandIndexMetacarpal = Body_Start + 50,
        Body_RightHandIndexProximal = Body_Start + 51,
        Body_RightHandIndexIntermediate = Body_Start + 52,
        Body_RightHandIndexDistal = Body_Start + 53,
        Body_RightHandIndexTip = Body_Start + 54,
        Body_RightHandMiddleMetacarpal = Body_Start + 55,
        Body_RightHandMiddleProximal = Body_Start + 56,
        Body_RightHandMiddleIntermediate = Body_Start + 57,
        Body_RightHandMiddleDistal = Body_Start + 58,
        Body_RightHandMiddleTip = Body_Start + 59,
        Body_RightHandRingMetacarpal = Body_Start + 60,
        Body_RightHandRingProximal = Body_Start + 61,
        Body_RightHandRingIntermediate = Body_Start + 62,
        Body_RightHandRingDistal = Body_Start + 63,
        Body_RightHandRingTip = Body_Start + 64,
        Body_RightHandLittleMetacarpal = Body_Start + 65,
        Body_RightHandLittleProximal = Body_Start + 66,
        Body_RightHandLittleIntermediate = Body_Start + 67,
        Body_RightHandLittleDistal = Body_Start + 68,
        Body_RightHandLittleTip = Body_Start + 69,
        Body_End = Body_Start + 70,

        // add new bones here
        Max = ((int)HandEnd > (int)Body_End) ? (int)HandEnd : (int)Body_End,
    }



    public class HandJointUtils
    {
        public static List<HandJointId[]> FingerToJointList = new List<HandJointId[]>()
        {
            new[] {HandJointId.HandThumb0,HandJointId.HandThumb1,HandJointId.HandThumb2,HandJointId.HandThumb3},
            new[] {HandJointId.HandIndex1, HandJointId.HandIndex2, HandJointId.HandIndex3},
            new[] {HandJointId.HandMiddle1, HandJointId.HandMiddle2, HandJointId.HandMiddle3},
            new[] {HandJointId.HandRing1,HandJointId.HandRing2,HandJointId.HandRing3},
            new[] {HandJointId.HandPinky0, HandJointId.HandPinky1, HandJointId.HandPinky2, HandJointId.HandPinky3}
        };

        public static HandJointId[] JointParentList = new[]
        {
            HandJointId.Invalid,
            HandJointId.HandStart,
            HandJointId.HandStart,
            HandJointId.HandThumb0,
            HandJointId.HandThumb1,
            HandJointId.HandThumb2,
            HandJointId.HandStart,
            HandJointId.HandIndex1,
            HandJointId.HandIndex2,
            HandJointId.HandStart,
            HandJointId.HandMiddle1,
            HandJointId.HandMiddle2,
            HandJointId.HandStart,
            HandJointId.HandRing1,
            HandJointId.HandRing2,
            HandJointId.HandStart,
            HandJointId.HandPinky0,
            HandJointId.HandPinky1,
            HandJointId.HandPinky2,
            HandJointId.HandThumb3,
            HandJointId.HandIndex3,
            HandJointId.HandMiddle3,
            HandJointId.HandRing3,
            HandJointId.HandPinky3
        };

        public static HandJointId[][] JointChildrenList = new[]
        {
            new []
            {
                HandJointId.HandThumb0,
                HandJointId.HandIndex1,
                HandJointId.HandMiddle1,
                HandJointId.HandRing1,
                HandJointId.HandPinky0
            },
            new HandJointId[0],
            new []{ HandJointId.HandThumb1 },
            new []{ HandJointId.HandThumb2 },
            new []{ HandJointId.HandThumb3 },
            new []{ HandJointId.HandThumbTip },
            new []{ HandJointId.HandIndex2 },
            new []{ HandJointId.HandIndex3 },
            new []{ HandJointId.HandIndexTip },
            new []{ HandJointId.HandMiddle2 },
            new []{ HandJointId.HandMiddle3 },
            new []{ HandJointId.HandMiddleTip },
            new []{ HandJointId.HandRing2 },
            new []{ HandJointId.HandRing3 },
            new []{ HandJointId.HandRingTip },
            new []{ HandJointId.HandPinky1 },
            new []{ HandJointId.HandPinky2 },
            new []{ HandJointId.HandPinky3 },
            new []{ HandJointId.HandPinkyTip },
            new HandJointId[0],
            new HandJointId[0],
            new HandJointId[0],
            new HandJointId[0],
            new HandJointId[0]
        };

        public static List<HandJointId> JointIds = new List<HandJointId>()
        {
            HandJointId.HandIndex1,
            HandJointId.HandIndex2,
            HandJointId.HandIndex3,
            HandJointId.HandMiddle1,
            HandJointId.HandMiddle2,
            HandJointId.HandMiddle3,
            HandJointId.HandRing1,
            HandJointId.HandRing2,
            HandJointId.HandRing3,
            HandJointId.HandPinky0,
            HandJointId.HandPinky1,
            HandJointId.HandPinky2,
            HandJointId.HandPinky3,
            HandJointId.HandThumb0,
            HandJointId.HandThumb1,
            HandJointId.HandThumb2,
            HandJointId.HandThumb3
        };

        private static readonly HandJointId[] _handFingerProximals =
        {
            HandJointId.HandThumb2, HandJointId.HandIndex1, HandJointId.HandMiddle1,
            HandJointId.HandRing1, HandJointId.HandPinky1
        };

        public static HandJointId GetHandFingerTip(HandFinger finger)
        {
            return HandJointId.HandMaxSkinnable + (int)finger;
        }

        /// <summary>
        /// Returns the "proximal" JointId for the given finger.
        /// This is commonly known as the Knuckle.
        /// For fingers, proximal is the join with index 1; eg HandIndex1.
        /// For thumb, proximal is the joint with index 2; eg HandThumb2.
        /// </summary>
        public static HandJointId GetHandFingerProximal(HandFinger finger)
        {
            return _handFingerProximals[(int)finger];
        }
    }

    public struct HandSkeletonJoint
    {
        /// <summary>
        /// Id of the parent joint in the skeleton hierarchy. Must always have a lower index than
        /// this joint.
        /// </summary>
        public int parent;

        /// <summary>
        /// Stores the pose of the joint, in local space.
        /// </summary>
        public Pose pose;
    }

    public interface IReadOnlyHandSkeletonJointList
    {
        ref readonly HandSkeletonJoint this[int jointId] { get; }
    }

    public interface IReadOnlyHandSkeleton
    {
        IReadOnlyHandSkeletonJointList Joints { get; }
    }

    public interface ICopyFrom<in TSelfType>
    {
        void CopyFrom(TSelfType source);
    }

    public class ReadOnlyHandJointPoses : IReadOnlyList<Pose>
    {
        private Pose[] _poses;

        public ReadOnlyHandJointPoses(Pose[] poses)
        {
            _poses = poses;
        }

        public IEnumerator<Pose> GetEnumerator()
        {
            foreach (var pose in _poses)
            {
                yield return pose;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static ReadOnlyHandJointPoses Empty { get; } = new ReadOnlyHandJointPoses(Array.Empty<Pose>());

        public int Count => _poses.Length;

        public Pose this[int index] => _poses[index];

        public ref readonly Pose this[HandJointId index] => ref _poses[(int)index];
    }

    public class HandSkeleton : IReadOnlyHandSkeleton, IReadOnlyHandSkeletonJointList
    {
        public HandSkeletonJoint[] joints = new HandSkeletonJoint[Constants.NUM_HAND_JOINTS];
        public IReadOnlyHandSkeletonJointList Joints => this;
        public ref readonly HandSkeletonJoint this[int jointId] => ref joints[jointId];


        public static readonly HandSkeleton DefaultLeftSkeleton = new HandSkeleton()
        {
            joints = new HandSkeletonJoint[]
            {
                new HandSkeletonJoint(){parent = -1, pose = new Pose(new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(0f,0f,0f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(-0.0200693f,0.0115541f,-0.01049652f), new Quaternion(-0.3753869f,0.4245841f,-0.007778856f,-0.8238644f))},
                new HandSkeletonJoint(){parent = 2,  pose = new Pose(new Vector3(-0.02485256f,-9.31E-10f,-1.863E-09f), new Quaternion(-0.2602303f,0.02433088f,0.125678f,-0.9570231f))},
                new HandSkeletonJoint(){parent = 3,  pose = new Pose(new Vector3(-0.03251291f,5.82E-10f,1.863E-09f), new Quaternion(0.08270377f,-0.0769617f,-0.08406223f,-0.9900357f))},
                new HandSkeletonJoint(){parent = 4,  pose = new Pose(new Vector3(-0.0337931f,3.26E-09f,1.863E-09f), new Quaternion(-0.08350593f,0.06501573f,-0.05827406f,-0.9926752f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(-0.09599624f,0.007316455f,-0.02355068f), new Quaternion(-0.03068309f,-0.01885559f,0.04328144f,-0.9984136f))},
                new HandSkeletonJoint(){parent = 6,  pose = new Pose(new Vector3(-0.0379273f,-5.82E-10f,-5.97E-10f), new Quaternion(0.02585241f,-0.007116061f,0.003292944f,-0.999635f))},
                new HandSkeletonJoint(){parent = 7,  pose = new Pose(new Vector3(-0.02430365f,-6.73E-10f,-6.75E-10f), new Quaternion(0.016056f,-0.02714872f,-0.072034f,-0.9969034f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(-0.09564661f,0.002543155f,-0.001725906f), new Quaternion(0.009066326f,-0.05146559f,0.05183575f,-0.9972874f))},
                new HandSkeletonJoint(){parent = 9,  pose = new Pose(new Vector3(-0.042927f,-8.51E-10f,-1.193E-09f), new Quaternion(0.01122823f,-0.004378874f,-0.001978267f,-0.9999254f))},
                new HandSkeletonJoint(){parent = 10, pose = new Pose(new Vector3(-0.02754958f,3.09E-10f,1.128E-09f), new Quaternion(0.03431955f,-0.004611839f,-0.09300701f,-0.9950631f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(-0.0886938f,0.006529308f,0.01746524f), new Quaternion(0.05315936f,-0.1231034f,0.04981349f,-0.9897162f))},
                new HandSkeletonJoint(){parent = 12, pose = new Pose(new Vector3(-0.0389961f,0f,5.24E-10f), new Quaternion(0.03363252f,-0.00278984f,0.00567602f,-0.9994143f))},
                new HandSkeletonJoint(){parent = 13, pose = new Pose(new Vector3(-0.02657339f,1.281E-09f,1.63E-09f), new Quaternion(0.003477462f,0.02917945f,-0.02502854f,-0.9992548f))},
                new HandSkeletonJoint(){parent = 0,  pose = new Pose(new Vector3(-0.03407356f,0.009419836f,0.02299858f), new Quaternion(0.207036f,-0.1403428f,0.0183118f,-0.9680417f))},
                new HandSkeletonJoint(){parent = 15, pose = new Pose(new Vector3(-0.04565055f,9.97679E-07f,-2.193963E-06f), new Quaternion(-0.09111304f,0.00407137f,0.02812923f,-0.9954349f))},
                new HandSkeletonJoint(){parent = 16, pose = new Pose(new Vector3(-0.03072042f,1.048E-09f,-1.75E-10f), new Quaternion(0.03761665f,-0.04293772f,-0.01328605f,-0.9982809f))},
                new HandSkeletonJoint(){parent = 17, pose = new Pose(new Vector3(-0.02031138f,-2.91E-10f,9.31E-10f), new Quaternion(-0.0006447434f,0.04917067f,-0.02401883f,-0.9985014f))},
                new HandSkeletonJoint(){parent = 5,  pose = new Pose(new Vector3(-0.02459077f,-0.001026974f,0.0006703701f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 8,  pose = new Pose(new Vector3(-0.02236338f,-0.00102507f,0.0002956076f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 11, pose = new Pose(new Vector3(-0.02496492f,-0.001137299f,0.0003086528f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 14, pose = new Pose(new Vector3(-0.02432613f,-0.001608172f,0.000257905f), new Quaternion(0f,0f,0f,-1f))},
                new HandSkeletonJoint(){parent = 18, pose = new Pose(new Vector3(-0.02192238f,-0.001216086f,-0.0002464796f), new Quaternion(0f,0f,0f,-1f)) }
            }
        };

        public static readonly HandSkeleton DefaultRightSkeleton = new HandSkeleton()
        {
            joints = DefaultLeftSkeleton.joints.Select(joint => new HandSkeletonJoint()
            {
                parent = joint.parent,
                pose = new Pose(-joint.pose.position, joint.pose.rotation)
            }).ToArray()
        };
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector2f
    {
        public float x;
        public float y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector3f
    {
        public float x;
        public float y;
        public float z;
        public static readonly Vector3f zero = new Vector3f { x = 0.0f, y = 0.0f, z = 0.0f };
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}", x, y, z);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4f
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public static readonly Vector4f zero = new Vector4f { x = 0.0f, y = 0.0f, z = 0.0f, w = 0.0f };
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Vector4s
    {
        public short x;
        public short y;
        public short z;
        public short w;
        public static readonly Vector4s zero = new Vector4s { x = 0, y = 0, z = 0, w = 0 };
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Quatf
    {
        public float x;
        public float y;
        public float z;
        public float w;
        public static readonly Quatf identity = new Quatf { x = 0.0f, y = 0.0f, z = 0.0f, w = 1.0f };
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}", x, y, z, w);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Posef
    {
        public Quatf Orientation;
        public Vector3f Position;
        public static readonly Posef identity = new Posef { Orientation = Quatf.identity, Position = Vector3f.zero };

        public Posef(Vector3f position,Quatf orientation)
        {
            Position = position;
            Orientation = orientation;
        
        }
        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Position ({0}), Orientation({1})", Position, Orientation);
        }
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct BoneCapsule
    {
        public short BoneIndex;
        public Vector3f StartPoint;
        public Vector3f EndPoint;
        public float Radius;
    }

   
    [StructLayout(LayoutKind.Sequential)]
    public struct Bone
    {
        public HandJointId Id;
        public short ParentBoneIndex;
        public Posef Pose;
    }
    public enum SkeletonType
    {
        None = -1,
        HandLeft = 0,
        HandRight = 1,
        Body = 2,
    }

    public enum SkeletonConstants
    {
        MaxHandBones = HandJointId.HandEnd,
        MaxBodyBones = HandJointId.Body_End,
        MaxBones = HandJointId.Max,
        MaxBoneCapsules = 19,
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Skeleton
    {
        public SkeletonType Type;
        public uint NumBones;
        public uint NumBoneCapsules;
        public Bone[] Bones;
        public BoneCapsule[] BoneCapsules;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct SkeletonInternal
    {
        public SkeletonType Type;
        public uint NumBones;
        public uint NumBoneCapsules;
        public Bone Bones_0;
        public Bone Bones_1;
        public Bone Bones_2;
        public Bone Bones_3;
        public Bone Bones_4;
        public Bone Bones_5;
        public Bone Bones_6;
        public Bone Bones_7;
        public Bone Bones_8;
        public Bone Bones_9;
        public Bone Bones_10;
        public Bone Bones_11;
        public Bone Bones_12;
        public Bone Bones_13;
        public Bone Bones_14;
        public Bone Bones_15;
        public Bone Bones_16;
        public Bone Bones_17;
        public Bone Bones_18;
        public Bone Bones_19;
        public Bone Bones_20;
        public Bone Bones_21;
        public Bone Bones_22;
        public Bone Bones_23;
        public Bone Bones_24;
        public Bone Bones_25;
        public Bone Bones_26;
        public Bone Bones_27;
        public Bone Bones_28;
        public Bone Bones_29;
        public Bone Bones_30;
        public Bone Bones_31;
        public Bone Bones_32;
        public Bone Bones_33;
        public Bone Bones_34;
        public Bone Bones_35;
        public Bone Bones_36;
        public Bone Bones_37;
        public Bone Bones_38;
        public Bone Bones_39;
        public Bone Bones_40;
        public Bone Bones_41;
        public Bone Bones_42;
        public Bone Bones_43;
        public Bone Bones_44;
        public Bone Bones_45;
        public Bone Bones_46;
        public Bone Bones_47;
        public Bone Bones_48;
        public Bone Bones_49;
        public Bone Bones_50;
        public Bone Bones_51;
        public Bone Bones_52;
        public Bone Bones_53;
        public Bone Bones_54;
        public Bone Bones_55;
        public Bone Bones_56;
        public Bone Bones_57;
        public Bone Bones_58;
        public Bone Bones_59;
        public Bone Bones_60;
        public Bone Bones_61;
        public Bone Bones_62;
        public Bone Bones_63;
        public Bone Bones_64;
        public Bone Bones_65;
        public Bone Bones_66;
        public Bone Bones_67;
        public Bone Bones_68;
        public Bone Bones_69;
        public BoneCapsule BoneCapsules_0;
        public BoneCapsule BoneCapsules_1;
        public BoneCapsule BoneCapsules_2;
        public BoneCapsule BoneCapsules_3;
        public BoneCapsule BoneCapsules_4;
        public BoneCapsule BoneCapsules_5;
        public BoneCapsule BoneCapsules_6;
        public BoneCapsule BoneCapsules_7;
        public BoneCapsule BoneCapsules_8;
        public BoneCapsule BoneCapsules_9;
        public BoneCapsule BoneCapsules_10;
        public BoneCapsule BoneCapsules_11;
        public BoneCapsule BoneCapsules_12;
        public BoneCapsule BoneCapsules_13;
        public BoneCapsule BoneCapsules_14;
        public BoneCapsule BoneCapsules_15;
        public BoneCapsule BoneCapsules_16;
        public BoneCapsule BoneCapsules_17;
        public BoneCapsule BoneCapsules_18;
    }

}
