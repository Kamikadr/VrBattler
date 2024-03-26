using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Unigine.Plugins;

[Component(PropertyGuid = "21eef5e42ece47f3d44e6cb6e180fc9dbff75558")]
public sealed class VRInput : VRBaseInput
{
	public enum BUTTONS : byte
	{
		LEFT_A = 0,
		LEFT_APPLICATIONMENU,
		LEFT_AXIS0,
		LEFT_AXIS1,
		LEFT_AXIS2,
		LEFT_AXIS3,
		LEFT_AXIS4,
		LEFT_DASHBOARD_BACK,
		LEFT_DPAD_DOWN,
		LEFT_DPAD_LEFT,
		LEFT_DPAD_RIGHT,
		LEFT_DPAD_UP,
		LEFT_GRIP,
		LEFT_STEAMVR_TOUCHPAD,
		LEFT_STEAMVR_TRIGGER,
		LEFT_SYSTEM,
		RIGHT_A,
		RIGHT_APPLICATIONMENU,
		RIGHT_AXIS0,
		RIGHT_AXIS1,
		RIGHT_AXIS2,
		RIGHT_AXIS3,
		RIGHT_AXIS4,
		RIGHT_DASHBOARD_BACK,
		RIGHT_DPAD_DOWN,
		RIGHT_DPAD_LEFT,
		RIGHT_DPAD_RIGHT,
		RIGHT_DPAD_UP,
		RIGHT_GRIP,
		RIGHT_STEAMVR_TOUCHPAD,
		RIGHT_STEAMVR_TRIGGER,
		RIGHT_SYSTEM
	}

	public enum AXES : byte
	{
		JOYSTICK_VERTICAL = 0,
		JOYSTICK_HORIZONTAL,
		JOYSTICK_UP,
		JOYSTICK_RIGHT,
		JOYSTICK_DOWN,
		JOYSTICK_LEFT,
		TRIGGER,
		TRIGGER_UP,
		TRIGGER_DOWN,
	}

