using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR.Management;
using UnityEngine.UI;
using TMPro;



using UnityEngine.XR;
 

using InputDevice = UnityEngine.XR.InputDevice;
using CommonUsages = UnityEngine.XR.CommonUsages;
using UnityEngine.InputSystem;

namespace Unity.XR.GSXR
{
 
    public class GSXR_DeviceStates : MonoBehaviour
    {
     

        private List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();
        public bool GetController(XRNode  node, ref UnityEngine.XR.InputDevice device)
        {
         
            InputDevices.GetDevicesAtXRNode(node, devices);
            if (devices.Count > 0)
            {
                  device = devices[0];
                return true;
            }
            else
            {
                return false;
            }
        }

 
        // Start is called before the first frame update
        void Start()
        {
            if (isActionAvtive) EndableActions();
        }


        [Header("  Data From ")]
        public bool isActionAvtive = false;


        private Vector3 Hmd_LocalPosition;
        private Quaternion Hmd_LocalRotation;
        private bool Hmd_BackPressed = false;
        private bool Hmd_StartPressed = false;
        private bool  Hmd_Present = false;
        private float Hmd_BatteryLevel = 0;

        [Header("Hmd Info")]
        public TextMeshProUGUI Hmd_LocalPositionText;
        public TextMeshProUGUI Hmd_LocalRotationText;
        public Toggle Hmd_BackPressedToggle;
        public Toggle Hmd_StartPressedToggle;
        public Toggle Hmd_PresentToggle;
        public TextMeshProUGUI Hmd_BatteryLevelText ;
        public TextMeshProUGUI Hmd_FpsText;
        public TextMeshProUGUI App_VersionText;
        private bool LeftController_Connect = false;
        private Vector3 LeftController_LocalPosition;
        private Quaternion LeftController_LocalRotation;
        private Vector3 LeftController_LineVelocity;
        private Vector3 LeftController_AngularVelocity;
        private bool LeftController_XButtonPressed = false;
        private bool LeftController_YButtonPressed = false;
        private bool LeftController_XButtonTouch = false;
        private bool LeftController_YButtonTouch = false;
        private bool LeftController_TriggerPressed = false;
        private bool LeftController_GripPressed = false;
        private float LeftController_TriggerAxis1D = 0;
        private float LeftController_GripAxis1D = 0;
        private bool LeftController_ThumbstickTouch = false;
        private bool LeftController_ThumbstickPressed = false;
        private Vector2 LeftController_ThumbstickAxis2D = Vector2.zero;
        private bool LeftController_Present = false;
        private float LeftController_BatteryLevel = 0;

        [Header("Left Controller Info")]
        public Toggle LeftController_ConnectToggle;
        public TextMeshProUGUI LeftController_LocalPositionText;
        public TextMeshProUGUI LeftController_LocalRotationText;
        public TextMeshProUGUI LeftController_LineVelocityText;
        public TextMeshProUGUI LeftController_AngularVelocityText;
        public Toggle LeftController_XButtonPressedToggle;
        public Toggle LeftController_YButtonPressedToggle;
        public Toggle LeftController_XButtonTouchToggle;
        public Toggle LeftController_YButtonTouchToggle;
        public Toggle LeftController_TriggerPressedToggle;
        public Toggle LeftController_GripPressedToggle;
        public TextMeshProUGUI LeftController_TriggerAxis1DText;
        public TextMeshProUGUI LeftController_GripAxis1DText;
        public Toggle LeftController_ThumbstickTouchToggle;
        public Toggle LeftController_ThumbstickPressedToggle;
        public TextMeshProUGUI LeftController_ThumbstickAxis2DText;
        public Toggle LeftController_PresentToggle;
        public TextMeshProUGUI LeftController_BatteryLevelText;

