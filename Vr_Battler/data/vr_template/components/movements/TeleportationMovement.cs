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

[Component(PropertyGuid = "7d978265b6f1d0803c4989df0fe0cdb2b3305169")]
public class TeleportationMovement : VRBaseMovement
{
	[ShowInEditor]
	[ParameterMask(Title = "Ray Intersection Mask", Group = "Teleportation", MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	private int rayIntersectionMask = 1;

	[ShowInEditor]
	[ParameterMask(Title = "Teleportation Mask", Group = "Teleportation", MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	private int teleportationMask = 1;

	[ShowInEditor]
	[ParameterSlider(Title = "Max Distance", Group = "Teleportation", Min = 0.0f)]
	private float maxDistance = 10.0f;

	[ShowInEditor]
	[Parameter(Title = "Allowed Point", Group = "Teleportation")]
	private Node allowedPoint = null;

	[ShowInEditor]
	[Parameter(Title = "Forbidden Point", Group = "Teleportation")]
	private Node forbiddenPoint = null;

	[ShowInEditor]
	[ParameterMaterial(Title = "Allowed Material", Group = "Teleportation")]
	private Material allowedMaterial = null;

	[ShowInEditor]
	[ParameterMaterial(Title = "Forbidden Material", Group = "Teleportation")]
	private Material forbiddenMaterial = null;

	[ShowInEditor]
	[ParameterSlider(Title = "Ray Width", Group = "Teleportation", Min = 0.0f)]
	private float rayWidth = 0.025f;

	[ShowInEditor]
	[Parameter(Title = "Start Point Offset", Group = "Teleportation")]
	private vec3 startPoint = vec3.ZERO;

	public int RayIntersectionMask
	{
		get { return rayIntersectionMask; }
		set { rayIntersectionMask = value; }
	}

	public int TeleportationMask
	{
		get { return teleportationMask; }
		set { teleportationMask = value; }
	}

	public float MaxDistance
	{
		get { return maxDistance; }
		set { maxDistance = MathLib.Abs(value); }
	}

	public float RayWidth
	{
		get { return rayWidth; }
		set { rayWidth = MathLib.Abs(value); }
	}

	public vec3 StartPoint
	{
		get { return startPoint; }
		set { startPoint = value; }
	}

	private ObjectMeshDynamic teleportRay = null;

	private PhysicsIntersection intersection = new PhysicsIntersection();

	private enum TELEPORT_SIDE
	{
		NONE,
		LEFT,
		RIGHT
	}

	private TELEPORT_SIDE currentTeleportSide = TELEPORT_SIDE.NONE;

	private void Init()
	{
		teleportRay = new ObjectMeshDynamic();
		for (int i = 0; i < teleportRay.NumSurfaces; i++)
		{
			teleportRay.SetCastShadow(false, i);
			teleportRay.SetCastWorldShadow(false, i);
			teleportRay.SetCollision(false, i);
			teleportRay.SetIntersection(false, i);
		}
		teleportRay.Position = Vec3.ZERO;
		teleportRay.SetRotation(quat.IDENTITY);

		allowedPoint.Enabled = false;
		forbiddenPoint.Enabled = false;
	}

	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null)
			return;

		bool isPress = false;
		bool isUp = false;
		Vec3 pos1 = Vec3.ZERO;
		Vec3 pos2 = Vec3.ZERO;

		if (currentTeleportSide == TELEPORT_SIDE.NONE)
		{
			if (InputSystem.IsLeftButtonPress(InputSystem.ControllerButtons.TELEPORTATION_BUTTON))
				currentTeleportSide = TELEPORT_SIDE.LEFT;

			if (InputSystem.IsRightButtonPress(InputSystem.ControllerButtons.TELEPORTATION_BUTTON))
				currentTeleportSide = TELEPORT_SIDE.RIGHT;
		}

		switch (currentTeleportSide)
		{
			case TELEPORT_SIDE.LEFT:
				if (player.LeftController != null)
				{
					isPress = InputSystem.IsLeftButtonPress(InputSystem.ControllerButtons.TELEPORTATION_BUTTON);
					isUp = InputSystem.IsLeftButtonUp(InputSystem.ControllerButtons.TELEPORTATION_BUTTON);

					pos1 = player.LeftController.WorldTransform * new Vec3(StartPoint);
					pos2 = pos1 + player.LeftController.WorldAxisY * MaxDistance;
				}
				break;

			case TELEPORT_SIDE.RIGHT:
				if (player.RightController != null)
				{
					isPress = InputSystem.IsRightButtonPress(InputSystem.ControllerButtons.TELEPORTATION_BUTTON);
					isUp = InputSystem.IsRightButtonUp(InputSystem.ControllerButtons.TELEPORTATION_BUTTON);

					pos1 = player.RightController.WorldTransform * new Vec3(StartPoint);
					pos2 = pos1 + player.RightController.WorldAxisY * MaxDistance;
				}
				break;

			default: break;
		}

		if (isPress || isUp)
		{
			Object hitObj = Physics.GetIntersection(pos1, pos2, RayIntersectionMask, intersection);

			if (hitObj)
			{
				var currentPhysicsMask = 0;
				if (intersection.Shape) currentPhysicsMask = intersection.Shape.PhysicsIntersectionMask;
				else if (intersection.Surface >= 0) currentPhysicsMask = hitObj.GetPhysicsIntersectionMask(intersection.Surface);

				if ((currentPhysicsMask & TeleportationMask) != 0)
				{
					// show marker
					if (isPress)
					{
						pos2 = intersection.Point + new Vec3(0, 0, 0.05f);
						allowedPoint.WorldPosition = pos2;
						allowedPoint.Enabled = true;

						teleportRay.SetMaterial(allowedMaterial, "*");

						forbiddenPoint.Enabled = false;
					}

					// teleport!
					if (isUp)
					{
						player.LandTo(intersection.Point, true);
						allowedPoint.Enabled = false;
						forbiddenPoint.Enabled = false;
					}
				}
				else
				{
					allowedPoint.Enabled = false;

					pos2 = intersection.Point + new Vec3(0, 0, 0.05f);
					forbiddenPoint.WorldPosition = pos2;
					forbiddenPoint.Enabled = true;

					teleportRay.SetMaterial(forbiddenMaterial, "*");
				}
			}
			else
			{
				allowedPoint.Enabled = false;
				forbiddenPoint.Enabled = false;

				teleportRay.SetMaterial(forbiddenMaterial, "*");
			}
		}

		// show ray
		if (isPress)
		{
			teleportRay.ClearVertex();
			teleportRay.ClearIndices();

			int num = 30;   // num of quads
			float inum = 1.0f / num;

			Vec3 lastP = pos1;
			for (int i = 1; i <= num; i++)
			{
				Vec3 p = GetHermiteSpline(pos1, pos1, pos2, pos2 + new Vec3(0, 0, -2.0f), inum * i);
				AddLineSegment(teleportRay, lastP, p, RayWidth);
				lastP = p;
			}

			teleportRay.UpdateBounds();
			teleportRay.UpdateTangents();
			teleportRay.FlushVertex();
			teleportRay.FlushIndices();

			teleportRay.Enabled = true;
		}
		else
		{
			teleportRay.Enabled = false;
			allowedPoint.Enabled = false;
			forbiddenPoint.Enabled = false;
		}

		if (isUp)
			currentTeleportSide = TELEPORT_SIDE.NONE;
	}

	private Vec3 GetHermiteSpline(Vec3 p0, Vec3 p1, Vec3 p2, Vec3 p3, float t)
	{
		float t2 = t * t;
		float t3 = t2 * t;

		float tension = 0.5f;   // 0.5 equivale a catmull-rom

		Vec3 pp1 = (p2 - p0) * tension;
		Vec3 pp2 = (p3 - p1) * tension;

		float blend1 = 2.0f * t3 - 3.0f * t2 + 1.0f;
		float blend2 = -2.0f * t3 + 3.0f * t2;
		float blend3 = t3 - 2.0f * t2 + t;
		float blend4 = t3 - t2;

		return p1 * blend1 + p2 * blend2 + pp1 * blend3 + pp2 * blend4;
	}

	private void AddLineSegment(ObjectMeshDynamic mesh, Vec3 from, Vec3 to, Vec3 fromForward, float width)
	{
		Vec3 up = new Vec3(0, 0, 1);
		Vec3 toForward = MathLib.Normalize(to - from);
		Vec3 toRight = MathLib.Normalize(MathLib.Cross(toForward, up));
		Vec3 fromRight = MathLib.Normalize(MathLib.Cross(fromForward, up));

		mesh.AddTriangleQuads(1);
		Vec3 p0 = from - fromRight * width * 0.5f; // 0, 0
		Vec3 p1 = from + fromRight * width * 0.5f;  // 1, 0
		Vec3 p2 = to + toRight * width * 0.5f; // 1, 1
		Vec3 p3 = to - toRight * width * 0.5f; // 0, 1
		mesh.AddVertex(new vec3(p0)); mesh.AddTexCoord(new vec4(0, 0, 0, 0));
		mesh.AddVertex(new vec3(p1)); mesh.AddTexCoord(new vec4(1, 0, 0, 0));
		mesh.AddVertex(new vec3(p2)); mesh.AddTexCoord(new vec4(1, 1, 0, 0));
		mesh.AddVertex(new vec3(p3)); mesh.AddTexCoord(new vec4(0, 1, 0, 0));
	}

	private void AddLineSegment(ObjectMeshDynamic mesh, Vec3 from, Vec3 to, float width)
	{
		Vec3 fromForward = MathLib.Normalize(to - from);
		AddLineSegment(mesh, from, to, fromForward, width);
	}
}
