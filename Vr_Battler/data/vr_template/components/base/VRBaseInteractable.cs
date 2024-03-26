using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "07186cadf0de66280e5613672797f99a2ca3fc31")]
public class VRBaseInteractable : Component
{
	static public event Action<VRBaseInteractable> onInit;

	public enum INTERACTABLE_STATE
	{
		NOT_INTERACT,
		HOVERED,
		GRABBED,
		USED
	}

	public INTERACTABLE_STATE CurrentState { get; set; }

	protected void Init()
	{
		onInit?.Invoke(this);
	}

	public virtual void OnHoverBegin(VRBaseInteraction interaction, VRBaseController controller) { }
	public virtual void OnHoverEnd(VRBaseInteraction interaction, VRBaseController controller) { }
	public virtual void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller) { }
	public virtual void OnGrabEnd(VRBaseInteraction interaction, VRBaseController controller) { }
	public virtual void OnUseBegin(VRBaseInteraction interaction, VRBaseController controller) { }
	public virtual void OnUseEnd(VRBaseInteraction interaction, VRBaseController controller) { }
}
