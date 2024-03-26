using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "aaeb341eb8128ce5bbe05900db49785f6666ea32")]
public class TurnMovement : VRBaseMovement
{
	public enum TURN_MODE
	{
		BUTTON_CLICK,
		BUTTON_PRESS
	}

	[ShowInEditor]
	[ParameterSlider(Title = "Turn Step", Group = "Turn", Min = 0.0f, Max = 360.0f)]
	private float turnStep = 15.0f;

	[ShowInEditor]
	[Parameter(Title = "Turn Mode", Group = "Turn")]
	private TURN_MODE turnMode = TURN_MODE.BUTTON_CLICK;

	[ShowInEditor]
	[ParameterSlider(Title = "Turn Delay", Group = "Turn", Min = 0.0f)]
	[ParameterCondition(nameof(turnMode), (int)TURN_MODE.BUTTON_PRESS)]
	private float turnDelay = 0.25f;

	public float TurnStep
	{
		get { return turnStep; }
		set { turnStep = MathLib.Clamp(value, 0.0f, 360.0f); }
	}

	public TURN_MODE TurnMode
	{
		get { return turnMode; }
		set { turnMode = value; }
	}

	public float TurnDelay
	{
		get { return turnDelay; }
		set { turnDelay = MathLib.Abs(value); }
	}

	private float lastTurnTime = 0.0f;

	private void Init()
	{
		lastTurnTime = Game.Time;
	}

	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null || player.LeftController == null && player.RightController == null)
			return;

		if (TurnMode == TURN_MODE.BUTTON_CLICK)
		{
			if (InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.LEFT))
				player.Turn(TurnStep);

			if (InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.RIGHT))
				player.Turn(-TurnStep);
		}
		else if (TurnMode == TURN_MODE.BUTTON_PRESS)
		{
			if (Game.Time - lastTurnTime > TurnDelay)
			{
				if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.LEFT))
				{
					player.Turn(TurnStep);
					lastTurnTime = Game.Time;
				}

				if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.RIGHT))
				{
					player.Turn(-TurnStep);
					lastTurnTime = Game.Time;
				}
			}
		}
	}
}
