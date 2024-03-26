using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c8055e32bb1ea7e959a550347cdcbe0315a4cb86")]
public class VRHeadController : HeadController
{
	protected override bool ControllerInit()
	{
		if (InputSystem.CurrentName.CompareTo("vr_input") != 0)
			node.Enabled = false;

		return (VRInput.IsLoaded) && node.Enabled;
	}
}
