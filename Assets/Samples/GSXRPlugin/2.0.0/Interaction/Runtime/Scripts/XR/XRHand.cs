using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace XR.Interaction.Input
{
    public class XRHand : MonoBehaviour, XRSkeleton.ISkeletonDataProvider
    {
        [SerializeField]
        private Handedness Handedness = Handedness.Left;
        public bool IsDataValid { get; private set; }
        public bool IsTracked { get; private set; }
        public bool IsDominantHand { get; private set; }
        public bool IsPointerPoseValid { get; private set; }
        public Transform PointerPose { get; private set; }
        public bool IsDataHighConfidence { get; private set; }
        public float HandScale { get; private set; }
        public bool IsSystemGestureInProgress { get; private set; }
        [SerializeField]
        private Transform _pointerPoseRoot = null;

        private GameObject _pointerPoseGO;

        private HandState _handState = new HandState();

        public TrackingConfidence HandConfidence { get; private set; }
  
   
        private void Awake()
        {
            _pointerPoseGO = new GameObject();
            PointerPose = _pointerPoseGO.transform;
            if (_pointerPoseRoot != null)
            {
                PointerPose.SetParent(_pointerPoseRoot, false);
            }

            GetHandState(Step.Render);
        }

        private void Update()
        {
            GetHandState(Step.Render);
        }

        private void FixedUpdate()
        {
            GetHandState(Step.Physics);
        }

        private void GetHandState(Step step)
        {
            if (XRHandSkeleton.GetHandState(step, Handedness, ref _handState))
            {
                IsTracked = XRHandSkeleton.GetHandTrackedState(Handedness);
                IsSystemGestureInProgress = (_handState.Status & HandStatus.SystemGestureInProgress) != 0;
                IsPointerPoseValid = IsTracked;
                IsDominantHand = (_handState.Status & HandStatus.DominantHand) != 0;
                PointerPose.localPosition = _handState.PointerPose.Position.FromFlippedZVector3f();
                PointerPose.localRotation = _handState.PointerPose.Orientation.FromFlippedZQuatf();
                HandScale = _handState.HandScale;
                HandConfidence = (TrackingConfidence)_handState.HandConfidence;
                //TODO:Fixed Data Valid False Hand Model Visable 
                IsDataValid = IsTracked;
                IsDataHighConfidence = IsTracked && HandConfidence == TrackingConfidence.High;
            }
            else
            {
                IsTracked = false;
                IsSystemGestureInProgress = false;
                IsPointerPoseValid = false;
                PointerPose.localPosition = Vector3.zero;
                PointerPose.localRotation = Quaternion.identity;
                HandScale = 1.0f;
                HandConfidence = TrackingConfidence.Low;

                IsDataValid = false;
                IsDataHighConfidence = false;
            }
        }


        public TrackingConfidence GetFingerConfidence(HandFinger finger)
        {
            if (IsDataValid
                && _handState.FingerConfidences != null
                && _handState.FingerConfidences.Length == (int)HandFinger.Max)
            {
                return (TrackingConfidence)_handState.FingerConfidences[(int)finger];
            }

            return TrackingConfidence.Low;
        }
        public bool GetFingerIsPinching(HandFinger finger)
        {
            return IsDataValid && (((int)_handState.Pinches & (1 << (int)finger)) != 0);
        }

        public float GetFingerPinchStrength(HandFinger finger)
        {
            if (IsDataValid
                && _handState.PinchStrength != null
                && _handState.PinchStrength.Length == (int)HandFinger.Max)
            {
                return _handState.PinchStrength[(int)finger];
            }

            return 0.0f;
        }

        public SkeletonType GetSkeletonType()
        {
            switch (Handedness)
            {
                case Handedness.Left:
                    return SkeletonType.HandLeft;
                case Handedness.Right:
                    return SkeletonType.HandRight;
              default:
                    return SkeletonType.HandRight;
            }
        }

        public XRSkeleton.SkeletonPoseData GetSkeletonPoseData()
        {
            var data = new XRSkeleton.SkeletonPoseData();

            data.IsDataValid = IsDataValid;
            if (IsDataValid)
            {
                data.RootPose = _handState.RootPose;
                data.RootScale = _handState.HandScale;
                data.BoneRotations = _handState.BoneRotations;
                data.IsDataHighConfidence = IsTracked && HandConfidence == TrackingConfidence.High;
            }

            return data;
        }
    }

}