        private bool RightController_Connect = false;
        private Vector3 RightController_LocalPosition;
        private Quaternion RightController_LocalRotation;
        private Vector3 RightController_LineVelocity;
        private Vector3 RightController_AngularVelocity;
        private bool RightController_AButtonPressed = false;
        private bool RightController_BButtonPressed = false;
        private bool RightController_AButtonTouch = false;
        private bool RightController_BButtonTouch = false;
        private bool RightController_TriggerPressed = false;
        private bool RightController_GripPressed = false;
        private float RightController_TriggerAxis1D = 0;
        private float RightController_GripAxis1D = 0;
        private bool RightController_ThumbstickTouch = false;
        private bool RightController_ThumbstickPressed = false;
        private Vector2 RightController_ThumbstickAxis2D = Vector2.zero;
        private bool RightController_Present = false;
        private float RightController_BatteryLevel = 0;

        [Header("Right Controller Info")]
        public Toggle RightController_ConnectToggle;
        public TextMeshProUGUI RightController_LocalPositionText;
        public TextMeshProUGUI RightController_LocalRotationText;
        public TextMeshProUGUI RightController_LineVelocityText;
        public TextMeshProUGUI RightController_AngularVelocityText;
        public Toggle RightController_XButtonPressedToggle;
        public Toggle RightController_YButtonPressedToggle;
        public Toggle RightController_XButtonTouchToggle;
        public Toggle RightController_YButtonTouchToggle;
        public Toggle RightController_TriggerPressedToggle;
        public Toggle RightController_GripPressedToggle;
        public TextMeshProUGUI RightController_TriggerAxis1DText;
        public TextMeshProUGUI RightController_GripAxis1DText;
        public Toggle RightController_ThumbstickTouchToggle;
        public Toggle RightController_ThumbstickPressedToggle;
        public TextMeshProUGUI RightController_ThumbstickAxis2DText;
        public Toggle RightController_PresentToggle;
        public TextMeshProUGUI RightController_BatteryLevelText;






         // Update is called once per frame
         void LateUpdate()
        {
            UpdateHmdState();
            UpdateRightControllerState();
            UpdateLeftControllerState();
        }
        private UnityEngine.XR.InputDevice hmd = new UnityEngine.XR.InputDevice();
        private float updateInterval = 0.5f;
        private float accum = 0.0f;
        private int frames = 0;
        private float timeLeft = 0.0f;
        private string strFps = null;
        private Vector3 temppos = Vector3.zero;
        private Quaternion temprotate = Quaternion.identity;
        private float tempfps = 0;
        private void UpdateHmdState()
        {
            if (!isActionAvtive)
            {
                UpdateHmdStatefromDevice();
            }
         
            if (Hmd_LocalPositionText) Hmd_LocalPositionText.text = Hmd_LocalPosition.ToString("F2");
            if (Hmd_LocalRotationText) Hmd_LocalRotationText.text = Hmd_LocalRotation.ToString("F2");
            if (Hmd_BackPressedToggle) Hmd_BackPressedToggle.isOn = Hmd_BackPressed;
            if (Hmd_StartPressedToggle) Hmd_StartPressedToggle.isOn = Hmd_StartPressed;
            if (Hmd_PresentToggle) Hmd_PresentToggle.isOn = Hmd_Present;
            if (Hmd_BatteryLevelText) Hmd_BatteryLevelText.text = Hmd_BatteryLevel.ToString() + "%";

            //FPSValue 
            if (Hmd_FpsText)
            {
                timeLeft -= Time.unscaledDeltaTime;
                accum += Time.unscaledDeltaTime;
                ++frames;

                if (timeLeft <= 0.0)
                {
                    float fps = frames / accum;


                    if (tempfps != fps)
                    {
                        tempfps = fps;
                        strFps = string.Format("{0:f0}", fps);
                        Hmd_FpsText.text = strFps;
                        Hmd_FpsText.SetAllDirty();

                    }
                    timeLeft += updateInterval;
                    accum = 0.0f;
                    frames = 0;
                }

            }


            if (App_VersionText)
            {
                App_VersionText.text = Application.version;
            }


        }

        private void UpdateHmdStatefromDevice()
        {
            bool result = GetController(XRNode.CenterEye, ref hmd);
            if (result)
            {


                hmd.TryGetFeatureValue(CommonUsages.centerEyePosition, out Hmd_LocalPosition);
                hmd.TryGetFeatureValue(CommonUsages.centerEyeRotation, out Hmd_LocalRotation);

                hmd.TryGetFeatureValue(CommonUsages.userPresence, out Hmd_BackPressed);
                hmd.TryGetFeatureValue(CommonUsages.userPresence, out Hmd_StartPressed);

                hmd.TryGetFeatureValue(CommonUsages.userPresence, out Hmd_Present);
                hmd.TryGetFeatureValue(CommonUsages.batteryLevel, out Hmd_BatteryLevel);

            }
        }

