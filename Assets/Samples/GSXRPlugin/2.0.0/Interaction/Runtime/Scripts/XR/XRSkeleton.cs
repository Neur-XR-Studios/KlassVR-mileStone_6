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
using UnityEngine;
namespace XR.Interaction.Input
{
    [DefaultExecutionOrder(-80)]
    public class XRSkeleton : MonoBehaviour
    {
        public interface ISkeletonDataProvider
        {
            SkeletonType GetSkeletonType();
            SkeletonPoseData GetSkeletonPoseData();
            bool enabled { get; }
        }

        public struct SkeletonPoseData
        {
            public Posef RootPose { get; set; }
            public float RootScale { get; set; }
            public  Quatf[] BoneRotations { get; set; }
            public bool IsDataValid { get; set; }
            public bool IsDataHighConfidence { get; set; }
            public  Vector3f[] BoneTranslations { get; set; }
            public int SkeletonChangedCount { get; set; }
        }

 
        [SerializeField]
        protected SkeletonType _skeletonType = SkeletonType.None;
        [SerializeField]
        private ISkeletonDataProvider _dataProvider;

        [SerializeField]
        private bool _updateRootPose = false;
        [SerializeField]
        private bool _updateRootScale = false;
        [SerializeField]
        private bool _enablePhysicsCapsules = false;
        [SerializeField]
        private bool _applyBoneTranslations = true;

        private GameObject _bonesGO;
        private GameObject _bindPosesGO;
        private GameObject _capsulesGO;

        protected List<XRBone> _bones;
        private List<XRBone> _bindPoses;
        private List<XRBoneCapsule> _capsules;

        protected Skeleton _skeleton = new Skeleton();
        private readonly Quaternion wristFixupRotation = new Quaternion(0.0f, 1.0f, 0.0f, 0.0f);

        public bool IsInitialized { get; private set; }
        public bool IsDataValid { get; private set; }
        public bool IsDataHighConfidence { get; private set; }
        public IList<XRBone> Bones { get; protected set; }
        public IList<XRBone> BindPoses { get; private set; }
        public IList<XRBoneCapsule> Capsules { get; private set; }
        public SkeletonType GetSkeletonType() { return _skeletonType; }
        public int SkeletonChangedCount { get; private set; }

        private void Awake()
        {
            if (_dataProvider == null)
            {
                _dataProvider = GetComponent<ISkeletonDataProvider>();
            }

            _bones = new List<XRBone>();
            Bones = _bones.AsReadOnly();

            _bindPoses = new List<XRBone>();
            BindPoses = _bindPoses.AsReadOnly();

            _capsules = new List<XRBoneCapsule>();
            Capsules = _capsules.AsReadOnly();
        }

        private void Start()
        {
            if (ShouldInitialize())
            {
                Initialize();
            }
        }

