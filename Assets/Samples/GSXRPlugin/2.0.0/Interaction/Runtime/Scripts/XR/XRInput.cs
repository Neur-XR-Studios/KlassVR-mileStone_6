using XR.Interaction.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using Node = UnityEngine.XR.XRNode;

public static class XRInput
{
    [Flags]
    /// Virtual 1-dimensional axis (float) mappings that allow the same input bindings to work across different controllers.
    public enum Axis1D
    {
        None = 0,     ///< Maps to RawAxis1D: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		PrimaryIndexTrigger = 0x01,  ///< Maps to RawAxis1D: [Gamepad, Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger], [Remote: None]
		PrimaryHandTrigger = 0x04,  ///< Maps to RawAxis1D: [Touch, LTouch: LHandTrigger], [RTouch: RHandTrigger], [Gamepad, Remote: None]
		SecondaryIndexTrigger = 0x02,  ///< Maps to RawAxis1D: [Gamepad, Touch: RIndexTrigger], [LTouch, RTouch, Remote: None]
		SecondaryHandTrigger = 0x08,  ///< Maps to RawAxis1D: [Touch: RHandTrigger], [Gamepad, LTouch, RTouch, Remote: None]
		PrimaryIndexTriggerCurl = 0x10,
        PrimaryIndexTriggerSlide = 0x20,
        PrimaryThumbRestForce = 0x40,
        PrimaryStylusForce = 0x80,
        SecondaryIndexTriggerCurl = 0x100,
        SecondaryIndexTriggerSlide = 0x200,
        SecondaryThumbRestForce = 0x400,
        SecondaryStylusForce = 0x800,
        Any = ~None, ///< Maps to RawAxis1D: [Gamepad, Touch, LTouch, RTouch: Any], [Remote: None]
	}
    [Flags]
    /// Virtual button mappings that allow the same input bindings to work across different controllers.
    public enum Button
    {
        None = 0,          ///< Maps to RawButton: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		One = 0x00000001, ///< Maps to RawButton: [Gamepad, Touch, RTouch: A], [LTouch: X], [Remote: Start]
		Two = 0x00000002, ///< Maps to RawButton: [Gamepad, Touch, RTouch: B], [LTouch: Y], [Remote: Back]
		Three = 0x00000004, ///< Maps to RawButton: [Gamepad, Touch: X], [LTouch, RTouch, Remote: None]
		Four = 0x00000008, ///< Maps to RawButton: [Gamepad, Touch: Y], [LTouch, RTouch, Remote: None]
		Start = 0x00000100, ///< Maps to RawButton: [Gamepad: Start], [Touch, LTouch, Remote: Start], [RTouch: None]
		Back = 0x00000200, ///< Maps to RawButton: [Gamepad, Remote: Back], [Touch, LTouch, RTouch: None]
		PrimaryShoulder = 0x00001000, ///< Maps to RawButton: [Gamepad: LShoulder], [Touch, LTouch, RTouch, Remote: None]
		PrimaryIndexTrigger = 0x00002000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger], [Remote: None]
		PrimaryHandTrigger = 0x00004000, ///< Maps to RawButton: [Touch, LTouch: LHandTrigger], [RTouch: RHandTrigger], [Gamepad, Remote: None]
		PrimaryThumbstick = 0x00008000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstick], [RTouch: RThumbstick], [Remote: None]
		PrimaryThumbstickUp = 0x00010000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickUp], [RTouch: RThumbstickUp], [Remote: None]
		PrimaryThumbstickDown = 0x00020000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickDown], [RTouch: RThumbstickDown], [Remote: None]
		PrimaryThumbstickLeft = 0x00040000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickLeft], [RTouch: RThumbstickLeft], [Remote: None]
		PrimaryThumbstickRight = 0x00080000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickRight], [RTouch: RThumbstickRight], [Remote: None]
		PrimaryTouchpad = 0x00000400, ///< Maps to RawButton: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		SecondaryShoulder = 0x00100000, ///< Maps to RawButton: [Gamepad: RShoulder], [Touch, LTouch, RTouch, Remote: None]
		SecondaryIndexTrigger = 0x00200000, ///< Maps to RawButton: [Gamepad, Touch: RIndexTrigger], [LTouch, RTouch, Remote: None]
		SecondaryHandTrigger = 0x00400000, ///< Maps to RawButton: [Touch: RHandTrigger], [Gamepad, LTouch, RTouch, Remote: None]
		SecondaryThumbstick = 0x00800000, ///< Maps to RawButton: [Gamepad, Touch: RThumbstick], [LTouch, RTouch, Remote: None]
		SecondaryThumbstickUp = 0x01000000, ///< Maps to RawButton: [Gamepad, Touch: RThumbstickUp], [LTouch, RTouch, Remote: None]
		SecondaryThumbstickDown = 0x02000000, ///< Maps to RawButton: [Gamepad, Touch: RThumbstickDown], [LTouch, RTouch, Remote: None]
		SecondaryThumbstickLeft = 0x04000000, ///< Maps to RawButton: [Gamepad, Touch: RThumbstickLeft], [LTouch, RTouch, Remote: None]
		SecondaryThumbstickRight = 0x08000000, ///< Maps to RawButton: [Gamepad, Touch: RThumbstickRight], [LTouch, RTouch, Remote: None]
		SecondaryTouchpad = 0x00000800, ///< Maps to RawButton: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		DpadUp = 0x00000010, ///< Maps to RawButton: [Gamepad, Remote: DpadUp], [Touch, LTouch, RTouch: None]
		DpadDown = 0x00000020, ///< Maps to RawButton: [Gamepad, Remote: DpadDown], [Touch, LTouch, RTouch: None]
		DpadLeft = 0x00000040, ///< Maps to RawButton: [Gamepad, Remote: DpadLeft], [Touch, LTouch, RTouch: None]
		DpadRight = 0x00000080, ///< Maps to RawButton: [Gamepad, Remote: DpadRight], [Touch, LTouch, RTouch: None]
		Up = 0x10000000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickUp], [RTouch: RThumbstickUp], [Remote: DpadUp]
		Down = 0x20000000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickDown], [RTouch: RThumbstickDown], [Remote: DpadDown]
		Left = 0x40000000, ///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickLeft], [RTouch: RThumbstickLeft], [Remote: DpadLeft]
		Right = unchecked((int)0x80000000),///< Maps to RawButton: [Gamepad, Touch, LTouch: LThumbstickRight], [RTouch: RThumbstickRight], [Remote: DpadRight]
		Any = ~None,      ///< Maps to RawButton: [Gamepad, Touch, LTouch, RTouch: Any]
	}

    [Flags]
    /// Raw button mappings that can be used to directly query the state of a controller.
    public enum RawButton
    {
        None = 0,          ///< Maps to Physical Button: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		A = 0x00000001, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: A], [LTouch, Remote: None]
		B = 0x00000002, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: B], [LTouch, Remote: None]
		X = 0x00000100, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: X], [RTouch, Remote: None]
		Y = 0x00000200, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: Y], [RTouch, Remote: None]
		Start = 0x00100000, ///< Maps to Physical Button: [Gamepad, Touch, LTouch, Remote: Start], [RTouch: None]
		Back = 0x00200000, ///< Maps to Physical Button: [Gamepad, Remote: Back], [Touch, LTouch, RTouch: None]
		LShoulder = 0x00000800, ///< Maps to Physical Button: [Gamepad: LShoulder], [Touch, LTouch, RTouch, Remote: None]
		LIndexTrigger = 0x10000000, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LIndexTrigger], [RTouch, Remote: None]
		LHandTrigger = 0x20000000, ///< Maps to Physical Button: [Touch, LTouch: LHandTrigger], [Gamepad, RTouch, Remote: None]
		LThumbstick = 0x00000400, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LThumbstick], [RTouch, Remote: None]
		LThumbstickUp = 0x00000010, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LThumbstickUp], [RTouch, Remote: None]
		LThumbstickDown = 0x00000020, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LThumbstickDown], [RTouch, Remote: None]
		LThumbstickLeft = 0x00000040, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LThumbstickLeft], [RTouch, Remote: None]
		LThumbstickRight = 0x00000080, ///< Maps to Physical Button: [Gamepad, Touch, LTouch: LThumbstickRight], [RTouch, Remote: None]
		LTouchpad = 0x40000000, ///< Maps to Physical Button: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		RShoulder = 0x00000008, ///< Maps to Physical Button: [Gamepad: RShoulder], [Touch, LTouch, RTouch, Remote: None]
		RIndexTrigger = 0x04000000, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RIndexTrigger], [LTouch, Remote: None]
		RHandTrigger = 0x08000000, ///< Maps to Physical Button: [Touch, RTouch: RHandTrigger], [Gamepad, LTouch, Remote: None]
		RThumbstick = 0x00000004, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RThumbstick], [LTouch, Remote: None]
		RThumbstickUp = 0x00001000, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RThumbstickUp], [LTouch, Remote: None]
		RThumbstickDown = 0x00002000, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RThumbstickDown], [LTouch, Remote: None]
		RThumbstickLeft = 0x00004000, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RThumbstickLeft], [LTouch, Remote: None]
		RThumbstickRight = 0x00008000, ///< Maps to Physical Button: [Gamepad, Touch, RTouch: RThumbstickRight], [LTouch, Remote: None]
		RTouchpad = unchecked((int)0x80000000),///< Maps to Physical Button: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		DpadUp = 0x00010000, ///< Maps to Physical Button: [Gamepad, Remote: DpadUp], [Touch, LTouch, RTouch: None]
		DpadDown = 0x00020000, ///< Maps to Physical Button: [Gamepad, Remote: DpadDown], [Touch, LTouch, RTouch: None]
		DpadLeft = 0x00040000, ///< Maps to Physical Button: [Gamepad, Remote: DpadLeft], [Touch, LTouch, RTouch: None]
		DpadRight = 0x00080000, ///< Maps to Physical Button: [Gamepad, Remote: DpadRight], [Touch, LTouch, RTouch: None]
		Any = ~None,      ///< Maps to Physical Button: [Gamepad, Touch, LTouch, RTouch, Remote: Any]
	}

    [Flags]
    /// Virtual capacitive touch mappings that allow the same input bindings to work across different controllers with capacitive touch support.
    public enum Touch
    {
        None = 0,                            ///< Maps to RawTouch: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		One = Button.One,                   ///< Maps to RawTouch: [Touch, RTouch: A], [LTouch: X], [Gamepad, Remote: None]
		Two = Button.Two,                   ///< Maps to RawTouch: [Touch, RTouch: B], [LTouch: Y], [Gamepad, Remote: None]
		Three = Button.Three,                 ///< Maps to RawTouch: [Touch: X], [Gamepad, LTouch, RTouch, Remote: None]
		Four = Button.Four,                  ///< Maps to RawTouch: [Touch: Y], [Gamepad, LTouch, RTouch, Remote: None]
		PrimaryIndexTrigger = Button.PrimaryIndexTrigger,   ///< Maps to RawTouch: [Touch, LTouch: LIndexTrigger], [RTouch: RIndexTrigger], [Gamepad, Remote: None]
		PrimaryThumbstick = Button.PrimaryThumbstick,     ///< Maps to RawTouch: [Touch, LTouch: LThumbstick], [RTouch: RThumbstick], [Gamepad, Remote: None]
		PrimaryThumbRest = 0x00001000,                   ///< Maps to RawTouch: [Touch, LTouch: LThumbRest], [RTouch: RThumbRest], [Gamepad, Remote: None]
		PrimaryTouchpad = Button.PrimaryTouchpad,       ///< Maps to RawTouch: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		SecondaryIndexTrigger = Button.SecondaryIndexTrigger, ///< Maps to RawTouch: [Touch: RIndexTrigger], [Gamepad, LTouch, RTouch, Remote: None]
		SecondaryThumbstick = Button.SecondaryThumbstick,   ///< Maps to RawTouch: [Touch: RThumbstick], [Gamepad, LTouch, RTouch, Remote: None]
		SecondaryThumbRest = 0x00100000,                   ///< Maps to RawTouch: [Touch: RThumbRest], [Gamepad, LTouch, RTouch, Remote: None]
		SecondaryTouchpad = Button.SecondaryTouchpad,     ///< Maps to RawTouch: [Gamepad, Touch, LTouch, RTouch, Remote: None]
		Any = ~None,                        ///< Maps to RawTouch: [Touch, LTouch, RTouch: Any], [Gamepad, Remote: None]
	}
    [Flags]
    /// Identifies a controller which can be used to query the virtual or raw input state.
    public enum Controller
    {
        None = 0,
        LTouch = 0x00000001,
        RTouch = 0x00000002,
        Touch = LTouch | RTouch,
        Remote = 0x00000004,
        Gamepad = 0x00000010,
        LHand = 0x00000020,
        RHand = 0x00000040,
        Hands = LHand | RHand,
        Active = unchecked((int)0x80000000),
        All = ~None,
    }





    private static Controller connectedControllerTypes = Controller.Hands;

    /// <summary>
    /// Returns a mask of all currently connected controller types.
    /// </summary>
    public static Controller GetConnectedControllers()
    {
        return connectedControllerTypes;
    }


    public static bool IsControllerConnected(Controller controller)
    {
        return true;
    }

    /// <summary>
	/// Gets the dominant hand that the user has specified in settings, for mobile devices.
	/// </summary>
	public static Handedness GetDominantHand()
    {
        return Handedness.Left;
    }

    public static Vector3 GetLocalControllerPosition(Controller controllerType)
    {
        return Vector3.zero;
    }
    public static Quaternion GetLocalControllerRotation(Controller controllerType)
    {
        return Quaternion.identity;
    }

    /// <summary>
    /// Gets the current state of the given virtual 1-dimensional axis mask on the given controller mask.
    /// Returns the value of the largest masked axis across all masked controllers. Values range from 0 to 1.
    /// </summary>
    public static float Get(Axis1D virtualMask, Controller controllerMask = Controller.Active)
    {
        return 0;
    }

}
