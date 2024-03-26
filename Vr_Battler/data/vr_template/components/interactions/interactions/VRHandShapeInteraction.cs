#region Math Variables
#if UNIGINE_DOUBLE
	using Scalar = System.Double;
	using Vec2 = Unigine.dvec2;
	using Vec3 = Unigine.dvec3;
	using Vec4 = Unigine.dvec4;
	using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "31ab77a3c2b9f8a68128b7cdf7c0f6c8cd172b87")]
public class VRHandShapeInteraction : VRBaseInteraction
{
	[ShowInEditor]
	[Parameter(Title = "Show Interact Trigger", Group = "VR Hand Shape Interaction")]
	private bool showInteractTrigger = false;

	private HandController controller = null;

	private Shape contactTrigger = null;
	private Mat4 localTriggerTransform = Mat4.IDENTITY;

	private bool isInit = true;

	private List<ShapeContact> contacts = null;

	private Object hoveredObject = null;
	private List<VRBaseInteractable> hoveredObjectComponents = new List<VRBaseInteractable>();
	private Object grabbedObject = null;
	private List<VRBaseInteractable> grabbedObjectComponents = new List<VRBaseInteractable>();
	private Object usedObject = null;
	private List<VRBaseInteractable> usedObjectComponents = new List<VRBaseInteractable>();

	public override VRBaseController Controller => controller;

	protected override void OnReady()
	{
		controller = GetComponent<HandController>(node);
		if (controller == null)
		{
			isInit = false;
			Enabled = false;
			return;
		}

		contacts = new List<ShapeContact>();

		base.OnReady();
	}

	[MethodInit]
	private void LocalInit()
	{
		if (node.ObjectBody && node.ObjectBody.NumShapes > 0)
		{
			Shape shape = node.ObjectBody.GetShape(0);
			localTriggerTransform = controller.IWorldTransform * shape.Transform;

			contactTrigger = shape.Clone();
			shape.Enabled = false;
		}

		if (showInteractTrigger && !Visualizer.Enabled)
			Visualizer.Enabled = true;
	}

	public override void Interact(VRInteractionManager.InteractablesState interactablesState, float ifps)
	{
		if (!isInit)
			return;

		// update trigger transform
		contactTrigger.Transform = controller.WorldTransform * localTriggerTransform;

		if (showInteractTrigger)
			contactTrigger.RenderVisualizer(new vec4(0, 0, 1, 1));

		// find new object in trigger
		Object currentObject = null;
		List<VRBaseInteractable> currentObjectComponents = new List<VRBaseInteractable>();

		contacts.Clear();
		contactTrigger.GetCollision(contacts);
		if (contacts.Count > 0)
		{
			float minDistance = MathLib.INFINITY;
			foreach (var c in contacts)
			{
				Object obj = c.Object;
				if (obj != null)
				{
					float deltaPos = new vec3((controller.WorldPosition - obj.WorldPosition)).Length2;
					if (deltaPos < minDistance)
					{
						currentObject = obj;
						minDistance = deltaPos;
					}
				}
			}
		}

		if(currentObject)
		{
			currentObjectComponents.Clear();
			currentObjectComponents.AddRange(GetComponents<VRBaseInteractable>(currentObject));
		}

		// update hover object
		if (hoveredObject != null && hoveredObject != currentObject)
		{
			foreach(var hoveredObjectComponent in hoveredObjectComponents)
			{
				hoveredObjectComponent.OnHoverEnd(this, controller);
				interactablesState.SetHovered(hoveredObjectComponent, false, this);
			}
			hoveredObjectComponents.Clear();
			hoveredObject = null;
		}

		if (currentObject != null && currentObject != hoveredObject)
		{
			hoveredObject = currentObject;
			hoveredObjectComponents.Clear();
			hoveredObjectComponents.AddRange(currentObjectComponents);
			foreach (var currentObjectComponent in currentObjectComponents)
			{
				currentObjectComponent.OnHoverBegin(this, controller);
				interactablesState.SetHovered(currentObjectComponent, true, this);
			}
		}

		// update current input
		bool grabDown = false;
		bool grabUp = false;
		bool useDown = false;
		bool useUp = false;

		switch (controller.Device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER:
				grabDown = InputSystem.IsLeftButtonDown(InputSystem.ControllerButtons.GRAB_BUTTON);
				grabUp = InputSystem.IsLeftButtonUp(InputSystem.ControllerButtons.GRAB_BUTTON);
				useDown = InputSystem.IsLeftButtonDown(InputSystem.ControllerButtons.USE_BUTTON);
				useUp = InputSystem.IsLeftButtonUp(InputSystem.ControllerButtons.USE_BUTTON);
				break;

			case InputSystem.VRDevice.RIGHT_CONTROLLER:
				grabDown = InputSystem.IsRightButtonDown(InputSystem.ControllerButtons.GRAB_BUTTON);
				grabUp = InputSystem.IsRightButtonUp(InputSystem.ControllerButtons.GRAB_BUTTON);
				useDown = InputSystem.IsRightButtonDown(InputSystem.ControllerButtons.USE_BUTTON);
				useUp = InputSystem.IsRightButtonUp(InputSystem.ControllerButtons.USE_BUTTON);
				break;

			case InputSystem.VRDevice.PC_HAND:
				grabDown = InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.FIRE_1);
				grabUp = InputSystem.IsGeneralButtonUp(InputSystem.GeneralButtons.FIRE_1);
				useDown = InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.FIRE_2);
				useUp = InputSystem.IsGeneralButtonUp(InputSystem.GeneralButtons.FIRE_2);
				break;

