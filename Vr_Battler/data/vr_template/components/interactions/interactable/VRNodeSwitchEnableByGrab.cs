using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "1f59f899f3fe89246f8442330f81a1f67d9230dd")]
public class VRNodeSwitchEnableByGrab : VRBaseInteractable
{
	[ShowInEditor]
	[ParameterSlider(Title = "Node To Switch", Group = "VR Node Switch Enabled By Grab")]
	private Node[] nodesToSwitch = null;

	enum NodeState
	{ 
		Enabled = 0,
		Disabled = 1,
	}

	[ShowInEditor]
	[ParameterSlider(Title = "Node To Switch", Group = "VR Node Switch Enabled By Grab")]
	private NodeState defaultNodesState = NodeState.Disabled;

	protected override void OnReady()
	{
		for (int i = 0; i < nodesToSwitch.Length; i++)
			if (nodesToSwitch[i] != null)
				nodesToSwitch[i].Enabled = defaultNodesState == NodeState.Enabled;
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		for (int i = 0; i < nodesToSwitch.Length; i++)
			if (nodesToSwitch[i] != null)
				nodesToSwitch[i].Enabled = !nodesToSwitch[i].Enabled;
	}
}