        private bool ShouldInitialize()
        {
            if (IsInitialized)
            {
                return false;
            }

            if (_dataProvider != null && !_dataProvider.enabled)
            {
                return false;
            }

            if (_skeletonType == SkeletonType.None)
            {
                return false;
            }
            else if (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight)
            {
#if UNITY_EDITOR
                return XRInput.IsControllerConnected(XRInput.Controller.Hands);
#else
            return true;
#endif
            }
            else
            {
                return true;
            }
        }
        private static SkeletonInternal cachedSkeleton2 = new SkeletonInternal();
        public static bool GetSkeleton(SkeletonType skeletonType, ref Skeleton skeleton)
        {
            if (skeleton.Bones == null || skeleton.Bones.Length != (int)SkeletonConstants.MaxBones)
            {
                skeleton.Bones = new Bone[(int)SkeletonConstants.MaxBones];
            }
            if (skeleton.BoneCapsules == null || skeleton.BoneCapsules.Length != (int)SkeletonConstants.MaxBoneCapsules)
            {
                skeleton.BoneCapsules = new BoneCapsule[(int)SkeletonConstants.MaxBoneCapsules];
            }
            if (skeletonType == SkeletonType.HandLeft)
            {
                skeleton = SkeletonData.LeftSkeleton;
             }
            else if (skeletonType == SkeletonType.HandRight)
            {
                skeleton = SkeletonData.RightSkeleton;
            }

            //skeleton.Type = cachedSkeleton2.Type;
            //skeleton.NumBones = cachedSkeleton2.NumBones;
            //skeleton.NumBoneCapsules = cachedSkeleton2.NumBoneCapsules;
            //skeleton.Bones[0] = cachedSkeleton2.Bones_0;
            //skeleton.Bones[1] = cachedSkeleton2.Bones_1;
            //skeleton.Bones[2] = cachedSkeleton2.Bones_2;
            //skeleton.Bones[3] = cachedSkeleton2.Bones_3;
            //skeleton.Bones[4] = cachedSkeleton2.Bones_4;
            //skeleton.Bones[5] = cachedSkeleton2.Bones_5;
            //skeleton.Bones[6] = cachedSkeleton2.Bones_6;
            //skeleton.Bones[7] = cachedSkeleton2.Bones_7;
            //skeleton.Bones[8] = cachedSkeleton2.Bones_8;
            //skeleton.Bones[9] = cachedSkeleton2.Bones_9;
            //skeleton.Bones[10] = cachedSkeleton2.Bones_10;
            //skeleton.Bones[11] = cachedSkeleton2.Bones_11;
            //skeleton.Bones[12] = cachedSkeleton2.Bones_12;
            //skeleton.Bones[13] = cachedSkeleton2.Bones_13;
            //skeleton.Bones[14] = cachedSkeleton2.Bones_14;
            //skeleton.Bones[15] = cachedSkeleton2.Bones_15;
            //skeleton.Bones[16] = cachedSkeleton2.Bones_16;
            //skeleton.Bones[17] = cachedSkeleton2.Bones_17;
            //skeleton.Bones[18] = cachedSkeleton2.Bones_18;
            //skeleton.Bones[19] = cachedSkeleton2.Bones_19;
            //skeleton.Bones[20] = cachedSkeleton2.Bones_20;
            //skeleton.Bones[21] = cachedSkeleton2.Bones_21;
            //skeleton.Bones[22] = cachedSkeleton2.Bones_22;
            //skeleton.Bones[23] = cachedSkeleton2.Bones_23;
            //skeleton.Bones[24] = cachedSkeleton2.Bones_24;
            //skeleton.Bones[25] = cachedSkeleton2.Bones_25;
            //skeleton.Bones[26] = cachedSkeleton2.Bones_26;
            //skeleton.Bones[27] = cachedSkeleton2.Bones_27;
            //skeleton.Bones[28] = cachedSkeleton2.Bones_28;
            //skeleton.Bones[29] = cachedSkeleton2.Bones_29;
            //skeleton.Bones[30] = cachedSkeleton2.Bones_30;
            //skeleton.Bones[31] = cachedSkeleton2.Bones_31;
            //skeleton.Bones[32] = cachedSkeleton2.Bones_32;
            //skeleton.Bones[33] = cachedSkeleton2.Bones_33;
            //skeleton.Bones[34] = cachedSkeleton2.Bones_34;
            //skeleton.Bones[35] = cachedSkeleton2.Bones_35;
            //skeleton.Bones[36] = cachedSkeleton2.Bones_36;
            //skeleton.Bones[37] = cachedSkeleton2.Bones_37;
            //skeleton.Bones[38] = cachedSkeleton2.Bones_38;
            //skeleton.Bones[39] = cachedSkeleton2.Bones_39;
            //skeleton.Bones[40] = cachedSkeleton2.Bones_40;
            //skeleton.Bones[41] = cachedSkeleton2.Bones_41;
            //skeleton.Bones[42] = cachedSkeleton2.Bones_42;
            //skeleton.Bones[43] = cachedSkeleton2.Bones_43;
            //skeleton.Bones[44] = cachedSkeleton2.Bones_44;
            //skeleton.Bones[45] = cachedSkeleton2.Bones_45;
            //skeleton.Bones[46] = cachedSkeleton2.Bones_46;
            //skeleton.Bones[47] = cachedSkeleton2.Bones_47;
            //skeleton.Bones[48] = cachedSkeleton2.Bones_48;
            //skeleton.Bones[49] = cachedSkeleton2.Bones_49;
            //skeleton.Bones[50] = cachedSkeleton2.Bones_50;
            //skeleton.Bones[51] = cachedSkeleton2.Bones_51;
            //skeleton.Bones[52] = cachedSkeleton2.Bones_52;
            //skeleton.Bones[53] = cachedSkeleton2.Bones_53;
            //skeleton.Bones[54] = cachedSkeleton2.Bones_54;
            //skeleton.Bones[55] = cachedSkeleton2.Bones_55;
            //skeleton.Bones[56] = cachedSkeleton2.Bones_56;
            //skeleton.Bones[57] = cachedSkeleton2.Bones_57;
            //skeleton.Bones[58] = cachedSkeleton2.Bones_58;
            //skeleton.Bones[59] = cachedSkeleton2.Bones_59;
            //skeleton.Bones[60] = cachedSkeleton2.Bones_60;
            //skeleton.Bones[61] = cachedSkeleton2.Bones_61;
            //skeleton.Bones[62] = cachedSkeleton2.Bones_62;
            //skeleton.Bones[63] = cachedSkeleton2.Bones_63;
            //skeleton.Bones[64] = cachedSkeleton2.Bones_64;
            //skeleton.Bones[65] = cachedSkeleton2.Bones_65;
            //skeleton.Bones[66] = cachedSkeleton2.Bones_66;
            //skeleton.Bones[67] = cachedSkeleton2.Bones_67;
            //skeleton.Bones[68] = cachedSkeleton2.Bones_68;
            //skeleton.Bones[69] = cachedSkeleton2.Bones_69;
            //skeleton.BoneCapsules[0] = cachedSkeleton2.BoneCapsules_0;
            //skeleton.BoneCapsules[1] = cachedSkeleton2.BoneCapsules_1;
            //skeleton.BoneCapsules[2] = cachedSkeleton2.BoneCapsules_2;
            //skeleton.BoneCapsules[3] = cachedSkeleton2.BoneCapsules_3;
            //skeleton.BoneCapsules[4] = cachedSkeleton2.BoneCapsules_4;
            //skeleton.BoneCapsules[5] = cachedSkeleton2.BoneCapsules_5;
            //skeleton.BoneCapsules[6] = cachedSkeleton2.BoneCapsules_6;
            //skeleton.BoneCapsules[7] = cachedSkeleton2.BoneCapsules_7;
            //skeleton.BoneCapsules[8] = cachedSkeleton2.BoneCapsules_8;
            //skeleton.BoneCapsules[9] = cachedSkeleton2.BoneCapsules_9;
            //skeleton.BoneCapsules[10] = cachedSkeleton2.BoneCapsules_10;
            //skeleton.BoneCapsules[11] = cachedSkeleton2.BoneCapsules_11;
            //skeleton.BoneCapsules[12] = cachedSkeleton2.BoneCapsules_12;
            //skeleton.BoneCapsules[13] = cachedSkeleton2.BoneCapsules_13;
            //skeleton.BoneCapsules[14] = cachedSkeleton2.BoneCapsules_14;
            //skeleton.BoneCapsules[15] = cachedSkeleton2.BoneCapsules_15;
            //skeleton.BoneCapsules[16] = cachedSkeleton2.BoneCapsules_16;
            //skeleton.BoneCapsules[17] = cachedSkeleton2.BoneCapsules_17;
            //skeleton.BoneCapsules[18] = cachedSkeleton2.BoneCapsules_18;

            return true;

        }

