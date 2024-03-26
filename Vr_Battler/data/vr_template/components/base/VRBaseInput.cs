using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c42575c19f19d42c5394419bf53d0a3e02ab2c85")]
public class VRBaseInput : Component
{
	[ShowInEditor]
	[Parameter(Title = "Input Name", Group = "Name")]
	private string inputName = "";

	[ShowInEditor]
	[Parameter(Title = "Left Controller Presets", Group = "Input Presets")]
	private List<VRBaseControllerInput> leftControllerPresets = null;

	[ShowInEditor]
	[Parameter(Title = "Right Controller Presets", Group = "Input Presets")]
	private List<VRBaseControllerInput> rightControllerPresets = null;

	[ShowInEditor]
	[Parameter(Title = "General Presets", Group = "Input Presets")]
	private List<VRBaseGeneralInput> generalPresets = null;

	private InputSystem.InputPresets currentLeftPreset = InputSystem.InputPresets.PRESET_0;
	private InputSystem.InputPresets currentRightPreset = InputSystem.InputPresets.PRESET_0;
	private InputSystem.InputPresets currentGeneralPreset = InputSystem.InputPresets.PRESET_0;

	public InputSystem.InputPresets CurrentLeftPreset
	{
		get { return currentLeftPreset; }
		set
		{
			if ((int)value < 0 || leftControllerPresets.Count <= (int)value)
				return;

			currentLeftPreset = value;
		}
	}

	public InputSystem.InputPresets CurrentRightPreset
	{
		get { return currentRightPreset; }
		set
		{
			if ((int)value < 0 || rightControllerPresets.Count <= (int)value)
				return;

			currentRightPreset = value;
		}
	}

	public InputSystem.InputPresets CurrentGeneralPreset
	{
		get { return currentGeneralPreset; }
		set
		{
			if ((int)value < 0 || generalPresets.Count <= (int)value)
				return;

			currentGeneralPreset = value;
		}
	}

	public string Name => inputName;

	public virtual bool HeadPositionLockInterface { get; set; }

	public virtual bool HeadRotationLockInterface { get; set; }

	public virtual InputSystem.Viewport ViewportModeInterface { get; set; }

	public bool IsLeftButtonDown(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentLeftPreset;
		if (preset < 0 || leftControllerPresets == null || leftControllerPresets.Count <= preset)
			return false;

		return leftControllerPresets[preset].IsButtonDown(button);
	}

	public bool IsLeftButtonPress(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentLeftPreset;
		if (preset < 0 || leftControllerPresets == null || leftControllerPresets.Count <= preset)
			return false;

		return leftControllerPresets[preset].IsButtonPress(button);
	}

	public bool IsLeftButtonUp(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentLeftPreset;
		if (preset < 0 || leftControllerPresets == null || leftControllerPresets.Count <= preset)
			return false;

		return leftControllerPresets[preset].IsButtonUp(button);
	}

	public float GetLeftAxis(InputSystem.ControllerAxes axis)
	{
		int preset = (int)currentLeftPreset;
		if (preset < 0 || leftControllerPresets == null || leftControllerPresets.Count <= preset)
			return 0.0f;

		return leftControllerPresets[preset].GetAxis(axis);
	}

	public bool IsRightButtonDown(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentRightPreset;
		if (preset < 0 || rightControllerPresets == null || rightControllerPresets.Count <= preset)
			return false;

		return rightControllerPresets[preset].IsButtonDown(button);
	}

	public bool IsRightButtonPress(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentRightPreset;
		if (preset < 0 || rightControllerPresets == null || rightControllerPresets.Count <= preset)
			return false;

		return rightControllerPresets[preset].IsButtonPress(button);
	}

	public bool IsRightButtonUp(InputSystem.ControllerButtons button)
	{
		int preset = (int)currentRightPreset;
		if (preset < 0 || rightControllerPresets == null || rightControllerPresets.Count <= preset)
			return false;

		return rightControllerPresets[preset].IsButtonUp(button);
	}

	public float GetRightAxis(InputSystem.ControllerAxes axis)
	{
		int preset = (int)currentRightPreset;
		if (preset < 0 || rightControllerPresets == null || rightControllerPresets.Count <= preset)
			return 0.0f;

		return rightControllerPresets[preset].GetAxis(axis);
	}

	public bool IsGeneralButtonDown(InputSystem.GeneralButtons button)
	{
		int preset = (int)currentGeneralPreset;
		if (preset < 0 || generalPresets == null || generalPresets.Count <= preset)
			return false;

		return generalPresets[preset].IsButtonDown(button);
	}

	public bool IsGeneralButtonPress(InputSystem.GeneralButtons button)
	{
		int preset = (int)currentGeneralPreset;
		if (preset < 0 || generalPresets == null || generalPresets.Count <= preset)
			return false;

		return generalPresets[preset].IsButtonPress(button);
	}

	public bool IsGeneralButtonUp(InputSystem.GeneralButtons button)
	{
		int preset = (int)currentGeneralPreset;
		if (preset < 0 || generalPresets == null || generalPresets.Count <= preset)
			return false;

		return generalPresets[preset].IsButtonUp(button);
	}

	public VRBaseGeneralInput.HandSide GetGeneralAxisSide(InputSystem.GeneralAxes axis)
	{
		int preset = (int)currentGeneralPreset;
		if (preset < 0 || generalPresets == null || generalPresets.Count <= preset)
			return VRBaseGeneralInput.HandSide.LEFT;

		return generalPresets[preset].GetAxisSide(axis);
	}

	public float GetGeneralAxis(InputSystem.GeneralAxes axis)
	{
		int preset = (int)currentGeneralPreset;
		if (preset < 0 || generalPresets == null || generalPresets.Count <= preset)
			return 0.0f;

		return generalPresets[preset].GetAxis(axis);
	}

	public virtual mat4 GetTransformInterface(InputSystem.VRDevice device) { return mat4.IDENTITY; }

	public virtual bool IsDeviceConnectedInterface(InputSystem.VRDevice device) { return false; }

	public virtual bool IsTransformValidInterface(InputSystem.VRDevice device) { return false; }

	public virtual vec3 GetLinearVelocityInterface(InputSystem.VRDevice device) { return vec3.ZERO; }

	public virtual vec3 GetAngularVelocityInterface(InputSystem.VRDevice device) { return vec3.ZERO; }

	public virtual void SetControllerVibrationInterface(InputSystem.VRDevice device, ushort duration, float amplitude) { }

	public virtual bool IsLoadedInterface => false;
}
