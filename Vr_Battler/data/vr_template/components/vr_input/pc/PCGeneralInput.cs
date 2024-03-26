using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "142610410330acaca780401b1a1ff6be64ec2f05")]
public class PCGeneralInput : VRBaseGeneralInput
{
	public enum BindType : byte
	{
		NONE = 0,
		KEY,
		MOUSE_BUTTON,
	}

	public struct ButtonBind
	{
		[Parameter(Title = "Bind Type")]
		public BindType bindType;

		[ParameterCondition(nameof(bindType), (int)BindType.KEY)]
		public PCInput.KEYS key;

		[ParameterCondition(nameof(bindType), (int)BindType.MOUSE_BUTTON)]
		public PCInput.MOUSE_BUTTONS button;

		public bool IsDown()
		{
			switch (bindType)
			{
				case BindType.KEY: return PCInput.IsKeyDown(key);
				case BindType.MOUSE_BUTTON: return PCInput.IsMouseButtonDown(button);
				default: return false;
			}
		}

		public bool IsPress()
		{
			switch (bindType)
			{
				case BindType.KEY: return PCInput.IsKeyPress(key);
				case BindType.MOUSE_BUTTON: return PCInput.IsMouseButtonPress(button);
				default: return false;
			}
		}

		public bool IsUp()
		{
			switch (bindType)
			{
				case BindType.KEY: return PCInput.IsKeyUp(key);
				case BindType.MOUSE_BUTTON: return PCInput.IsMouseButtonUp(button);
				default: return false;
			}
		}
	}

	public struct AxisBind
	{
		public PCInput.MOUSE_WHEEL_AXES axis;

		public float getValue()
		{
			return PCInput.GetAxis(axis);
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
