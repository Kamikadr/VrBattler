using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "966f78f019b7b78faadd3fa247d7ba990fad6324")]
public class VRControllerInput : VRBaseControllerInput
{
	public enum BindType : byte
	{
		NONE,
		BUTTON,
		TOUCH,
		CLICK_ON_AXIS
	}

	public struct Bind
	{
		[Parameter(Title = "Bind Type")]
		public BindType bindType;

		[ParameterCondition(nameof(bindType), (int)BindType.BUTTON)]
		public VRInput.BUTTONS button;

		[ParameterCondition(nameof(bindType), (int)BindType.TOUCH)]
		public VRInput.BUTTONS touch;

		[Parameter(Title = "Clickable Axis")]
		[ParameterCondition(nameof(bindType), (int)BindType.CLICK_ON_AXIS)]
		public VRInput.AXES clickableAxis;

		public bool IsDown(HandSide side)
		{
			InputSystem.VRDevice device = ((side == HandSide.LEFT) ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);

			switch (bindType)
			{
				case BindType.BUTTON: return VRInput.IsButtonDown(button);
				case BindType.TOUCH: return VRInput.IsTouchBegan(touch);
				case BindType.CLICK_ON_AXIS: return VRInput.IsAxisDown(device, clickableAxis);
				default: return false;
			}
		}

		public bool IsPress(HandSide side)
		{
			InputSystem.VRDevice device = ((side == HandSide.LEFT) ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);

			switch (bindType)
			{
				case BindType.BUTTON: return VRInput.IsButtonPress(button);
				case BindType.TOUCH: return VRInput.IsTouchStationary(touch);
				case BindType.CLICK_ON_AXIS: return VRInput.IsAxisPress(device, clickableAxis);
				default: return false;
			}
		}

		public bool IsUp(HandSide side)
		{
			InputSystem.VRDevice device = ((side == HandSide.LEFT) ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);

			switch (bindType)
			{
				case BindType.BUTTON: return VRInput.IsButtonUp(button);
				case BindType.TOUCH: return VRInput.IsTouchEnded(touch);
				case BindType.CLICK_ON_AXIS: return VRInput.IsAxisUp(device, clickableAxis);
				default: return false;
			}
		}
	}

	public enum HandSide : byte
	{
		LEFT,
		RIGHT
	}

	[ShowInEditor]
	[Parameter(Group = "Bind")]
	private HandSide side = HandSide.LEFT;

	[ShowInEditor]
	[Parameter(Title = "Grab Button", Group = "Bind")]
	private Bind grabButton;

	[ShowInEditor]
	[Parameter(Title = "Use Button", Group = "Bind")]
	private Bind useButton;

	[ShowInEditor]
	[Parameter(Title = "Teleportation Button", Group = "Bind")]
	private Bind teleportationButton;

	[ShowInEditor]
	[Parameter(Title = "Action Button", Group = "Bind")]
	private Bind actionButton;

	[ShowInEditor]
	[Parameter(Title = "Trigger Axis", Group = "Bind")]
	private VRInput.AXES triggerAxis = VRInput.AXES.JOYSTICK_DOWN;

	[ShowInEditor]
	[Parameter(Title = "Grab Axis", Group = "Bind")]
	private VRInput.AXES grabAxis = VRInput.AXES.JOYSTICK_DOWN;

	public override bool IsButtonDown(InputSystem.ControllerButtons button)
	{
		switch (button)
		{
			case InputSystem.ControllerButtons.GRAB_BUTTON: return grabButton.IsDown(side);
			case InputSystem.ControllerButtons.TELEPORTATION_BUTTON: return teleportationButton.IsDown(side);
			case InputSystem.ControllerButtons.USE_BUTTON: return useButton.IsDown(side);
			case InputSystem.ControllerButtons.ACTION_BUTTON: return actionButton.IsDown(side);
			default: return false;
		}
	}

	public override bool IsButtonPress(InputSystem.ControllerButtons button)
	{
		switch (button)
		{
			case InputSystem.ControllerButtons.GRAB_BUTTON: return grabButton.IsPress(side);
			case InputSystem.ControllerButtons.TELEPORTATION_BUTTON: return teleportationButton.IsPress(side);
			case InputSystem.ControllerButtons.USE_BUTTON: return useButton.IsPress(side);
			case InputSystem.ControllerButtons.ACTION_BUTTON: return actionButton.IsPress(side);
			default: return false;
		}
	}

	public override bool IsButtonUp(InputSystem.ControllerButtons button)
	{
		switch (button)
		{
			case InputSystem.ControllerButtons.GRAB_BUTTON: return grabButton.IsUp(side);
			case InputSystem.ControllerButtons.TELEPORTATION_BUTTON: return teleportationButton.IsUp(side);
			case InputSystem.ControllerButtons.USE_BUTTON: return useButton.IsUp(side);
			case InputSystem.ControllerButtons.ACTION_BUTTON: return actionButton.IsUp(side);
			default: return false;
		}
	}

	public override float GetAxis(InputSystem.ControllerAxes axis)
	{
		InputSystem.VRDevice device = ((side == HandSide.LEFT) ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);

		switch (axis)
		{
			case InputSystem.ControllerAxes.GRAB_AXIS: return VRInput.GetAxis(device, grabAxis);
			case InputSystem.ControllerAxes.TRIGGER_AXIS: return VRInput.GetAxis(device, triggerAxis);
		}

		return 0.0f;
	}
}
