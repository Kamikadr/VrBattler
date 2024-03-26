using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "9de1ce4f5ed0e762e1cdbf595dd020e4235f76f3")]
public class PCHeadController : HeadController
{ 
	protected override bool ControllerInit()
	{
		if (InputSystem.CurrentName.CompareTo("pc_input") != 0)
			node.Enabled = false;

		return PCInput.IsLoaded && node.Enabled;
	}
}
