using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "841c0b473848cffa6a691f118ecf4d3ca097c76b")]
public class VRBaseInteraction : Component
{
	static public event Action<VRBaseInteraction> onInit;

	public virtual VRBaseController Controller => null;

	protected void Init()
	{
		onInit?.Invoke(this);
	}

	public virtual void Interact(VRInteractionManager.InteractablesState interactablesState, float ifps) { }

	public virtual void StopHover(VRInteractionManager.InteractablesState interactablesState) { }

	public virtual void StopGrab(VRInteractionManager.InteractablesState interactablesState) { }

	public virtual void StopUse(VRInteractionManager.InteractablesState interactablesState) { }
}
