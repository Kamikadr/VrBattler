using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ebec5202624de979123b04841e1ec6135ec59456")]
public class VRGeneralInput : VRBaseGeneralInput
{
	public enum BindType : byte
	{
		NONE,
		BUTTON,
		TOUCH,
		CLICK_ON_AXIS
	}

	public struct ButtonBind
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

		[ParameterCondition(nameof(bindType), (int)BindType.CLICK_ON_AXIS)]
		public HandSide side;

		public bool IsDown()
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

		public bool IsPress()
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

		public bool IsUp()
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

	public struct AxisBind
	{
		public HandSide side;
		public VRInput.AXES axis;

		public float getValue()
		{
			InputSystem.VRDevice device = ((side == HandSide.LEFT) ? InputSystem.VRDevice.LEFT_CONTROLLER : InputSystem.VRDevice.RIGHT_CONTROLLER);
			return VRInput.GetAxis(device, axis);
		}
	}

	[ShowInEditor]
	[Parameter(Title = "Horizontal Axis", Group = "Bind")]
	private AxisBind horizontalAxis = new AxisBind();

	[ShowInEditor]
	[Parameter(Title = "Vertical Axis", Group = "Bind")]
	private AxisBind verticalAxis = new AxisBind();

	[ShowInEditor]
	[Parameter(Title = "Up", Group = "Bind")]
	private ButtonBind upButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Right", Group = "Bind")]
	private ButtonBind rightButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Down", Group = "Bind")]
	private ButtonBind downButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Left", Group = "Bind")]
	private ButtonBind leftButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Fire 1", Group = "Bind")]
	private ButtonBind fire1Button = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Fire 2", Group = "Bind")]
	private ButtonBind fire2Button = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Fire 3", Group = "Bind")]
	private ButtonBind fire3Button = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Fire 4", Group = "Bind")]
	private ButtonBind fire4Button = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Jump", Group = "Bind")]
	private ButtonBind jumpButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Crouch", Group = "Bind")]
	private ButtonBind crouchButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Submit", Group = "Bind")]
	private ButtonBind submitButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Cancel", Group = "Bind")]
	private ButtonBind cancelButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Horizontal", Group = "Bind")]
	private AxisBind additionalHorizontalAxis = new AxisBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Vertical", Group = "Bind")]
	private AxisBind additionalVerticalAxis = new AxisBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Up", Group = "Bind")]
	private ButtonBind additionalUpButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Right", Group = "Bind")]
	private ButtonBind additionalRightButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Down", Group = "Bind")]
	private ButtonBind additionalDownButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Left", Group = "Bind")]
	private ButtonBind additionalLeftButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Submit", Group = "Bind")]
	private ButtonBind additionalSubmitButton = new ButtonBind();

	[ShowInEditor]
	[Parameter(Title = "Additional Cancel", Group = "Bind")]
	private ButtonBind additionalCancelButton = new ButtonBind();

	private ButtonBind[] buttonBinds = null;

	private void Init()
	{
		buttonBinds = new ButtonBind[(int)InputSystem.Counts.GeneralButtons];
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_CANCEL] = additionalCancelButton;
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_DOWN] = additionalDownButton;
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_LEFT] = additionalLeftButton;
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_RIGHT] = additionalRightButton;
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_SUBMIT] = additionalSubmitButton;
		buttonBinds[(int)InputSystem.GeneralButtons.ADDITIONAL_UP] = additionalUpButton;
		buttonBinds[(int)InputSystem.GeneralButtons.FIRE_1] = fire1Button;
		buttonBinds[(int)InputSystem.GeneralButtons.FIRE_2] = fire2Button;
		buttonBinds[(int)InputSystem.GeneralButtons.FIRE_3] = fire3Button;
		buttonBinds[(int)InputSystem.GeneralButtons.FIRE_4] = fire4Button;
		buttonBinds[(int)InputSystem.GeneralButtons.JUMP] = jumpButton;
		buttonBinds[(int)InputSystem.GeneralButtons.CROUCH] = crouchButton;
		buttonBinds[(int)InputSystem.GeneralButtons.CANCEL] = cancelButton;
		buttonBinds[(int)InputSystem.GeneralButtons.DOWN] = downButton;
		buttonBinds[(int)InputSystem.GeneralButtons.LEFT] = leftButton;
		buttonBinds[(int)InputSystem.GeneralButtons.RIGHT] = rightButton;
		buttonBinds[(int)InputSystem.GeneralButtons.SUBMIT] = submitButton;
		buttonBinds[(int)InputSystem.GeneralButtons.UP] = upButton;
	}

	public override bool IsButtonDown(InputSystem.GeneralButtons button)
	{
		return buttonBinds[(int)button].IsDown();
	}

	public override bool IsButtonPress(InputSystem.GeneralButtons button)
	{
		return buttonBinds[(int)button].IsPress();
	}

	public override bool IsButtonUp(InputSystem.GeneralButtons button)
	{
		return buttonBinds[(int)button].IsUp();
	}

	public override HandSide GetAxisSide(InputSystem.GeneralAxes axis)
	{
		switch (axis)
		{
			case InputSystem.GeneralAxes.ADDITIONAL_HORIZONTAL: return additionalHorizontalAxis.side;
			case InputSystem.GeneralAxes.ADDITIONAL_VERTICAL: return additionalVerticalAxis.side;
			case InputSystem.GeneralAxes.MAIN_HORIZONTAL: return horizontalAxis.side;
			case InputSystem.GeneralAxes.MAIN_VERTICAL: return verticalAxis.side;
			default: return 0.0f;
		}
	}

	public override float GetAxis(InputSystem.GeneralAxes axis)
	{
		switch (axis)
		{
			case InputSystem.GeneralAxes.ADDITIONAL_HORIZONTAL: return additionalHorizontalAxis.getValue();
			case InputSystem.GeneralAxes.ADDITIONAL_VERTICAL: return additionalVerticalAxis.getValue();
			case InputSystem.GeneralAxes.MAIN_HORIZONTAL: return horizontalAxis.getValue();
			case InputSystem.GeneralAxes.MAIN_VERTICAL: return verticalAxis.getValue();
			default: return 0.0f;
		}
	}
}