	public enum Counts : byte
	{
		BUTTONS = 32,
		AXES = 24
	}

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Joystick Pressed Value", Group = "Axes Pressed Values")]
	private float joystickPressedValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "None Pressed Value", Group = "Axes Pressed Values")]
	private float nonePressedValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Trackpad Value", Group = "Axes Pressed Values")]
	private float trackpadPressedValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Trigger Pressed Value", Group = "Axes Pressed Values")]
	private float triggerPressedValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Joystick Release Value", Group = "Axes Release Values")]
	private float joystickReleaseValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "None Release Value", Group = "Axes Release Values")]
	private float noneReleaseValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Trackpad Release Value", Group = "Axes Release Values")]
	private float trackpadReleaseValue = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Min = 0.0f, Max = 1.0f, Title = "Trigger Release Value", Group = "Axes Release Values")]
	private float triggerReleaseValue = 0.5f;


	public float JoystickPressedValue
	{
		get { return joystickPressedValue; }
		set { joystickPressedValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float NonePressedValue
	{
		get { return nonePressedValue; }
		set { nonePressedValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float TrackpadPressedValue
	{
		get { return trackpadPressedValue; }
		set { trackpadPressedValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float TriggerPressedValue
	{
		get { return triggerPressedValue; }
		set { triggerPressedValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float JoystickReleaseValue
	{
		get { return joystickReleaseValue; }
		set { joystickReleaseValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float NoneReleaseValue
	{
		get { return noneReleaseValue; }
		set { noneReleaseValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float TrackpadReleaseValue
	{
		get { return trackpadReleaseValue; }
		set { trackpadReleaseValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public float TriggerReleaseValue
	{
		get { return triggerReleaseValue; }
		set { triggerReleaseValue = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public override bool HeadPositionLockInterface
	{
		get => HeadPositionLock;
		set => HeadPositionLock = value;
	}

	public override bool HeadRotationLockInterface
	{
		get => HeadRotationLock;
		set => HeadRotationLock = value;
	}

	public override InputSystem.Viewport ViewportModeInterface
	{
		get
		{
			switch (ViewportMode)
			{
				case VR.MIRROR_MODE.BLACK_SCREEN: 
					return InputSystem.Viewport.BLACK_SCREEN;
				case VR.MIRROR_MODE.MONO_LEFT: 
				case VR.MIRROR_MODE.MONO_RIGHT: 
					return InputSystem.Viewport.MONO;
				case VR.MIRROR_MODE.STEREO:
					return InputSystem.Viewport.STEREO;
			}

			return InputSystem.Viewport.NONE;
		}

		set
		{
			switch (value)
			{
				case InputSystem.Viewport.BLACK_SCREEN: ViewportMode = VR.MIRROR_MODE.BLACK_SCREEN; break;
				case InputSystem.Viewport.MONO: ViewportMode = VR.MIRROR_MODE.MONO_LEFT; break;
				case InputSystem.Viewport.STEREO: ViewportMode = VR.MIRROR_MODE.STEREO; break;
				default: break;
			}
		}
	}

	static private List<int> controllersId = null;
	static private List<int> genericTrackersId = null;
	static private List<int> trackingsId = null;
	static private int hmdId = -1;

	static private Dictionary<InputSystem.VRDevice, int> devicesId = null;

	static private bool[] buttonsDown = null;
	static private bool[] buttonsPress = null;
	static private bool[] buttonsUp = null;

	static private bool[] touchesBegan = null;
	static private bool[] touchesStationary = null;
	static private bool[] touchesEnded = null;

	static private bool[] leftAxesDown = null;
	static private bool[] leftAxesPress = null;
	static private bool[] leftAxesUp = null;

	static private bool[] rightAxesDown = null;
	static private bool[] rightAxesPress = null;
	static private bool[] rightAxesUp = null;

	static private bool firstInstance = true;

	static public bool HeadPositionLock
	{
		get
		{
			if (IsLoaded)
			{
				var hmd = Input.VRHead;
				if (hmd)
					return !hmd.TrackingPositionEnabled; // because lock == not enabled

				return false;
			}
			return false;
		}

		set
		{
			if (IsLoaded)
			{
				var hmd = Input.VRHead;
				if (hmd)
					hmd.TrackingPositionEnabled = !value; // because lock == not enabled
			}
		}
	}

	static public bool HeadRotationLock
	{
		get
		{
			if (IsLoaded)
			{
				var hmd = Input.VRHead;
				if (hmd)
					return !hmd.TrackingRotationEnabled; // because lock == not enabled

				return false;
			}
			return false;
		}

		set
		{
			if (IsLoaded)
			{
				var hmd = Input.VRHead;
				if (hmd)
					hmd.TrackingRotationEnabled = !value; // because lock == not enabled
			}
		}
	}

	static public bool IsLoaded => VR.ApiType != VR.API.NULL;

	static public VR.MIRROR_MODE ViewportMode
	{
		get
		{
			if (IsLoaded)
				return VR.MirrorMode;
			return VR.MIRROR_MODE.BLACK_SCREEN;
		}

		set
		{
			if (IsLoaded)
				VR.MirrorMode = value;
		}
	}

	public VRInput() : base()
	{
		if (!firstInstance)
			return;

		firstInstance = false;

		if (!IsLoaded)
			return;

		buttonsDown = new bool[(int)Counts.BUTTONS];
		buttonsPress = new bool[(int)Counts.BUTTONS];
		buttonsUp = new bool[(int)Counts.BUTTONS];

		touchesBegan = new bool[(int)Counts.BUTTONS];
		touchesStationary = new bool[(int)Counts.BUTTONS];
		touchesEnded = new bool[(int)Counts.BUTTONS];

		leftAxesDown = new bool[(int)Counts.AXES];
		leftAxesPress = new bool[(int)Counts.AXES];
		leftAxesUp = new bool[(int)Counts.AXES];

		rightAxesDown = new bool[(int)Counts.AXES];
		rightAxesPress = new bool[(int)Counts.AXES];
		rightAxesUp = new bool[(int)Counts.AXES];

		controllersId = new List<int>();
		devicesId = new Dictionary<InputSystem.VRDevice, int>();
		genericTrackersId = new List<int>();
		trackingsId = new List<int>();

		int count = Input.NumVRDevices;
		int controllersCount = 0;
		for (int i = 0; i < count; i++)
		{
			InputVRDevice device= Input.GetVRDevice(i);
			switch (device.Type)
			{
				case InputVRDevice.TYPE.INPUT_VR_CONTROLLER:
					if (controllersCount == 0)
						devicesId.Add(InputSystem.VRDevice.LEFT_CONTROLLER, i);
					else if (controllersCount == 1)
						devicesId.Add(InputSystem.VRDevice.RIGHT_CONTROLLER, i);

					controllersId.Add(i);
					controllersCount++;
					break;

				case InputVRDevice.TYPE.INPUT_VR_TRACKER: genericTrackersId.Add(i); break;
				case InputVRDevice.TYPE.INPUT_VR_HEAD: hmdId = i; break;
				case InputVRDevice.TYPE.INPUT_VR_BASE_STATION: trackingsId.Add(i); break;
				default: break;
			}
		}

		foreach (int i in controllersId)
		{
			InputVRController controller = Input.GetVRDevice(i) as InputVRController;

			if (!controller)
				continue;

			switch (controller.ControllerType)
			{
				case InputVRController.CONTROLLER_TYPE.HAND_LEFT:
					if (devicesId.ContainsKey(InputSystem.VRDevice.LEFT_CONTROLLER))
						devicesId[InputSystem.VRDevice.LEFT_CONTROLLER] = i;
					else
						devicesId.Add(InputSystem.VRDevice.LEFT_CONTROLLER, i);
					break;

				case InputVRController.CONTROLLER_TYPE.HAND_RIGHT:
					if (devicesId.ContainsKey(InputSystem.VRDevice.RIGHT_CONTROLLER))
						devicesId[InputSystem.VRDevice.RIGHT_CONTROLLER] = i;
					else
						devicesId.Add(InputSystem.VRDevice.RIGHT_CONTROLLER, i);
					break;

				case InputVRController.CONTROLLER_TYPE.TREADMILL:
					if (devicesId.ContainsKey(InputSystem.VRDevice.TREADMILL))
						devicesId[InputSystem.VRDevice.TREADMILL] = i;
					else
						devicesId.Add(InputSystem.VRDevice.TREADMILL, i);
					break;

				default: break;
			}
		}

		if (hmdId != -1)
			devicesId.Add(InputSystem.VRDevice.HMD, hmdId);

		if (trackingsId.Count > 0)
			devicesId.Add(InputSystem.VRDevice.BASESTATION_0, trackingsId[0]);

		if (trackingsId.Count > 1)
			devicesId.Add(InputSystem.VRDevice.BASESTATION_1, trackingsId[1]);

		if (trackingsId.Count > 2)
			devicesId.Add(InputSystem.VRDevice.BASESTATION_2, trackingsId[2]);

		if (trackingsId.Count > 3)
			devicesId.Add(InputSystem.VRDevice.BASESTATION_3, trackingsId[3]);

		if (genericTrackersId.Count > 0)
			devicesId.Add(InputSystem.VRDevice.GENERIC_TRACKER_0, genericTrackersId[0]);

		if (genericTrackersId.Count > 1)
			devicesId.Add(InputSystem.VRDevice.GENERIC_TRACKER_1, genericTrackersId[1]);

		if (genericTrackersId.Count > 2)
			devicesId.Add(InputSystem.VRDevice.GENERIC_TRACKER_2, genericTrackersId[2]);

		if (genericTrackersId.Count > 3)
			devicesId.Add(InputSystem.VRDevice.GENERIC_TRACKER_3, genericTrackersId[3]);
	}

	[MethodUpdate]
	private void ControllersDevicesUpdate()
	{
		if (!IsLoaded)
			return;

		if (devicesId.ContainsKey(InputSystem.VRDevice.LEFT_CONTROLLER) &&
			devicesId.ContainsKey(InputSystem.VRDevice.RIGHT_CONTROLLER))
			return;

		int count = Input.NumVRDevices;
		int controllersCount = 0;
		for (int i = 0; i < count; i++)
		{
			InputVRController controller = Input.GetVRDevice(i) as InputVRController;

			if (controller)
			{
				if (!controllersId.Contains(i))
					controllersId.Add(i);

				if (controllersCount == 0 && !devicesId.ContainsKey(InputSystem.VRDevice.LEFT_CONTROLLER))
					devicesId.Add(InputSystem.VRDevice.LEFT_CONTROLLER, i);
				else if (controllersCount == 1 && !devicesId.ContainsKey(InputSystem.VRDevice.RIGHT_CONTROLLER))
					devicesId.Add(InputSystem.VRDevice.RIGHT_CONTROLLER, i);

				controllersCount++;
			}
		}

		foreach (int i in controllersId)
		{
			InputVRController controller = Input.GetVRDevice(i) as InputVRController;

			if (!controller)
				continue;

			switch (controller.ControllerType)
			{
				case InputVRController.CONTROLLER_TYPE.HAND_LEFT:
					if (devicesId.ContainsKey(InputSystem.VRDevice.LEFT_CONTROLLER))
						devicesId[InputSystem.VRDevice.LEFT_CONTROLLER] = i;
					else
						devicesId.Add(InputSystem.VRDevice.LEFT_CONTROLLER, i);
					break;

				case InputVRController.CONTROLLER_TYPE.HAND_RIGHT:
					if (devicesId.ContainsKey(InputSystem.VRDevice.RIGHT_CONTROLLER))
						devicesId[InputSystem.VRDevice.RIGHT_CONTROLLER] = i;
					else
						devicesId.Add(InputSystem.VRDevice.RIGHT_CONTROLLER, i);
					break;

				default: break;
			}
		}
	}

	private void Update()
	{
		if (!IsLoaded)
			return;

		for (int i = 0; i < (int)Counts.BUTTONS; i++)
		{
			bool oldValue = buttonsPress[i];
			bool newValue = GetButton((BUTTONS)i);

			buttonsDown[i] = newValue && (newValue != oldValue);
			buttonsPress[i] = newValue;
			buttonsUp[i] = oldValue && (newValue != oldValue);
		}

		for (int i = 0; i < (int)Counts.BUTTONS; i++)
		{
			bool oldValue = touchesStationary[i];
			bool newValue = GetTouch((BUTTONS)i);

			touchesBegan[i] = newValue && (newValue != oldValue);
			touchesStationary[i] = newValue;
			touchesEnded[i] = oldValue && (newValue != oldValue);
		}

		for (AXES axis = AXES.JOYSTICK_VERTICAL; axis <= AXES.JOYSTICK_LEFT; axis++)
			UpdateAxisState(axis, JoystickPressedValue, JoystickReleaseValue);

		for (AXES axis = AXES.TRIGGER; axis <= AXES.TRIGGER_DOWN; axis++)
			UpdateAxisState(axis, TriggerPressedValue, TriggerReleaseValue);
	}

	static public bool IsButtonDown(BUTTONS button)
	{
		return (buttonsDown != null) ? buttonsDown[(int)button] : false;
	}

	static public bool IsButtonPress(BUTTONS button)
	{
		return (buttonsPress != null) ? buttonsPress[(int)button] : false;
	}

	static public bool IsButtonUp(BUTTONS button)
	{
		return (buttonsUp != null) ? buttonsUp[(int)button] : false;
	}

	static public bool IsTouchBegan(BUTTONS touch)
	{
		return (touchesBegan != null) ? touchesBegan[(int)touch] : false;
	}

	static public bool IsTouchStationary(BUTTONS touch)
	{
		return (touchesStationary != null) ? touchesStationary[(int)touch] : false;
	}

	static public bool IsTouchEnded(BUTTONS touch)
	{
		return (touchesEnded != null) ? touchesEnded[(int)touch] : false;
	}

	static public bool IsAxisDown(InputSystem.VRDevice device, AXES axis)
	{
		if (leftAxesDown == null || rightAxesDown == null)
			return false;

		switch (device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER: return leftAxesDown[(int)axis];
			case InputSystem.VRDevice.RIGHT_CONTROLLER: return rightAxesDown[(int)axis];
			default: return false;
		}
	}

	static public bool IsAxisPress(InputSystem.VRDevice device, AXES axis)
	{
		if (leftAxesPress == null || rightAxesPress == null)
			return false;

		switch (device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER: return leftAxesPress[(int)axis];
			case InputSystem.VRDevice.RIGHT_CONTROLLER: return rightAxesPress[(int)axis];
			default: return false;
		}
	}

	static public bool IsAxisUp(InputSystem.VRDevice device, AXES axis)
	{
		if (leftAxesUp == null || rightAxesUp == null)
			return false;

		switch (device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER: return leftAxesUp[(int)axis];
			case InputSystem.VRDevice.RIGHT_CONTROLLER: return rightAxesUp[(int)axis];
			default: return false;
		}
	}
	static public vec3 GetAngularVelocity(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return vec3.ZERO;

		if (!devicesId.ContainsKey(device))
			return vec3.ZERO;

		InputVRDevice vr_device = Input.GetVRDevice(devicesId[device]);
		if (!vr_device)
			return vec3.ZERO;

		return vr_device.AngularVelocity;
	}

	static public mat4 GetTransform(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return mat4.ZERO;

		if (!devicesId.ContainsKey(device))
			return mat4.ZERO;

		InputVRDevice vr_device = Input.GetVRDevice(devicesId[device]);
		if (!vr_device)
			return mat4.ZERO;

		return vr_device.Transform;
	}

	static public vec3 GetLinearVelocity(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return vec3.ZERO;

		if (!devicesId.ContainsKey(device))
			return vec3.ZERO;

		InputVRDevice vr_device = Input.GetVRDevice(devicesId[device]);
		if (!vr_device)
			return vec3.ZERO;

		return vr_device.LinearVelocity;
	}

	static public bool IsDeviceConnected(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return false;

		if (!devicesId.ContainsKey(device))
			return false;

		InputVRDevice vr_device = Input.GetVRDevice(devicesId[device]);
		if (!vr_device)
			return false;

		return vr_device.IsAvailable;
	}

	static public bool IsTransformValid(InputSystem.VRDevice device)
	{
		if (!IsLoaded)
			return false;

		if (!devicesId.ContainsKey(device))
			return false;

		InputVRDevice vr_device = Input.GetVRDevice(devicesId[device]);
		if (!vr_device)
			return false;

		return vr_device.IsTransformValid;
	}

	static public void SetControllerVibration(InputSystem.VRDevice device, ushort duration)
	{
		if (!IsLoaded)
			return;

		if (!devicesId.ContainsKey(device))
			return;

		InputVRController vr_controller = Input.GetVRDevice(devicesId[device]) as InputVRController;
		if (!vr_controller)
			return;

		vr_controller.SetVibration(duration);
	}

	static public bool GetButton(BUTTONS button)
	{
		if (!IsLoaded)
			return false;

		InputSystem.VRDevice device = (button < BUTTONS.RIGHT_A ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);
		if (!devicesId.ContainsKey(device))
			return false;

		InputVRController vr_controller = Input.GetVRDevice(devicesId[device]) as InputVRController;

		if (!vr_controller)
			return false;

		switch (button)
		{
			case BUTTONS.LEFT_A: return vr_controller.IsButtonPressed(Input.VR_BUTTON.X);
			case BUTTONS.RIGHT_A: return vr_controller.IsButtonPressed(Input.VR_BUTTON.A);
			case BUTTONS.LEFT_APPLICATIONMENU: return vr_controller.IsButtonPressed(Input.VR_BUTTON.Y);
			case BUTTONS.RIGHT_APPLICATIONMENU: return vr_controller.IsButtonPressed(Input.VR_BUTTON.B);
			case BUTTONS.LEFT_AXIS0: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_0_LEFT);
			case BUTTONS.RIGHT_AXIS0: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_0_RIGHT);
			case BUTTONS.LEFT_AXIS1: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_1_LEFT);
			case BUTTONS.RIGHT_AXIS1: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_1_RIGHT);
			case BUTTONS.LEFT_AXIS2: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_2_LEFT);
			case BUTTONS.RIGHT_AXIS2: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_2_RIGHT);
			case BUTTONS.LEFT_AXIS3: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_3_LEFT);
			case BUTTONS.RIGHT_AXIS3: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_3_RIGHT);
			case BUTTONS.LEFT_AXIS4: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_4_LEFT);
			case BUTTONS.RIGHT_AXIS4: return vr_controller.IsButtonPressed(Input.VR_BUTTON.AXIS_4_RIGHT);
			case BUTTONS.LEFT_DASHBOARD_BACK:
			case BUTTONS.RIGHT_DASHBOARD_BACK: return false;
			case BUTTONS.LEFT_DPAD_DOWN: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_DOWN);
			case BUTTONS.RIGHT_DPAD_DOWN: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_DOWN);
			case BUTTONS.LEFT_DPAD_LEFT: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_LEFT);
			case BUTTONS.RIGHT_DPAD_LEFT: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_LEFT);
			case BUTTONS.LEFT_DPAD_RIGHT: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_RIGHT);
			case BUTTONS.RIGHT_DPAD_RIGHT: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_RIGHT);
			case BUTTONS.LEFT_DPAD_UP: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_UP);
			case BUTTONS.RIGHT_DPAD_UP: return vr_controller.IsButtonPressed(Input.VR_BUTTON.DPAD_UP);
			case BUTTONS.LEFT_GRIP: return vr_controller.IsButtonPressed(Input.VR_BUTTON.GRIP_LEFT);
			case BUTTONS.RIGHT_GRIP: return vr_controller.IsButtonPressed(Input.VR_BUTTON.GRIP_RIGHT);
			case BUTTONS.LEFT_STEAMVR_TOUCHPAD:
			case BUTTONS.RIGHT_STEAMVR_TOUCHPAD:
			case BUTTONS.LEFT_STEAMVR_TRIGGER:
			case BUTTONS.RIGHT_STEAMVR_TRIGGER: return false;
			case BUTTONS.LEFT_SYSTEM: return vr_controller.IsButtonPressed(Input.VR_BUTTON.SYSTEM_LEFT);
			case BUTTONS.RIGHT_SYSTEM: return vr_controller.IsButtonPressed(Input.VR_BUTTON.SYSTEM_RIGHT);
			default: break;
		}

		return false;
	}

	static public bool GetTouch(BUTTONS button)
	{
		if (!IsLoaded)
			return false;

		InputSystem.VRDevice device = (button < BUTTONS.RIGHT_A ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);
		if (!devicesId.ContainsKey(device))
			return false;

		InputVRController vr_controller = Input.GetVRDevice(devicesId[device]) as InputVRController;

		if (!vr_controller)
			return false;

		switch (button)
		{
			case BUTTONS.LEFT_A: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.X);
			case BUTTONS.RIGHT_A: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.A);
			case BUTTONS.LEFT_APPLICATIONMENU: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.Y);
			case BUTTONS.RIGHT_APPLICATIONMENU: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.B);
			case BUTTONS.LEFT_AXIS0: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_0_LEFT);
			case BUTTONS.RIGHT_AXIS0: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_0_RIGHT);
			case BUTTONS.LEFT_AXIS1: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_1_LEFT);
			case BUTTONS.RIGHT_AXIS1: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_1_RIGHT);
			case BUTTONS.LEFT_AXIS2: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_2_LEFT);
			case BUTTONS.RIGHT_AXIS2: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_2_RIGHT);
			case BUTTONS.LEFT_AXIS3: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_3_LEFT);
			case BUTTONS.RIGHT_AXIS3: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_3_RIGHT);
			case BUTTONS.LEFT_AXIS4: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_4_LEFT);
			case BUTTONS.RIGHT_AXIS4: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.AXIS_4_RIGHT);
			case BUTTONS.LEFT_DASHBOARD_BACK:
			case BUTTONS.RIGHT_DASHBOARD_BACK: return false;
			case BUTTONS.LEFT_DPAD_DOWN: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_DOWN);
			case BUTTONS.RIGHT_DPAD_DOWN: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_DOWN);
			case BUTTONS.LEFT_DPAD_LEFT: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_LEFT);
			case BUTTONS.RIGHT_DPAD_LEFT: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_LEFT);
			case BUTTONS.LEFT_DPAD_RIGHT: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_RIGHT);
			case BUTTONS.RIGHT_DPAD_RIGHT: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_RIGHT);
			case BUTTONS.LEFT_DPAD_UP: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_UP);
			case BUTTONS.RIGHT_DPAD_UP: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.DPAD_UP);
			case BUTTONS.LEFT_GRIP: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.GRIP_LEFT);
			case BUTTONS.RIGHT_GRIP: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.GRIP_RIGHT);
			case BUTTONS.LEFT_STEAMVR_TOUCHPAD:
			case BUTTONS.RIGHT_STEAMVR_TOUCHPAD:
			case BUTTONS.LEFT_STEAMVR_TRIGGER:
			case BUTTONS.RIGHT_STEAMVR_TRIGGER: return false;
			case BUTTONS.LEFT_SYSTEM: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.SYSTEM_LEFT);
			case BUTTONS.RIGHT_SYSTEM: return vr_controller.IsButtonTouchPressed(Input.VR_BUTTON.SYSTEM_RIGHT);
			default: break;
		}

		return false;
	}

	static public float GetAxis(InputSystem.VRDevice device, AXES axis)
	{
		if (!IsLoaded)
			return 0.0f;

		if (!devicesId.ContainsKey(device))
			return 0.0f;

		InputVRController vr_controller = Input.GetVRDevice(devicesId[device]) as InputVRController;

		if (!vr_controller)
			return 0.0f;

		switch (axis)
		{
			case AXES.JOYSTICK_VERTICAL: return vr_controller.GetAxis(1);
			case AXES.JOYSTICK_HORIZONTAL: return vr_controller.GetAxis(0);
			case AXES.JOYSTICK_UP: return MathLib.Clamp(vr_controller.GetAxis(1), 0.0f, 1.0f);
			case AXES.JOYSTICK_RIGHT: return MathLib.Clamp(vr_controller.GetAxis(0), 0.0f, 1.0f);
			case AXES.JOYSTICK_DOWN: return MathLib.Clamp(-vr_controller.GetAxis(1), 0.0f, 1.0f);
			case AXES.JOYSTICK_LEFT: return MathLib.Clamp(-vr_controller.GetAxis(0), 0.0f, 1.0f);
			case AXES.TRIGGER: return vr_controller.GetAxis(2);
			case AXES.TRIGGER_UP: return MathLib.Clamp(vr_controller.GetAxis(2), 0.0f, 1.0f);
			case AXES.TRIGGER_DOWN: return MathLib.Clamp(-vr_controller.GetAxis(2), 0.0f, 1.0f);
			default: break;
		}

		return 0.0f;
	}

	public override mat4 GetTransformInterface(InputSystem.VRDevice device)
	{
		return GetTransform(device);
	}

	public override bool IsDeviceConnectedInterface(InputSystem.VRDevice device)
	{
		return IsDeviceConnected(device);
	}

	public override bool IsTransformValidInterface(InputSystem.VRDevice device)
	{
		return IsTransformValid(device);
	}

	public override vec3 GetLinearVelocityInterface(InputSystem.VRDevice device)
	{
		return GetLinearVelocity(device);
	}

	public override vec3 GetAngularVelocityInterface(InputSystem.VRDevice device)
	{
		return GetAngularVelocity(device);
	}

	public override void SetControllerVibrationInterface(InputSystem.VRDevice device, ushort duration, float amplitude)
	{
		if (!devicesId.ContainsKey(device))
			return;

		InputVRController vr_controller = Input.GetVRDevice(devicesId[device]) as InputVRController;

		if (!vr_controller)
			return;

		vr_controller.SetVibration(duration);
	}

	public override bool IsLoadedInterface => VR.ApiType != VR.API.NULL;

	private void UpdateAxisState(AXES axis, float pressedValue, float releaseValue)
	{
		bool newValue = GetAxis(InputSystem.VRDevice.LEFT_CONTROLLER, axis) > (leftAxesPress[(int)axis] ? releaseValue : pressedValue);
		bool oldValue = leftAxesPress[(int)axis];

		leftAxesDown[(int)axis] = newValue && (newValue != oldValue);
		leftAxesPress[(int)axis] = newValue;
		leftAxesUp[(int)axis] = oldValue && (newValue != oldValue);

		newValue = GetAxis(InputSystem.VRDevice.RIGHT_CONTROLLER, axis) > (rightAxesPress[(int)axis] ? releaseValue : pressedValue);
		oldValue = rightAxesPress[(int)axis];

		rightAxesDown[(int)axis] = newValue && (newValue != oldValue);
		rightAxesPress[(int)axis] = newValue;
		rightAxesUp[(int)axis] = oldValue && (newValue != oldValue);
	}
}
