using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "8620ca97e590670a7dcc2ae128dbc6cc4ab953bb")]
public class VRBaseMovement : Component
{
	public virtual void Move(VRPlayer player, float ifps) { }
}