        private InputDevice RightController = new InputDevice();
        private void UpdateRightControllerState()
        {
            UpdateRightControllerStatefromDevice();

            if (RightController_ConnectToggle) RightController_ConnectToggle.isOn = RightController_Connect;

            if (RightController_LocalPositionText) RightController_LocalPositionText.text = RightController_LocalPosition.ToString("F2");
            if (RightController_LocalRotationText) RightController_LocalRotationText.text = RightController_LocalRotation.ToString("F2");
            if (RightController_LineVelocityText) RightController_LineVelocityText.text = RightController_LineVelocity.ToString("F2");
            if (RightController_AngularVelocityText) RightController_AngularVelocityText.text = RightController_AngularVelocity.ToString("F2");
            if (RightController_XButtonPressedToggle) RightController_XButtonPressedToggle.isOn = RightController_AButtonPressed;
            if (RightController_YButtonPressedToggle) RightController_YButtonPressedToggle.isOn = RightController_BButtonPressed;
            if (RightController_XButtonTouchToggle) RightController_XButtonTouchToggle.isOn = RightController_AButtonTouch;
            if (RightController_YButtonTouchToggle) RightController_YButtonTouchToggle.isOn = RightController_BButtonTouch;
            if (RightController_TriggerPressedToggle) RightController_TriggerPressedToggle.isOn = RightController_TriggerPressed;
            if (RightController_GripPressedToggle) RightController_GripPressedToggle.isOn = RightController_GripPressed;
            if (RightController_TriggerAxis1DText) RightController_TriggerAxis1DText.text = RightController_TriggerAxis1D.ToString("F2");
            if (RightController_GripAxis1DText) RightController_GripAxis1DText.text = RightController_GripAxis1D.ToString("F2");
            if (RightController_ThumbstickTouchToggle) RightController_ThumbstickTouchToggle.isOn = RightController_ThumbstickTouch;
            if (RightController_ThumbstickPressedToggle) RightController_ThumbstickPressedToggle.isOn = RightController_ThumbstickPressed;
            if (RightController_ThumbstickAxis2DText) RightController_ThumbstickAxis2DText.text = RightController_ThumbstickAxis2D.ToString("F2");
            if (RightController_PresentToggle) RightController_PresentToggle.isOn = RightController_Present;
            if (RightController_BatteryLevelText) RightController_BatteryLevelText.text = RightController_BatteryLevel.ToString() + "%";


        }