			default: break;
		}

		// can grab and use hovered object
		if (hoveredObject != null)
		{
			if (grabDown && grabbedObject == null)
			{

				grabbedObject = hoveredObject;
				grabbedObjectComponents.Clear();
				grabbedObjectComponents.AddRange(hoveredObjectComponents);
				foreach (var grabbedObjectComponent in grabbedObjectComponents)
				{
					if (VRInteractionManager.IsGrabbed(grabbedObjectComponent))
					{
						VRBaseInteraction inter = VRInteractionManager.GetGrabInteraction(grabbedObjectComponent);
						VRInteractionManager.StopGrab(inter);
					}

					grabbedObjectComponent.OnGrabBegin(this, controller);
					interactablesState.SetGrabbed(grabbedObjectComponent, true, this);
				}
			}

			if (useDown)
			{
				usedObject = hoveredObject;
				usedObjectComponents.Clear();
				usedObjectComponents.AddRange(hoveredObjectComponents);
				foreach (var usedObjectComponent in usedObjectComponents)
				{
					usedObjectComponent.OnUseBegin(this, controller);
					interactablesState.SetUsed(usedObjectComponent, true, this);
				}
			}
		}

		// can use grabbed object, but can't grab used object
		if (hoveredObject == null && grabbedObject != null)
		{
			if (useDown)
			{
				usedObject = grabbedObject;
				usedObjectComponents.Clear();
				usedObjectComponents.AddRange(grabbedObjectComponents);
				foreach (var usedObjectComponent in usedObjectComponents)
				{
					usedObjectComponent.OnUseBegin(this, controller);
					interactablesState.SetUsed(usedObjectComponent, true, this);
				}
			}
		}

		// stop grab
		if (grabbedObject != null && grabUp)
		{
			foreach (var grabbedObjectComponent in grabbedObjectComponents)
			{
				grabbedObjectComponent.OnGrabEnd(this, controller);
				interactablesState.SetGrabbed(grabbedObjectComponent, false, this);
			}
			grabbedObjectComponents.Clear();
			grabbedObject = null;
		}

		// stop use
		if (usedObject != null && useUp)
		{
			foreach (var usedObjectComponent in usedObjectComponents)
			{
				usedObjectComponent.OnUseEnd(this, controller);
				interactablesState.SetUsed(usedObjectComponent, false, this);
			}
			usedObjectComponents.Clear();
			usedObject = null;
		}
	}

	public override void StopHover(VRInteractionManager.InteractablesState interactablesState)
	{
		if (hoveredObject != null)
		{
			foreach (var hoveredObjectComponent in hoveredObjectComponents)
			{
				hoveredObjectComponent.OnHoverEnd(this, controller);
				interactablesState.SetHovered(hoveredObjectComponent, false, this);
			}
			hoveredObjectComponents.Clear();
			hoveredObject = null;
		}
	}

	public override void StopGrab(VRInteractionManager.InteractablesState interactablesState)
	{
		if (grabbedObject != null)
		{
			foreach (var grabbedObjectComponent in grabbedObjectComponents)
			{
				grabbedObjectComponent.OnGrabEnd(this, interactablesState.GetGrabInteraction(grabbedObjectComponent).Controller);
				interactablesState.SetGrabbed(grabbedObjectComponent, false, null);
			}
			grabbedObjectComponents.Clear();
			grabbedObject = null;
		}
	}
}
