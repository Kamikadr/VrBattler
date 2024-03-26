using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unigine;

[Component(PropertyGuid = "4431a30d66306908f88578c7d4c45f7a9b5c6891")]
public class VRNodeSwitchEnableByKey : VRBaseInteractable
{
	[ShowInEditor]
	[ParameterSlider(Title = "Node To Switch", Group = "VR Node Switch Enabled By Key")]
	private Node[] nodesToSwitch = null;

	enum NodeState
	{
		Enabled = 0,
		Disabled = 1,
	}

	HandController controller = null;

	[ShowInEditor]
	[ParameterSlider(Title = "Node To Switch", Group = "VR Node Switch Enabled By Key")]
	private NodeState defaultNodesState = NodeState.Disabled;

	protected override void OnReady()
	{
		if (nodesToSwitch != null)
		{

			for (int i = 0; i < nodesToSwitch.Length; i++)
				if (nodesToSwitch[i] != null)
					nodesToSwitch[i].Enabled = defaultNodesState == NodeState.Enabled;

			controller = GetComponent<HandController>(node);
		}
	}

	private void Update()
	{
		if (controller == null)
			return;

		bool buttonDown = false;

		switch (controller.Device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER:
				buttonDown = InputSystem.IsLeftButtonDown(InputSystem.ControllerButtons.ACTION_BUTTON);
				break;
			case InputSystem.VRDevice.RIGHT_CONTROLLER:
				buttonDown = InputSystem.IsRightButtonDown(InputSystem.ControllerButtons.ACTION_BUTTON);
				break;
			case InputSystem.VRDevice.PC_HAND:
				buttonDown = InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.FIRE_4);
				break;

			default: break;
		}

		if(buttonDown)
			for (int i = 0; i < nodesToSwitch.Length; i++)
				if (nodesToSwitch[i] != null)
					nodesToSwitch[i].Enabled = !nodesToSwitch[i].Enabled;

	}
}