        private void UpdateRightControllerStatefromDevice()
        {
            bool result = GetController(XRNode.RightHand, ref RightController);
            if (result)
            {
                RightController_Connect = true;
                RightController.TryGetFeatureValue(CommonUsages.devicePosition, out RightController_LocalPosition);
                RightController.TryGetFeatureValue(CommonUsages.deviceRotation, out RightController_LocalRotation);
                RightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out RightController_LineVelocity);
                RightController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out RightController_AngularVelocity);
                RightController.TryGetFeatureValue(CommonUsages.primaryButton, out RightController_AButtonPressed);
                RightController.TryGetFeatureValue(CommonUsages.secondaryButton, out RightController_BButtonPressed);
                RightController.TryGetFeatureValue(CommonUsages.primaryTouch, out RightController_AButtonTouch);
                RightController.TryGetFeatureValue(CommonUsages.secondaryTouch, out RightController_BButtonTouch);
                RightController.TryGetFeatureValue(CommonUsages.triggerButton, out RightController_TriggerPressed);
                RightController.TryGetFeatureValue(CommonUsages.gripButton, out RightController_GripPressed);
                RightController.TryGetFeatureValue(CommonUsages.trigger, out RightController_TriggerAxis1D);
                RightController.TryGetFeatureValue(CommonUsages.grip, out RightController_GripAxis1D);
                RightController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out RightController_ThumbstickTouch);
                RightController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out RightController_ThumbstickPressed);
                RightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out RightController_ThumbstickAxis2D);
                RightController.TryGetFeatureValue(CommonUsages.userPresence, out RightController_Present);
                RightController.TryGetFeatureValue(CommonUsages.batteryLevel, out RightController_BatteryLevel);
            }
            else
            {
                RightController_Connect = false;
            }
        }

        private InputDevice LeftController = new InputDevice();
        private void UpdateLeftControllerState()
        {
            UpdateLeftControllerStatefromDevice();

            if (LeftController_ConnectToggle) LeftController_ConnectToggle.isOn = LeftController_Connect;

            if (LeftController_LocalPositionText) LeftController_LocalPositionText.text = LeftController_LocalPosition.ToString("F2");
            if (LeftController_LocalRotationText) LeftController_LocalRotationText.text = LeftController_LocalRotation.ToString("F2");
            if (LeftController_LineVelocityText) LeftController_LineVelocityText.text = LeftController_LineVelocity.ToString("F2");
            if (LeftController_AngularVelocityText) LeftController_AngularVelocityText.text = LeftController_AngularVelocity.ToString("F2");
            if (LeftController_XButtonPressedToggle) LeftController_XButtonPressedToggle.isOn = LeftController_XButtonPressed;
            if (LeftController_YButtonPressedToggle) LeftController_YButtonPressedToggle.isOn = LeftController_YButtonPressed;
            if (LeftController_XButtonTouchToggle) LeftController_XButtonTouchToggle.isOn = LeftController_XButtonTouch;
            if (LeftController_YButtonTouchToggle) LeftController_YButtonTouchToggle.isOn = LeftController_YButtonTouch;
            if (LeftController_TriggerPressedToggle) LeftController_TriggerPressedToggle.isOn = LeftController_TriggerPressed;
            if (LeftController_GripPressedToggle) LeftController_GripPressedToggle.isOn = LeftController_GripPressed;
            if (LeftController_TriggerAxis1DText) LeftController_TriggerAxis1DText.text = LeftController_TriggerAxis1D.ToString("F2");
            if (LeftController_GripAxis1DText) LeftController_GripAxis1DText.text = LeftController_GripAxis1D.ToString("F2");
            if (LeftController_ThumbstickTouchToggle) LeftController_ThumbstickTouchToggle.isOn = LeftController_ThumbstickTouch;
            if (LeftController_ThumbstickPressedToggle) LeftController_ThumbstickPressedToggle.isOn = LeftController_ThumbstickPressed;
            if (LeftController_ThumbstickAxis2DText) LeftController_ThumbstickAxis2DText.text = LeftController_ThumbstickAxis2D.ToString("F2");
            if (LeftController_PresentToggle) LeftController_PresentToggle.isOn = LeftController_Present;
            if (LeftController_BatteryLevelText) LeftController_BatteryLevelText.text = LeftController_BatteryLevel.ToString() + "%";
        }

        private void UpdateLeftControllerStatefromDevice()
        {
            bool result = GetController(XRNode.LeftHand, ref LeftController);
            if (result)
            {
                LeftController_Connect = true;
                LeftController.TryGetFeatureValue(CommonUsages.devicePosition, out LeftController_LocalPosition);
                LeftController.TryGetFeatureValue(CommonUsages.deviceRotation, out LeftController_LocalRotation);
                LeftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out LeftController_LineVelocity);
                LeftController.TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out LeftController_AngularVelocity);
                LeftController.TryGetFeatureValue(CommonUsages.primaryButton, out LeftController_XButtonPressed);
                LeftController.TryGetFeatureValue(CommonUsages.secondaryButton, out LeftController_YButtonPressed);
                LeftController.TryGetFeatureValue(CommonUsages.primaryTouch, out LeftController_XButtonTouch);
                LeftController.TryGetFeatureValue(CommonUsages.secondaryTouch, out LeftController_YButtonTouch);
                LeftController.TryGetFeatureValue(CommonUsages.triggerButton, out LeftController_TriggerPressed);
                LeftController.TryGetFeatureValue(CommonUsages.gripButton, out LeftController_GripPressed);
                LeftController.TryGetFeatureValue(CommonUsages.trigger, out LeftController_TriggerAxis1D);
                LeftController.TryGetFeatureValue(CommonUsages.grip, out LeftController_GripAxis1D);
                LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisTouch, out LeftController_ThumbstickTouch);
                LeftController.TryGetFeatureValue(CommonUsages.primary2DAxisClick, out LeftController_ThumbstickPressed);
                LeftController.TryGetFeatureValue(CommonUsages.primary2DAxis, out LeftController_ThumbstickAxis2D);
                LeftController.TryGetFeatureValue(CommonUsages.userPresence, out LeftController_Present);
                LeftController.TryGetFeatureValue(CommonUsages.batteryLevel, out LeftController_BatteryLevel);
            }
            else
            {
                LeftController_Connect = false;
            }
        }
 

        [Header("Action Base Hmd Action Bindings")]
        public UnityEngine.InputSystem.InputAction Hmd_LocalPositionAction ;//= new InputAction(name: "Hmd_LocalPositionAction", type: InputActionType.Value, binding: "<XR HMD>/centerEyePosition");
        public UnityEngine.InputSystem.InputAction Hmd_LocalRotationAction ;//= new InputAction(name: "Hmd_LocalRotationAction", type: InputActionType.Value, binding: "<XR HMD>/centerEyeRotation");
        public UnityEngine.InputSystem.InputAction Hmd_BackPressedAction ;//= new InputAction(name: "Hmd_BackPressedAction", type: InputActionType.Button, binding: "<XR HMD>/back");
        public UnityEngine.InputSystem.InputAction Hmd_StartPressedAction ;//= new InputAction(name: "Hmd_StartPressedAction", type: InputActionType.Button, binding: "<XR HMD>/start");
        public UnityEngine.InputSystem.InputAction Hmd_PresentAction ;//= new InputAction(name: "Hmd_PresentAction", type: InputActionType.Button, binding: "<XR HMD>/userPresence");
        public UnityEngine.InputSystem.InputAction Hmd_BatteryLevelAction ;//= new InputAction(name: "Hmd_BatteryLevelAction", type: InputActionType.Value, binding: "<XR HMD>/batteryLevel");

        [Header("Action Base RightController Action Bindings")]
        public UnityEngine.InputSystem.InputAction RightController_ConnectAction ;//= new InputAction(name: "RightController_ConnectAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/isTracked");
        public UnityEngine.InputSystem.InputAction RightController_LocalPositionAction ;//= new InputAction(name: "RightController_LocalPositionAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/devicePosition");
        public UnityEngine.InputSystem.InputAction RightController_LocalRotationAction ;//= new InputAction(name: "RightController_LocalRotationAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/deviceRotation");
        public UnityEngine.InputSystem.InputAction RightController_LineVelocityAction ;//= new InputAction(name: "RightController_LineVelocityAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/deviceVelocity");
        public UnityEngine.InputSystem.InputAction RightController_AngularVelocityAction ;//= new InputAction(name: "RightController_AngularVelocityAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/deviceAngularVelocity");
        public UnityEngine.InputSystem.InputAction RightController_AButtonPressedAction ;//= new InputAction(name: "RightController_AButtonPressedAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/primaryButton");
        public UnityEngine.InputSystem.InputAction RightController_BButtonPressedAction ;//= new InputAction(name: "RightController_BButtonPressedAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/secondaryButton");
        public UnityEngine.InputSystem.InputAction RightController_AButtonTouchAction ;//= new InputAction(name: "RightController_AButtonTouchAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/primaryTouch");
        public UnityEngine.InputSystem.InputAction RightController_BButtonTouchAction ;//= new InputAction(name: "RightController_BButtonTouchAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/secondaryTouch");
        public UnityEngine.InputSystem.InputAction RightController_TriggerPressedAction ;//= new InputAction(name: "RightController_TriggerPressedAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/triggerButton");
        public UnityEngine.InputSystem.InputAction RightController_GripPressedAction ;//= new InputAction(name: "RightController_GripPressedAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/gripButton");
        public UnityEngine.InputSystem.InputAction RightController_TriggerAxis1DAction ;//= new InputAction(name: "RightController_TriggerAxis1DAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/trigger");
        public UnityEngine.InputSystem.InputAction RightController_GripAxis1DAction ;//= new InputAction(name: "RightController_GripAxis1DAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/grip");
        public UnityEngine.InputSystem.InputAction RightController_ThumbstickTouchAction ;//= new InputAction(name: "RightController_ThumbstickTouchAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/thumbstickTouch");
        public UnityEngine.InputSystem.InputAction RightController_ThumbstickPressedAction ;//= new InputAction(name: "RightController_ThumbstickPressedAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/thumbstickClick");
        public UnityEngine.InputSystem.InputAction RightController_ThumbstickAxis2DAction ;//= new InputAction(name: "RightController_ThumbstickAxis2DAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/thumbstick");
        public UnityEngine.InputSystem.InputAction RightController_PresentAction ;//= new InputAction(name: "RightController_PresentAction", type: InputActionType.Button, binding: "<XR Controller>{RightHand}/userPresence");
        public UnityEngine.InputSystem.InputAction RightController_BatteryLevelAction ;//= new InputAction(name: "RightController_BatteryLevelAction", type: InputActionType.Value, binding: "<XR Controller>{RightHand}/batteryLevel");


        [Header("Action Base LeftController Action Bindings")]
        public UnityEngine.InputSystem.InputAction LeftController_ConnectAction ;//= new InputAction(name: "LeftController_ConnectAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/isTracked");
        public UnityEngine.InputSystem.InputAction LeftController_LocalPositionAction ;//= new InputAction(name: "LeftController_LocalPositionAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/devicePosition");
        public UnityEngine.InputSystem.InputAction LeftController_LocalRotationAction ;//= new InputAction(name: "LeftController_LocalRotationAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/deviceRotation");
        public UnityEngine.InputSystem.InputAction LeftController_LineVelocityAction ;//= new InputAction(name: "LeftController_LineVelocityAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/deviceVelocity");
        public UnityEngine.InputSystem.InputAction LeftController_AngularVelocityAction ;//= new InputAction(name: "LeftController_AngularVelocityAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/deviceAngularVelocity");
        public UnityEngine.InputSystem.InputAction LeftController_AButtonPressedAction ;//= new InputAction(name: "LeftController_AButtonPressedAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/primaryButton");
        public UnityEngine.InputSystem.InputAction LeftController_BButtonPressedAction ;//= new InputAction(name: "LeftController_BButtonPressedAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/secondaryButton");
        public UnityEngine.InputSystem.InputAction LeftController_AButtonTouchAction ;//= new InputAction(name: "LeftController_AButtonTouchAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/primaryTouch");
        public UnityEngine.InputSystem.InputAction LeftController_BButtonTouchAction ;//= new InputAction(name: "LeftController_BButtonTouchAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/secondaryTouch");
        public UnityEngine.InputSystem.InputAction LeftController_TriggerPressedAction ;//= new InputAction(name: "LeftController_TriggerPressedAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/triggerButton");
        public UnityEngine.InputSystem.InputAction LeftController_GripPressedAction ;//= new InputAction(name: "LeftController_GripPressedAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/gripButton");
        public UnityEngine.InputSystem.InputAction LeftController_TriggerAxis1DAction ;//= new InputAction(name: "LeftController_TriggerAxis1DAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/trigger");
        public UnityEngine.InputSystem.InputAction LeftController_GripAxis1DAction ;//= new InputAction(name: "LeftController_GripAxis1DAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/grip");
        public UnityEngine.InputSystem.InputAction LeftController_ThumbstickTouchAction ;//= new InputAction(name: "LeftController_ThumbstickTouchAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/thumbstickTouch");
        public UnityEngine.InputSystem.InputAction LeftController_ThumbstickPressedAction ;//= new InputAction(name: "LeftController_ThumbstickPressedAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/thumbstickClick");
        public UnityEngine.InputSystem.InputAction LeftController_ThumbstickAxis2DAction ;//= new InputAction(name: "LeftController_ThumbstickAxis2DAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/thumbstick");
        public UnityEngine.InputSystem.InputAction LeftController_PresentAction ;//= new InputAction(name: "LeftController_PresentAction", type: InputActionType.Button, binding: "<XR Controller>{LeftHand}/userPresence");
        public UnityEngine.InputSystem.InputAction LeftController_BatteryLevelAction ;//= new InputAction(name: "LeftController_BatteryLevelAction", type: InputActionType.Value, binding: "<XR Controller>{LeftHand}/batteryLevel");


         


        private void EndableActions()
        {
            Hmd_LocalPositionAction.Enable();
            Hmd_LocalRotationAction.Enable();
            Hmd_BackPressedAction.Enable();
            Hmd_StartPressedAction.Enable();
            Hmd_PresentAction.Enable();
            Hmd_BatteryLevelAction.Enable();


            RightController_ConnectAction.Enable();
            RightController_LocalPositionAction.Enable();
            RightController_LocalRotationAction.Enable();
            RightController_LineVelocityAction.Enable();
            RightController_AngularVelocityAction.Enable();
            RightController_AButtonPressedAction.Enable();
            RightController_BButtonPressedAction.Enable();
            RightController_AButtonTouchAction.Enable();
            RightController_BButtonTouchAction.Enable();
            RightController_TriggerPressedAction.Enable();
            RightController_GripPressedAction.Enable();
            RightController_TriggerAxis1DAction.Enable();
            RightController_GripAxis1DAction.Enable();
            RightController_ThumbstickTouchAction.Enable();
            RightController_ThumbstickPressedAction.Enable();
            RightController_ThumbstickAxis2DAction.Enable();
            RightController_PresentAction.Enable();
            RightController_BatteryLevelAction.Enable();


            LeftController_ConnectAction.Enable();
            LeftController_LocalPositionAction.Enable();
            LeftController_LocalRotationAction.Enable();
            LeftController_LineVelocityAction.Enable();
            LeftController_AngularVelocityAction.Enable();
            LeftController_AButtonPressedAction.Enable();
            LeftController_BButtonPressedAction.Enable();
            LeftController_AButtonTouchAction.Enable();
            LeftController_BButtonTouchAction.Enable();
            LeftController_TriggerPressedAction.Enable();
            LeftController_GripPressedAction.Enable();
            LeftController_TriggerAxis1DAction.Enable();
            LeftController_GripAxis1DAction.Enable();
            LeftController_ThumbstickTouchAction.Enable();
            LeftController_ThumbstickPressedAction.Enable();
            LeftController_ThumbstickAxis2DAction.Enable();
            LeftController_PresentAction.Enable();
            LeftController_BatteryLevelAction.Enable();



            Hmd_LocalPositionAction.performed += (InputAction.CallbackContext context) =>
            {
                Hmd_LocalPosition = context.ReadValue<Vector3>();
            };
            Hmd_LocalRotationAction.performed += (InputAction.CallbackContext context) =>
            {
                Hmd_LocalRotation = context.ReadValue<Quaternion>();
            };
            Hmd_BackPressedAction.performed += (InputAction.CallbackContext context) =>
            {
                Hmd_BackPressed = context.ReadValueAsButton();
            };
            Hmd_StartPressedAction.performed += (InputAction.CallbackContext context) => 
            {
                Hmd_StartPressed = context.ReadValueAsButton();
            };
            Hmd_PresentAction.performed += (InputAction.CallbackContext context) =>
            {
             
                   Hmd_Present = context.ReadValueAsButton();
               

            };
            Hmd_BatteryLevelAction.performed += (InputAction.CallbackContext context) =>
            {
                Hmd_BatteryLevel = context.ReadValue<float>();
            };


        }



        public void ResetOrientationAndPosition()
        {
            InputTracking.Recenter();
         
        }

        public void SystemSettingWifi()
        {
            GSXR_Plugin.SetSystemSetting( GSXR_XrSystemSettingType.GSXR_XrSystemSettingType_Wifi);

        }


        public void SystemSettingBluetooth()
        {
            GSXR_Plugin.SetSystemSetting(GSXR_XrSystemSettingType.GSXR_XrSystemSettingType_Bluetooth);


        }


    }
}
