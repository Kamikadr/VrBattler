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

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;


[Component(PropertyGuid = "2fecb15d4c34031821a6ab971c02d40a7e09b3e1")]
public class VRPlayer : Component
{
	[ShowInEditor]
	[ParameterSlider(Title = "Player Height", Group = "VR Player", Min = 0.0f)]
	private float playerHeight = 1.7f;

	[ShowInEditor]
	[ParameterSlider(Title = "Player Height", Group = "VR Player", Min = 0.0f)]
	private float playerCrouchHeight = 1.3f;

	[ShowInEditor]
	[ParameterMaterial(Title = "Post Material", Group = "VR Player")]
	private Material postMaterial = null;

	public Mat4 Transform
	{
		get { return node.Transform; }
		set
		{
			node.Transform = value;
			onTransformChanged?.Invoke();
		}
	}

	public Vec3 Position
	{
		get { return node.Position; }
		set
		{
			node.Position = value;
			onTransformChanged?.Invoke();
		}
	}

	public Mat4 WorldTransform
	{
		get { return node.WorldTransform; }
		set
		{
			node.WorldTransform = value;
			onTransformChanged?.Invoke();
		}
	}

	public Vec3 WorldPosition
	{
		get { return node.WorldPosition; }
		set
		{
			node.WorldPosition = value;
			onTransformChanged?.Invoke();
		}
	}

	public PlayerDummy Camera => node as PlayerDummy;

	public HandController LeftController { get; private set; } = null;

	public HandController RightController { get; private set; } = null;

	public HeadController HeadController { get; private set; } = null;

	public HandController PCHandController { get; private set; } = null;

	static public VRPlayer LastPlayer { get; private set; } = null;

	static public event Action onTransformChanged;

	private WorldIntersection intersection = new WorldIntersection();

	private float currentPlayerHeight = 0.0f;

	protected override void OnReady()
	{
		VRBaseController.onInit += OnControllerInitHandler;
	}

	protected override void OnEnable()
	{
		VRBaseController.onInit += OnControllerInitHandler;
	}

	protected override void OnDisable()
	{
		VRBaseController.onInit -= OnControllerInitHandler;
	}

	[MethodInit(Order = 1)]
	private void Init()
	{
		if (postMaterial != null)
			Camera.AddScriptableMaterial(postMaterial);

		LastPlayer = this;

		currentPlayerHeight = playerHeight;

		LandTo(node.WorldPosition);
		SetViewDirection(node.GetWorldDirection());
	}

	private void Shutdown()
	{
		VRBaseController.onInit -= OnControllerInitHandler;
	}

	public void SetWorldPosition(Vec3 position)
	{
		if (HeadController == null)
			return;

		Vec3 headOffset = WorldPosition - HeadController.WorldPosition;
		headOffset.z = currentPlayerHeight;

		node.WorldPosition = position + new Vec3(headOffset);

		onTransformChanged?.Invoke();
	}

	public void SetViewDirection(vec3 dir)
	{
		if (HeadController == null)
			return;

		dir = MathLib.Normalize(dir);
		quat rot = MathLib.Conjugate(new quat(MathLib.LookAt(new vec3(0, 0, 0), new vec3(dir), new vec3(0, 0, 1))));
		vec3 angles = MathLib.DecomposeRotationYXZ(new mat3(HeadController.Transform));
		node.SetWorldRotation(rot * new quat(0.0f, -angles.y, 0.0f));

		onTransformChanged?.Invoke();
	}

	public void LandTo(Vec3 position, bool isGroundPoint = false)
	{
		if (HeadController == null)
			return;

		Vec3 headOffset = node.WorldPosition - HeadController.WorldPosition;
		headOffset.z = InputSystem.CurrentName.Equals("vr_input") ? 0.0f : 1.7f;

		if (!isGroundPoint)
		{
			// put player to the ground
			Vec3 pos2 = position + new Vec3(0, 0, -1) * (node as Player).ZFar;
			Unigine.Object hitObj = World.GetIntersection(position, pos2, 1, intersection);
			if (hitObj != null)
				node.WorldPosition = intersection.Point + new Vec3(headOffset);
		}
		else
			node.WorldPosition = position + new Vec3(headOffset);

		onTransformChanged?.Invoke();
	}

	public void Turn(float angle)
	{
		if (HeadController == null)
			return;

		quat rot = new quat(0, 0, angle);
		Vec3 offset0 = HeadController.WorldPosition - node.WorldPosition;
		Vec3 offset1 = rot * offset0;

		node.Position = node.Position + offset0 - offset1;
		node.SetRotation(rot * node.GetRotation());

		onTransformChanged?.Invoke();
	}

	public void OnCrouchBegin()
	{
		Vec3 position = new Vec3(WorldPosition.x, WorldPosition.y, WorldPosition.z - currentPlayerHeight);
		currentPlayerHeight = playerCrouchHeight;
		SetWorldPosition(position);
	}

	public void OnCrouchEnd()
	{
		Vec3 position = new Vec3(WorldPosition.x, WorldPosition.y, WorldPosition.z - currentPlayerHeight);
		currentPlayerHeight = playerHeight;
		SetWorldPosition(position);
	}

	private void OnControllerInitHandler(VRBaseController controller)
	{
		if ((controller is HandController) && (controller.Device == InputSystem.VRDevice.LEFT_CONTROLLER))
			LeftController = controller as HandController;

		if ((controller is HandController) && (controller.Device == InputSystem.VRDevice.RIGHT_CONTROLLER))
			RightController = controller as HandController;

		if ((controller is HeadController) && (controller.Device == InputSystem.VRDevice.HMD || controller.Device == InputSystem.VRDevice.PC_HEAD))
			HeadController = controller as HeadController;

		if ((controller is HandController) && (controller.Device == InputSystem.VRDevice.PC_HAND))
			PCHandController = controller as HandController;
	}
}
