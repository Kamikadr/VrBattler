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
using System.Text.RegularExpressions;

[Component(PropertyGuid = "99fe91cbb9463cae8e2b5c35bc8cc010a5ab142f")]
public class VRHandMenuInteraction : VRBaseInteraction
{
	[ShowInEditor]
	[ParameterSlider(Title = "Max Distance", Group = "Hand Menu Interaction", Min = 0.0f)]
	private float maxDistance = 10.0f;

	[ShowInEditor]
	[ParameterMaterial(Title = "Menu Ray Material", Group = "Hand Menu Interaction")]
	private Material rayMaterial = null;

	[ShowInEditor]
	[ParameterSlider(Title = "Ray Width", Group = "Hand Menu Interaction", Min = 0.0f)]
	private float rayWidth = 0.01f;

	[ShowInEditor]
	[Parameter(Title = "Exclude Nodes", Group = "Hand Menu Interaction")]
	private Node[] excludeNodes = null;

	private HandController controller = null;
	
	private bool isInit = true;

	private WorldIntersectionNormal intersection = new WorldIntersectionNormal();

	private ObjectMeshDynamic menuRay = null;

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

		base.OnReady();
	}

	[MethodInit]
	private void LocalInit()
	{
		menuRay = new ObjectMeshDynamic();
		for (int i = 0; i < menuRay.NumSurfaces; i++)
		{
			menuRay.SetCastShadow(false, i);
			menuRay.SetCastWorldShadow(false, i);
			menuRay.SetCollision(false, i);
			menuRay.SetIntersection(false, i);
		}
		menuRay.Position = Vec3.ZERO;
		menuRay.SetRotation(quat.IDENTITY);
	}

	public override void Interact(VRInteractionManager.InteractablesState interactablesState, float ifps)
	{
		if (!isInit)
			return;

		bool hitGui = false;
		bool mouseDown = false;

		switch (controller.Device)
		{
			case InputSystem.VRDevice.LEFT_CONTROLLER:
				mouseDown = InputSystem.IsLeftButtonDown(InputSystem.ControllerButtons.USE_BUTTON);
				break;

			case InputSystem.VRDevice.RIGHT_CONTROLLER:
				mouseDown = InputSystem.IsRightButtonDown(InputSystem.ControllerButtons.USE_BUTTON);
				break;

			case InputSystem.VRDevice.PC_HAND:
				mouseDown = InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.FIRE_1);
				break;

			default: break;
		}

		Vec3 p0 = controller.WorldPosition;
		mat4 m = new mat4(controller.WorldTransform);
		Vec3 p1 = p0 + new Vec3(Utils.GetDirectionNZ(m) + Utils.GetDirectionY(m)) * maxDistance;
		Vec3 p1_end = p1;
		Object hitObj;
		if (excludeNodes != null)
			hitObj = World.GetIntersection(p0, p1, 1, excludeNodes, intersection);
		else
			hitObj = World.GetIntersection(p0, p1, 1, intersection);
		ObjectGui objGui = hitObj as ObjectGui;
		ObjectGuiMesh objGuiMesh = hitObj as ObjectGuiMesh;

		if(objGui != null)
		{
			hitGui = true;
			p1_end = intersection.Point;
			objGui.SetMouse(p0, p1, mouseDown ? 1 : 0, false);
		}

		if(objGuiMesh != null)
		{
			hitGui = true;
			p1_end = intersection.Point;
			objGuiMesh.SetMouse(p0, p1, mouseDown ? 1 : 0, false);
		}

		if (hitGui)
		{
			menuRay.ClearVertex();
			menuRay.ClearIndices();
			vec3 from = new vec3(p0);
			vec3 to = new vec3(p1_end);
			vec3 from_right = MathLib.Cross(MathLib.Normalize(to - from), new vec3(MathLib.Normalize(VRPlayer.LastPlayer.HeadController.WorldPosition - p0)));
			vec3 to_right = MathLib.Cross(MathLib.Normalize(to - from), new vec3(MathLib.Normalize(VRPlayer.LastPlayer.HeadController.WorldPosition - p1_end)));
			Utils.AddLineSegment(menuRay, from, to, from_right, to_right, rayWidth);
			menuRay.UpdateBounds();
			menuRay.UpdateTangents();
			menuRay.FlushVertex();
			menuRay.FlushIndices();
			menuRay.Enabled = true;
			menuRay.SetMaterial(rayMaterial, "*");
		}
		else
			menuRay.Enabled = false;
	}
}
