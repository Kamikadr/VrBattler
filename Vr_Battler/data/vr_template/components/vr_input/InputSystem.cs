#region Math Variables
#if UNIGINE_DOUBLE
	using Scalar = System.Double;
	using Vec2 = Unigine.dvec2;
	using Vec3 = Unigine.dvec3;
	using Vec4 = Unigine.dvec4;
	using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using Unigine.Plugins;

[Component(PropertyGuid = "53961ef6516a89dd485b019853889817c136d68a")]
public class InputSystem : Component
{
	public enum InputPresets : byte
	{
		PRESET_0 = 0,
		PRESET_1,
		PRESET_2,
		PRESET_3,
		PRESET_4,
		PRESET_5,
		PRESET_6,
		PRESET_7,
		PRESET_8,
		PRESET_9
	}

	public enum VRDevice : byte
	{
		LEFT_CONTROLLER,
		RIGHT_CONTROLLER,
		HMD,
		BASESTATION_0,
		BASESTATION_1,
		BASESTATION_2,
		BASESTATION_3,
		GENERIC_TRACKER_0,
		GENERIC_TRACKER_1,
		GENERIC_TRACKER_2,
		GENERIC_TRACKER_3,
		TREADMILL,
		PC_HEAD,
		PC_HAND,
	}

	public enum ControllerButtons : byte
	{
		GRAB_BUTTON,
		USE_BUTTON,
		TELEPORTATION_BUTTON,
		ACTION_BUTTON,
	}

	public enum ControllerAxes : byte
	{
		GRAB_AXIS,
		TRIGGER_AXIS
	}

	public enum GeneralButtons : byte
	{
		FIRE_1,
		FIRE_2,
		FIRE_3,
		FIRE_4,
		JUMP,
		CROUCH,
		SUBMIT,
		ADDITIONAL_SUBMIT,
		CANCEL,
		ADDITIONAL_CANCEL,
		UP,
		RIGHT,
		DOWN,
		LEFT,
		ADDITIONAL_UP,
		ADDITIONAL_RIGHT,
		ADDITIONAL_DOWN,
		ADDITIONAL_LEFT
	}

	public enum Viewport
	{
		NONE,
		BLACK_SCREEN,
		MONO,
		STEREO,
		STEREO_FINAL
	}

	public enum GeneralAxes : byte
	{
		MAIN_HORIZONTAL,
		MAIN_VERTICAL,
		ADDITIONAL_HORIZONTAL,
		ADDITIONAL_VERTICAL
	}

	public enum Counts : byte
	{
		ControllerButtons = 3,
		ControllerAxes = 2,
		GeneralButtons = 18,
		GeneralAxes = 4
	}

	static public string CurrentName
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return "NONE";

