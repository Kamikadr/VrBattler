using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "7a92b22a4ef8bb81a95edeec681e4d5b26ae22fe")]
public class VRBaseControllerInput : Component
{
	public virtual bool IsButtonDown(InputSystem.ControllerButtons button) { return false; }

	public virtual bool IsButtonPress(InputSystem.ControllerButtons button) { return false; }

	public virtual bool IsButtonUp(InputSystem.ControllerButtons button) { return false; }

	public virtual float GetAxis(InputSystem.ControllerAxes axis) { return 0.0f; }
}
