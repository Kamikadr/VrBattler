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

using Unigine;

[Component(PropertyGuid = "028848a0c1ea637f30619ad2b3957c090752d5d4")]
public class VRPCHeadMenuInteraction : VRBaseInteraction
{
	[ShowInEditor]
	[ParameterSlider(Title = "Max Distance", Group = "Hand Menu Interaction", Min = 0.0f)]
	private float maxDistance = 10.0f;

	[ShowInEditor]
	[ParameterMaterial(Title = "Sphere Material", Group = "Hand Menu Interaction")]
	private Material sphereMaterial = null;

	[ShowInEditor]
	[ParameterSlider(Title = "Sphere Radius", Group = "Hand Menu Interaction", Min = 0.0f)]
	private float sphereRadius = 0.01f;

	[ShowInEditor]
	[Parameter(Title = "Exclude Nodes", Group = "Hand Menu Interaction")]
	private Node[] excludeNodes = null;

	private HeadController controller = null;

	private bool isInit = true;

	private WorldIntersectionNormal intersection = new WorldIntersectionNormal();

	private ObjectMeshDynamic menuSphere = null;

	public override VRBaseController Controller => controller;

	protected override void OnReady()
	{
		controller = GetComponent<HeadController>(node);
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
		menuSphere = Primitives.CreateSphere(sphereRadius);
		for (int i = 0; i < menuSphere.NumSurfaces; i++)
		{
			menuSphere.SetCastShadow(false, i);
			menuSphere.SetCastWorldShadow(false, i);
			menuSphere.SetCollision(false, i);
			menuSphere.SetIntersection(false, i);
		}
		menuSphere.Position = Vec3.ZERO;
		menuSphere.SetRotation(quat.IDENTITY);
	}

	public override void Interact(VRInteractionManager.InteractablesState interactablesState, float ifps)
	{
		if (!isInit)
			return;

		bool hitGui = false;
		bool mouseDown = InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.FIRE_1);

		Vec3 p0 = controller.WorldPosition;
		mat4 m = new mat4(controller.WorldTransform);
		Vec3 p1 = p0 + new Vec3(MathLib.Normalize(Utils.GetDirectionY(m))) * maxDistance;
		Vec3 p1_end = p1;
		Object hitObj;
		if (excludeNodes != null)
			hitObj = World.GetIntersection(p0, p1, 1, excludeNodes, intersection);
		else
			hitObj = World.GetIntersection(p0, p1, 1, intersection);
		ObjectGui objGui = hitObj as ObjectGui;
		ObjectGuiMesh objGuiMesh = hitObj as ObjectGuiMesh;

		if (objGui != null)
		{
			hitGui = true;
			p1_end = intersection.Point;
			objGui.SetMouse(p0, p1, mouseDown ? 1 : 0, false);
		}

		if (objGuiMesh != null)
		{
			hitGui = true;
			p1_end = intersection.Point;
			objGuiMesh.SetMouse(p0, p1, mouseDown ? 1 : 0, false);
		}

		if (hitGui)
		{
			menuSphere.Position = intersection.Point;
			menuSphere.SetMaterial(sphereMaterial, "*");
			menuSphere.Enabled = true;
		}
		else
			menuSphere.Enabled = false;
	}
}
