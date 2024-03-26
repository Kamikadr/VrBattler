using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "ecebe2be654f2b05201520a078215b7f8ffc32d2")]
public class VRInteractionManager : Component
{
	[ShowInEditor]
	[ParameterMask(Title = "Interaction Collision Mask", Group = "VR Interaction Manager", MaskType = ParameterMaskAttribute.TYPE.COLLISION)]
	private int interactionCollisionMask = 0B00000010;

	[ShowInEditor]
	[ParameterMask(Title = "Grab Collision Mask", Group = "VR Interaction Manager", MaskType = ParameterMaskAttribute.TYPE.COLLISION)]
	private int grabCollisionMask = 0B00000010;

	public static int InteractionCollisionMask { get; private set; }

	public static int GrabCollisionMask { get; private set; }

	static private VRInteractionManager instance = null;

	private bool isInit = false;

	public VRInteractionManager() : base()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public class InteractablesState
	{
		private enum INTERACTABLE_MASK : byte
		{
			HOVERED = 0B00000001,
			GRABBED = 0B00000010,
			USED = 0B00000100
		}

		private class Info
		{
			public byte state = 0;
			public VRBaseInteraction hoverInteraction = null;
			public VRBaseInteraction grabInteraction = null;
			public VRBaseInteraction useInteraction = null;
		}

		private Dictionary<VRBaseInteractable, Info> interactables = null;

		public InteractablesState()
		{
			interactables = new Dictionary<VRBaseInteractable, Info>();
		}

		public bool Contains(VRBaseInteractable obj)
		{
			if (obj == null)
				return false;

			return interactables.ContainsKey(obj);
		}

		public void AddObject(VRBaseInteractable obj)
		{
			if (obj != null && !interactables.ContainsKey(obj))
				interactables.Add(obj, new Info());
		}

		public void RemoveObject(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				interactables.Remove(obj);
		}

		public void SetHovered(VRBaseInteractable obj, bool isHovered, VRBaseInteraction interaction)
		{
			if (obj != null && interactables.ContainsKey(obj))
			{
				if (isHovered)
				{
					interactables[obj].state |= (byte)INTERACTABLE_MASK.HOVERED;
					interactables[obj].hoverInteraction = interaction;
				}
				else
				{
					interactables[obj].state &= (byte)(~INTERACTABLE_MASK.HOVERED);
					interactables[obj].hoverInteraction = null;
				}
			}
		}

		public void SetGrabbed(VRBaseInteractable obj, bool isGrabbed, VRBaseInteraction interaction)
		{
			if (obj != null && interactables.ContainsKey(obj))
			{
				if (isGrabbed)
				{
					interactables[obj].state |= (byte)INTERACTABLE_MASK.GRABBED;
					interactables[obj].grabInteraction = interaction;
				}
				else
				{
					interactables[obj].state &= (byte)(~INTERACTABLE_MASK.GRABBED);
					interactables[obj].grabInteraction = null;
				}
			}
		}

		public void SetUsed(VRBaseInteractable obj, bool isUsed, VRBaseInteraction interaction)
		{
			if (obj != null && interactables.ContainsKey(obj))
			{
				if (isUsed)
				{
					interactables[obj].state |= (byte)INTERACTABLE_MASK.USED;
					interactables[obj].useInteraction = interaction;
				}
				else
				{
					interactables[obj].state &= (byte)(~INTERACTABLE_MASK.USED);
					interactables[obj].useInteraction = null;
				}
			}
		}

		public bool IsInteract(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return interactables[obj].state != 0;

			return false;
		}

		public bool IsHovered(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return (interactables[obj].state & (byte)INTERACTABLE_MASK.HOVERED) != 0;

			return false;
		}

		public bool IsGrabbed(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return (interactables[obj].state & (byte)INTERACTABLE_MASK.GRABBED) != 0;

			return false;
		}

		public bool IsUsed(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return (interactables[obj].state & (byte)INTERACTABLE_MASK.USED) != 0;

			return false;
		}

		public VRBaseInteraction GetHoverInteraction(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return interactables[obj].hoverInteraction;

			return null;
		}

		public VRBaseInteraction GetGrabInteraction(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return interactables[obj].grabInteraction;

			return null;
		}

		public VRBaseInteraction GetUseInteraction(VRBaseInteractable obj)
		{
			if (obj != null && interactables.ContainsKey(obj))
				return interactables[obj].useInteraction;

			return null;
		}

	}

	private List<VRBaseInteraction> interactions = null;
	private InteractablesState interactablesState = null;

	private VRBaseInteractable firstObject = null;

	protected override void OnEnable()
	{
		if (!isInit)
			return;

		VRBaseInteraction.onInit += OnInteractionInit;
		VRBaseInteractable.onInit += OnInteractableInit;
	}

	protected override void OnDisable()
	{
		VRBaseInteraction.onInit -= OnInteractionInit;
		VRBaseInteractable.onInit -= OnInteractableInit;
	}

	protected override void OnReady()
	{
		interactions = new List<VRBaseInteraction>();
		interactablesState = new InteractablesState();

		VRBaseInteraction.onInit += OnInteractionInit;
		VRBaseInteractable.onInit += OnInteractableInit;

		InteractionCollisionMask = interactionCollisionMask;
		GrabCollisionMask = grabCollisionMask;

		isInit = true;
	}

	// must be called after all transformation have been applied
	[MethodUpdate(Order = 2)]
	private void Update()
	{
		float ifps = Game.IFps;

		foreach (var interaction in interactions)
			if (interaction.Enabled)
				interaction.Interact(interactablesState, ifps);
	}

	private void Shutdown()
	{
		VRBaseInteraction.onInit -= OnInteractionInit;
		VRBaseInteractable.onInit -= OnInteractableInit;

		instance = null;
	}

	private void OnInteractionInit(VRBaseInteraction interaction)
	{
		interactions.Add(interaction);
	}

	private void OnInteractableInit(VRBaseInteractable interactable)
	{
		if (firstObject == null)
			firstObject = interactable;

		interactablesState.AddObject(interactable);
	}

	private void StopHoverInternal(VRBaseInteraction interaction)
	{
		if (interaction != null)
			interaction.StopHover(interactablesState);
	}

	private void StopGrabInternal(VRBaseInteraction interaction)
	{
		if (interaction != null)
			interaction.StopGrab(interactablesState);
	}

	private void StopUseInternal(VRBaseInteraction interaction)
	{
		if (interaction != null)
			interaction.StopUse(interactablesState);
	}

	static public bool IsInteract(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.IsInteract(obj);

		return false;
	}

	static public bool IsHovered(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.IsHovered(obj);

		return false;
	}

	static public bool IsGrabbed(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.IsGrabbed(obj);

		return false;
	}

	static public bool IsUsed(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.IsUsed(obj);

		return false;
	}

	static public VRBaseInteraction GetHoverInteraction(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.GetHoverInteraction(obj);

		return null;
	}

	static public VRBaseInteraction GetGrabInteraction(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.GetGrabInteraction(obj);

		return null;
	}

	static public VRBaseInteraction GetUseInteraction(VRBaseInteractable obj)
	{
		if (instance != null)
			return instance.interactablesState.GetUseInteraction(obj);

		return null;
	}

	static public void StopHover(VRBaseInteraction interaction)
	{
		if (instance != null)
			instance.StopHoverInternal(interaction);
	}

	static public void StopGrab(VRBaseInteraction interaction)
	{
		if (instance != null)
			instance.StopGrabInternal(interaction);
	}

	static public void StopUse(VRBaseInteraction interaction)
	{
		if (instance != null)
			instance.StopUseInternal(interaction);
	}
}