			return instance.inputs[instance.currentInput].Name;
		}
	}

	static public VRBaseInput Current
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return null;

			return instance.inputs[instance.currentInput];
		}
	}

	static public Viewport ViewportMode
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return Viewport.NONE;

			return instance.inputs[instance.currentInput].ViewportModeInterface;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].ViewportModeInterface = value;
		}
	}

	static public bool HeadPositionLock
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return false;

			return instance.inputs[instance.currentInput].HeadPositionLockInterface;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].HeadPositionLockInterface = value;
		}
	}

	static public bool HeadRotationLock
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return false;

			return instance.inputs[instance.currentInput].HeadRotationLockInterface;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].HeadRotationLockInterface = value;
		}
	}

	static public InputPresets CurrentLeftPreset
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return InputPresets.PRESET_0;

			return instance.inputs[instance.currentInput].CurrentLeftPreset;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].CurrentLeftPreset = value;
		}
	}

	static public InputPresets CurrentRightPreset
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return InputPresets.PRESET_0;

			return instance.inputs[instance.currentInput].CurrentRightPreset;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].CurrentRightPreset = value;
		}
	}

	static public InputPresets CurrentGeneralPreset
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return InputPresets.PRESET_0;

			return instance.inputs[instance.currentInput].CurrentGeneralPreset;
		}

		set
		{
			if (instance == null || instance.currentInput == -1)
				return;

			instance.inputs[instance.currentInput].CurrentGeneralPreset = value;
		}
	}

	[ShowInEditor]
	[Parameter(Title = "Inputs", Group = "Inputs")]
	private List<VRBaseInput> inputs = null;

	private int currentInput = 0;

	static private InputSystem instance = null;

	public InputSystem() : base()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	protected override void OnReady()
	{
		if (inputs == null)
			return;

		for (int i = 0; i < inputs.Count; i++)
			if (inputs[i] != null && inputs[i].IsLoadedInterface)
				currentInput = i;

		if (currentInput == 0)
			Input.MouseHandle = Input.MOUSE_HANDLE.GRAB;
	}

	static public bool IsLeftButtonDown(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsLeftButtonDown(button);
	}

	static public bool IsLeftButtonPress(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsLeftButtonPress(button);
	}

	static public bool IsLeftButtonUp(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsLeftButtonUp(button);
	}

	static public float GetLeftAxis(ControllerAxes axis)
	{
		if (instance == null || instance.currentInput == -1)
			return 0.0f;

		return instance.inputs[instance.currentInput].GetLeftAxis(axis);
	}

	static public bool IsRightButtonDown(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsRightButtonDown(button);
	}

	static public bool IsRightButtonPress(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsRightButtonPress(button);
	}

	static public bool IsRightButtonUp(ControllerButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsRightButtonUp(button);
	}

	static public float GetRightAxis(ControllerAxes axis)
	{
		if (instance == null || instance.currentInput == -1)
			return 0.0f;

		return instance.inputs[instance.currentInput].GetRightAxis(axis);
	}

	static public bool IsGeneralButtonDown(GeneralButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsGeneralButtonDown(button);
	}

	static public bool IsGeneralButtonPress(GeneralButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsGeneralButtonPress(button);
	}

	static public bool IsGeneralButtonUp(GeneralButtons button)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsGeneralButtonUp(button);
	}

	static public VRBaseGeneralInput.HandSide GetGeneralAxisSide(GeneralAxes axis)
	{
		if (instance == null || instance.currentInput == -1)
			return VRBaseGeneralInput.HandSide.LEFT;

		return instance.inputs[instance.currentInput].GetGeneralAxisSide(axis);
	}

	static public float GetGeneralAxis(GeneralAxes axis)
	{
		if (instance == null || instance.currentInput == -1)
			return 0.0f;

		return instance.inputs[instance.currentInput].GetGeneralAxis(axis);
	}

	static public mat4 GetTransform(VRDevice device)
	{
		if (instance == null || instance.currentInput == -1)
			return mat4.IDENTITY;

		return instance.inputs[instance.currentInput].GetTransformInterface(device);
	}

	static public bool IsDeviceConnected(VRDevice device)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsDeviceConnectedInterface(device);
	}

	static public bool IsTransformValid(VRDevice device)
	{
		if (instance == null || instance.currentInput == -1)
			return false;

		return instance.inputs[instance.currentInput].IsTransformValidInterface(device);
	}

	static public vec3 GetLinearVelocity(VRDevice device)
	{
		if (instance == null || instance.currentInput == -1)
			return vec3.ZERO;

		return instance.inputs[instance.currentInput].GetLinearVelocityInterface(device);
	}

	static public vec3 GetAngularVelocity(VRDevice device)
	{
		if (instance == null || instance.currentInput == -1)
			return vec3.ZERO;

		return instance.inputs[instance.currentInput].GetAngularVelocityInterface(device);
	}

	static public void SetControllerVibration(VRDevice device, ushort duration, float amplitude)
	{
		if (instance == null || instance.currentInput == -1)
			return;

		instance.inputs[instance.currentInput].SetControllerVibrationInterface(device, duration, amplitude);
	}

	static public bool IsLoaded
	{
		get
		{
			if (instance == null || instance.currentInput == -1)
				return false;

			return instance.inputs[instance.currentInput].IsLoadedInterface;
		}
	}
}