        private void Initialize()
        {
            if (GetSkeleton(_skeletonType, ref _skeleton))
            {
                InitializeBones();
                InitializeBindPose();
                InitializeCapsules();

                IsInitialized = true;
            }
        }

        protected virtual Transform GetBoneTransform(HandJointId boneId) => null;

        protected virtual void InitializeBones()
        {
            bool flipX = (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight);

            if (!_bonesGO)
            {
                _bonesGO = new GameObject("Bones");
                _bonesGO.transform.SetParent(transform, false);
                _bonesGO.transform.localPosition = Vector3.zero;
                _bonesGO.transform.localRotation = Quaternion.identity;
            }

            if (_bones == null || _bones.Count != _skeleton.NumBones)
            {
                _bones = new List<XRBone>(new XRBone[_skeleton.NumBones]);
                Bones = _bones.AsReadOnly();
            }

            // pre-populate bones list before attempting to apply bone hierarchy
            for (int i = 0; i < _bones.Count; ++i)
            {
                XRBone bone = _bones[i] ?? (_bones[i] = new XRBone());
                bone.Id = _skeleton.Bones[i].Id;
                bone.ParentBoneIndex = _skeleton.Bones[i].ParentBoneIndex;

                bone.Transform = GetBoneTransform(bone.Id);
                if (bone.Transform == null)
                {
                    bone.Transform = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id)).transform;
                }

