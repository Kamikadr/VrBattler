using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "2fe33f7b69472e9e7f0d4565c09eb7802d1b44df")]
public class VRBaseGeneralInput : Component
{
	public enum HandSide : byte
	{
		LEFT,
		RIGHT
	}

	public virtual bool IsButtonDown(InputSystem.GeneralButtons button) { return false; }

	public virtual bool IsButtonPress(InputSystem.GeneralButtons button) { return false; }

	public virtual bool IsButtonUp(InputSystem.GeneralButtons button) { return false; }

	public virtual HandSide GetAxisSide(InputSystem.GeneralAxes axis) { return HandSide.LEFT; }

	public virtual float GetAxis(InputSystem.GeneralAxes axis) { return 0.0f; }
}