                var pose = _skeleton.Bones[i].Pose;

                if (_applyBoneTranslations)
                {
                    bone.Transform.localPosition = flipX
                        ? pose.Position.FromFlippedXVector3f()
                        : pose.Position.FromFlippedZVector3f();
                }

                bone.Transform.localRotation = flipX
                    ? pose.Orientation.FromFlippedXQuatf()
                    : pose.Orientation.FromFlippedZQuatf();
            }

            for (int i = 0; i < _bones.Count; ++i)
            {
                if ((HandJointId)_bones[i].ParentBoneIndex == HandJointId.Invalid ||
                    _skeletonType == SkeletonType.Body)  // Body bones are always in tracking space
                {
                    _bones[i].Transform.SetParent(_bonesGO.transform, false);
                }
                else
                {
                    _bones[i].Transform.SetParent(_bones[_bones[i].ParentBoneIndex].Transform, false);
                }
            }
        }

        private void InitializeBindPose()
        {
            if (!_bindPosesGO)
            {
                _bindPosesGO = new GameObject("BindPoses");
                _bindPosesGO.transform.SetParent(transform, false);
                _bindPosesGO.transform.localPosition = Vector3.zero;
                _bindPosesGO.transform.localRotation = Quaternion.identity;
            }

            if (_bindPoses == null || _bindPoses.Count != _bones.Count)
            {
                _bindPoses = new List<XRBone>(new XRBone[_bones.Count]);
                BindPoses = _bindPoses.AsReadOnly();
            }

            // pre-populate bones list before attempting to apply bone hierarchy
            for (int i = 0; i < _bindPoses.Count; ++i)
            {
                XRBone bone = _bones[i];
                XRBone bindPoseBone = _bindPoses[i] ?? (_bindPoses[i] = new XRBone());
                bindPoseBone.Id = bone.Id;
                bindPoseBone.ParentBoneIndex = bone.ParentBoneIndex;

                Transform trans = bindPoseBone.Transform ? bindPoseBone.Transform : (bindPoseBone.Transform =
                    new GameObject(BoneLabelFromBoneId(_skeletonType, bindPoseBone.Id)).transform);
                trans.localPosition = bone.Transform.localPosition;
                trans.localRotation = bone.Transform.localRotation;
            }

            for (int i = 0; i < _bindPoses.Count; ++i)
            {
                if ((HandJointId)_bindPoses[i].ParentBoneIndex == HandJointId.Invalid ||
                    _skeletonType == SkeletonType.Body) // Body bones are always in tracking space
                {
                    _bindPoses[i].Transform.SetParent(_bindPosesGO.transform, false);
                }
                else
                {
                    _bindPoses[i].Transform.SetParent(_bindPoses[_bindPoses[i].ParentBoneIndex].Transform, false);
                }
            }
        }

        private void InitializeCapsules()
        {
            bool flipX = (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight);

            if (_enablePhysicsCapsules)
            {
                if (!_capsulesGO)
                {
                    _capsulesGO = new GameObject("Capsules");
                    _capsulesGO.transform.SetParent(transform, false);
                    _capsulesGO.transform.localPosition = Vector3.zero;
                    _capsulesGO.transform.localRotation = Quaternion.identity;
                }

                if (_capsules == null || _capsules.Count != _skeleton.NumBoneCapsules)
                {
                    _capsules = new List<XRBoneCapsule>(new XRBoneCapsule[_skeleton.NumBoneCapsules]);
                    Capsules = _capsules.AsReadOnly();
                }

                for (int i = 0; i < _capsules.Count; ++i)
                {
                     XRBone bone = _bones[_skeleton.BoneCapsules[i].BoneIndex];
                    XRBoneCapsule capsule = _capsules[i] ?? (_capsules[i] = new XRBoneCapsule());
                    capsule.BoneIndex = _skeleton.BoneCapsules[i].BoneIndex;

                    if (capsule.CapsuleRigidbody == null)
                    {
                        capsule.CapsuleRigidbody = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleRigidbody").AddComponent<Rigidbody>();
                        capsule.CapsuleRigidbody.mass = 1.0f;
                        capsule.CapsuleRigidbody.isKinematic = true;
                        capsule.CapsuleRigidbody.useGravity = false;
                        capsule.CapsuleRigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                    }

                    GameObject rbGO = capsule.CapsuleRigidbody.gameObject;
                    rbGO.transform.SetParent(_capsulesGO.transform, false);
                    rbGO.transform.position = bone.Transform.position;
                    rbGO.transform.rotation = bone.Transform.rotation;

                    if (capsule.CapsuleCollider == null)
                    {
                        capsule.CapsuleCollider = new GameObject(BoneLabelFromBoneId(_skeletonType, bone.Id) + "_CapsuleCollider").AddComponent<CapsuleCollider>();
                        capsule.CapsuleCollider.isTrigger = false;
                    }

                    var p0 = flipX ? _skeleton.BoneCapsules[i].StartPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[i].StartPoint.FromFlippedZVector3f();
                    var p1 = flipX ? _skeleton.BoneCapsules[i].EndPoint.FromFlippedXVector3f() : _skeleton.BoneCapsules[i].EndPoint.FromFlippedZVector3f();
                    var delta = p1 - p0;
                    var mag = delta.magnitude;
                    var rot = Quaternion.FromToRotation(Vector3.right, delta);
                    capsule.CapsuleCollider.radius = _skeleton.BoneCapsules[i].Radius;
                    capsule.CapsuleCollider.height = mag + _skeleton.BoneCapsules[i].Radius * 2.0f;
                    capsule.CapsuleCollider.direction = 0;
                    capsule.CapsuleCollider.center = Vector3.right * mag * 0.5f;

                    GameObject ccGO = capsule.CapsuleCollider.gameObject;
                    ccGO.transform.SetParent(rbGO.transform, false);
                    ccGO.transform.localPosition = p0;
                    ccGO.transform.localRotation = rot;
                }
            }
        }

        private void Update()
        {
            if (ShouldInitialize())
            {
                Initialize();
            }

            if (!IsInitialized || _dataProvider == null)
            {
                IsDataValid = false;
                IsDataHighConfidence = false;
                return;
            }

            var data = _dataProvider.GetSkeletonPoseData();

            IsDataValid = data.IsDataValid;
            if (data.IsDataValid)
            {

                if (SkeletonChangedCount != data.SkeletonChangedCount)
                {
                    SkeletonChangedCount = data.SkeletonChangedCount;
                    IsInitialized = false;
                    Initialize();
                }

                IsDataHighConfidence = data.IsDataHighConfidence;

                if (_updateRootPose)
                {
                    transform.localPosition = data.RootPose.Position.FromFlippedZVector3f();
                    transform.localRotation = data.RootPose.Orientation.FromFlippedZQuatf();
                }

                if (_updateRootScale)
                {
                    transform.localScale = new Vector3(data.RootScale, data.RootScale, data.RootScale);
                }

                for (var i = 0; i < _bones.Count; ++i)
                {
                    var boneTransform = _bones[i].Transform;
                    if (boneTransform == null) continue;

                    if (_skeletonType == SkeletonType.Body)
                    {
                        boneTransform.localPosition = data.BoneTranslations[i].FromFlippedZVector3f();
                        boneTransform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();
                    }
                    else if (_skeletonType == SkeletonType.HandLeft || _skeletonType == SkeletonType.HandRight)
                    {
                        boneTransform.localRotation = data.BoneRotations[i].FromFlippedXQuatf();

                        if (_bones[i].Id == HandJointId.HandWristRoot)
                        {
                            boneTransform.localRotation *= wristFixupRotation;
                        }
                    }
                    else
                    {
                        boneTransform.localRotation = data.BoneRotations[i].FromFlippedZQuatf();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (!IsInitialized || _dataProvider == null)
            {
                IsDataValid = false;
                IsDataHighConfidence = false;

                return;
            }

            Update();

            if (_enablePhysicsCapsules)
            {
                var data = _dataProvider.GetSkeletonPoseData();

                IsDataValid = data.IsDataValid;
                IsDataHighConfidence = data.IsDataHighConfidence;

                for (int i = 0; i < _capsules.Count; ++i)
                {
                    XRBoneCapsule capsule = _capsules[i];
                    var capsuleGO = capsule.CapsuleRigidbody.gameObject;

                    if (data.IsDataValid && data.IsDataHighConfidence)
                    {
                        Transform bone = _bones[(int)capsule.BoneIndex].Transform;

                        if (capsuleGO.activeSelf)
                        {
                            capsule.CapsuleRigidbody.MovePosition(bone.position);
                            capsule.CapsuleRigidbody.MoveRotation(bone.rotation);
                        }
                        else
                        {
                            capsuleGO.SetActive(true);
                            capsule.CapsuleRigidbody.position = bone.position;
                            capsule.CapsuleRigidbody.rotation = bone.rotation;
                        }
                    }
                    else
                    {
                        if (capsuleGO.activeSelf)
                        {
                            capsuleGO.SetActive(false);
                        }
                    }
                }
            }
        }

        public HandJointId GetCurrentStartBoneId()
        {
            switch (_skeletonType)
            {
                case SkeletonType.HandLeft:
                case SkeletonType.HandRight:
                    return HandJointId.HandStart;
                case SkeletonType.Body:
                    return HandJointId.HandStart;
                case SkeletonType.None:
                default:
                    return HandJointId.Invalid;
            }
        }

        public HandJointId GetCurrentEndBoneId()
        {
            switch (_skeletonType)
            {
                case SkeletonType.HandLeft:
                case SkeletonType.HandRight:
                    return HandJointId.HandEnd;
                case SkeletonType.Body:
                    return HandJointId.HandEnd;
                case SkeletonType.None:
                default:
                    return HandJointId.Invalid;
            }
        }

        private HandJointId GetCurrentMaxSkinnableBoneId()
        {
            switch (_skeletonType)
            {
                case SkeletonType.HandLeft:
                case SkeletonType.HandRight:
                    return HandJointId.HandMaxSkinnable;
                case SkeletonType.Body:
                    return HandJointId.HandEnd;
                case SkeletonType.None:
                default:
                    return HandJointId.Invalid;
            }
        }

        public int GetCurrentNumBones()
        {
            switch (_skeletonType)
            {
                case SkeletonType.HandLeft:
                case SkeletonType.HandRight:
                case SkeletonType.Body:
                    return GetCurrentEndBoneId() - GetCurrentStartBoneId();
                case SkeletonType.None:
                default:
                    return 0;
            }
        }

        public int GetCurrentNumSkinnableBones()
        {
            switch (_skeletonType)
            {
                case SkeletonType.HandLeft:
                case SkeletonType.HandRight:
                case SkeletonType.Body:
                    return GetCurrentMaxSkinnableBoneId() - GetCurrentStartBoneId();
                case SkeletonType.None:
                default:
                    return 0;
            }
        }


        // force aliased enum values to the more appropriate value
        public static string BoneLabelFromBoneId(SkeletonType skeletonType, HandJointId boneId)
        {
            if (skeletonType ==  SkeletonType.Body)
            {
                switch (boneId)
                {
                    case HandJointId.Body_Root:
                        return "Body_Root";
                    case HandJointId.Body_Hips:
                        return "Body_Hips";
                    case HandJointId.Body_SpineLower:
                        return "Body_SpineLower";
                    case HandJointId.Body_SpineMiddle:
                        return "Body_SpineMiddle";
                    case HandJointId.Body_SpineUpper:
                        return "Body_SpineUpper";
                    case HandJointId.Body_Chest:
                        return "Body_Chest";
                    case HandJointId.Body_Neck:
                        return "Body_Neck";
                    case HandJointId.Body_Head:
                        return "Body_Head";
                    case HandJointId.Body_LeftShoulder:
                        return "Body_LeftShoulder";
                    case HandJointId.Body_LeftScapula:
                        return "Body_LeftScapula";
                    case HandJointId.Body_LeftArmUpper:
                        return "Body_LeftArmUpper";
                    case HandJointId.Body_LeftArmLower:
                        return "Body_LeftArmLower";
                    case HandJointId.Body_LeftHandWristTwist:
                        return "Body_LeftHandWristTwist";
                    case HandJointId.Body_RightShoulder:
                        return "Body_RightShoulder";
                    case HandJointId.Body_RightScapula:
                        return "Body_RightScapula";
                    case HandJointId.Body_RightArmUpper:
                        return "Body_RightArmUpper";
                    case HandJointId.Body_RightArmLower:
                        return "Body_RightArmLower";
                    case HandJointId.Body_RightHandWristTwist:
                        return "Body_RightHandWristTwist";
                    case HandJointId.Body_LeftHandPalm:
                        return "Body_LeftHandPalm";
                    case HandJointId.Body_LeftHandWrist:
                        return "Body_LeftHandWrist";
                    case HandJointId.Body_LeftHandThumbMetacarpal:
                        return "Body_LeftHandThumbMetacarpal";
                    case HandJointId.Body_LeftHandThumbProximal:
                        return "Body_LeftHandThumbProximal";
                    case HandJointId.Body_LeftHandThumbDistal:
                        return "Body_LeftHandThumbDistal";
                    case HandJointId.Body_LeftHandThumbTip:
                        return "Body_LeftHandThumbTip";
                    case HandJointId.Body_LeftHandIndexMetacarpal:
                        return "Body_LeftHandIndexMetacarpal";
                    case HandJointId.Body_LeftHandIndexProximal:
                        return "Body_LeftHandIndexProximal";
                    case HandJointId.Body_LeftHandIndexIntermediate:
                        return "Body_LeftHandIndexIntermediate";
                    case HandJointId.Body_LeftHandIndexDistal:
                        return "Body_LeftHandIndexDistal";
                    case HandJointId.Body_LeftHandIndexTip:
                        return "Body_LeftHandIndexTip";
                    case HandJointId.Body_LeftHandMiddleMetacarpal:
                        return "Body_LeftHandMiddleMetacarpal";
                    case HandJointId.Body_LeftHandMiddleProximal:
                        return "Body_LeftHandMiddleProximal";
                    case HandJointId.Body_LeftHandMiddleIntermediate:
                        return "Body_LeftHandMiddleIntermediate";
                    case HandJointId.Body_LeftHandMiddleDistal:
                        return "Body_LeftHandMiddleDistal";
                    case HandJointId.Body_LeftHandMiddleTip:
                        return "Body_LeftHandMiddleTip";
                    case HandJointId.Body_LeftHandRingMetacarpal:
                        return "Body_LeftHandRingMetacarpal";
                    case HandJointId.Body_LeftHandRingProximal:
                        return "Body_LeftHandRingProximal";
                    case HandJointId.Body_LeftHandRingIntermediate:
                        return "Body_LeftHandRingIntermediate";
                    case HandJointId.Body_LeftHandRingDistal:
                        return "Body_LeftHandRingDistal";
                    case HandJointId.Body_LeftHandRingTip:
                        return "Body_LeftHandRingTip";
                    case HandJointId.Body_LeftHandLittleMetacarpal:
                        return "Body_LeftHandLittleMetacarpal";
                    case HandJointId.Body_LeftHandLittleProximal:
                        return "Body_LeftHandLittleProximal";
                    case HandJointId.Body_LeftHandLittleIntermediate:
                        return "Body_LeftHandLittleIntermediate";
                    case HandJointId.Body_LeftHandLittleDistal:
                        return "Body_LeftHandLittleDistal";
                    case HandJointId.Body_LeftHandLittleTip:
                        return "Body_LeftHandLittleTip";
                    case HandJointId.Body_RightHandPalm:
                        return "Body_RightHandPalm";
                    case HandJointId.Body_RightHandWrist:
                        return "Body_RightHandWrist";
                    case HandJointId.Body_RightHandThumbMetacarpal:
                        return "Body_RightHandThumbMetacarpal";
                    case HandJointId.Body_RightHandThumbProximal:
                        return "Body_RightHandThumbProximal";
                    case HandJointId.Body_RightHandThumbDistal:
                        return "Body_RightHandThumbDistal";
                    case HandJointId.Body_RightHandThumbTip:
                        return "Body_RightHandThumbTip";
                    case HandJointId.Body_RightHandIndexMetacarpal:
                        return "Body_RightHandIndexMetacarpal";
                    case HandJointId.Body_RightHandIndexProximal:
                        return "Body_RightHandIndexProximal";
                    case HandJointId.Body_RightHandIndexIntermediate:
                        return "Body_RightHandIndexIntermediate";
                    case HandJointId.Body_RightHandIndexDistal:
                        return "Body_RightHandIndexDistal";
                    case HandJointId.Body_RightHandIndexTip:
                        return "Body_RightHandIndexTip";
                    case HandJointId.Body_RightHandMiddleMetacarpal:
                        return "Body_RightHandMiddleMetacarpal";
                    case HandJointId.Body_RightHandMiddleProximal:
                        return "Body_RightHandMiddleProximal";
                    case HandJointId.Body_RightHandMiddleIntermediate:
                        return "Body_RightHandMiddleIntermediate";
                    case HandJointId.Body_RightHandMiddleDistal:
                        return "Body_RightHandMiddleDistal";
                    case HandJointId.Body_RightHandMiddleTip:
                        return "Body_RightHandMiddleTip";
                    case HandJointId.Body_RightHandRingMetacarpal:
                        return "Body_RightHandRingMetacarpal";
                    case HandJointId.Body_RightHandRingProximal:
                        return "Body_RightHandRingProximal";
                    case HandJointId.Body_RightHandRingIntermediate:
                        return "Body_RightHandRingIntermediate";
                    case HandJointId.Body_RightHandRingDistal:
                        return "Body_RightHandRingDistal";
                    case HandJointId.Body_RightHandRingTip:
                        return "Body_RightHandRingTip";
                    case HandJointId.Body_RightHandLittleMetacarpal:
                        return "Body_RightHandLittleMetacarpal";
                    case HandJointId.Body_RightHandLittleProximal:
                        return "Body_RightHandLittleProximal";
                    case HandJointId.Body_RightHandLittleIntermediate:
                        return "Body_RightHandLittleIntermediate";
                    case HandJointId.Body_RightHandLittleDistal:
                        return "Body_RightHandLittleDistal";
                    case HandJointId.Body_RightHandLittleTip:
                        return "Body_RightHandLittleTip";
                    default:
                        return "Body_Unknown";
                }
            }
            else if (skeletonType == SkeletonType.HandLeft || skeletonType == SkeletonType.HandRight)
            {
                switch (boneId)
                {
                    case HandJointId.HandWristRoot:
                        return "Hand_WristRoot";
                    case HandJointId.HandForearmStub:
                        return "Hand_ForearmStub";
                    case HandJointId.HandThumb0:
                        return "Hand_Thumb0";
                    case HandJointId.HandThumb1:
                        return "Hand_Thumb1";
                    case HandJointId.HandThumb2:
                        return "Hand_Thumb2";
                    case HandJointId.HandThumb3:
                        return "Hand_Thumb3";
                    case HandJointId.HandIndex1:
                        return "Hand_Index1";
                    case HandJointId.HandIndex2:
                        return "Hand_Index2";
                    case HandJointId.HandIndex3:
                        return "Hand_Index3";
                    case HandJointId.HandMiddle1:
                        return "Hand_Middle1";
                    case HandJointId.HandMiddle2:
                        return "Hand_Middle2";
                    case HandJointId.HandMiddle3:
                        return "Hand_Middle3";
                    case HandJointId.HandRing1:
                        return "Hand_Ring1";
                    case HandJointId.HandRing2:
                        return "Hand_Ring2";
                    case HandJointId.HandRing3:
                        return "Hand_Ring3";
                    case HandJointId.HandPinky0:
                        return "Hand_Pinky0";
                    case HandJointId.HandPinky1:
                        return "Hand_Pinky1";
                    case HandJointId.HandPinky2:
                        return "Hand_Pinky2";
                    case HandJointId.HandPinky3:
                        return "Hand_Pinky3";
                    case HandJointId.HandThumbTip:
                        return "Hand_ThumbTip";
                    case HandJointId.HandIndexTip:
                        return "Hand_IndexTip";
                    case HandJointId.HandMiddleTip:
                        return "Hand_MiddleTip";
                    case HandJointId.HandRingTip:
                        return "Hand_RingTip";
                    case HandJointId.HandPinkyTip:
                        return "Hand_PinkyTip";
                    default:
                        return "Hand_Unknown";
                }
            }
            else
            {
                return "Skeleton_Unknown";
            }
        }
    }

    public class XRBone
    {
        public HandJointId Id { get; set; }
        public short ParentBoneIndex { get; set; }
        public Transform Transform { get; set; }

        public XRBone() { }

        public XRBone(HandJointId id, short parentBoneIndex, Transform trans)
        {
            Id = id;
            ParentBoneIndex = parentBoneIndex;
            Transform = trans;
        }
    }

    public class XRBoneCapsule
    {
        public short BoneIndex { get; set; }
        public Rigidbody CapsuleRigidbody { get; set; }
        public CapsuleCollider CapsuleCollider { get; set; }

        public XRBoneCapsule() { }

        public XRBoneCapsule(short boneIndex, Rigidbody capsuleRigidBody, CapsuleCollider capsuleCollider)
        {
            BoneIndex = boneIndex;
            CapsuleRigidbody = capsuleRigidBody;
            CapsuleCollider = capsuleCollider;
        }
    }
}