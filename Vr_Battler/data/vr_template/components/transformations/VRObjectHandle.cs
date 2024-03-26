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

[Component(PropertyGuid = "41be672a834eec8172455beca6a0a65bf3c2a7fe")]
public class VRObjectHandle : VRBaseInteractable
{
	[ShowInEditor]
	[ParameterSlider(Title = "Change Position", Group = "VR Object Handle")]
	private bool changePosition = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Min Position", Group = "VR Object Handle")]
	[ParameterCondition(nameof(changePosition), 1)]
	private Vec3 handleMinPos = Vec3.ZERO;

	[ShowInEditor]
	[ParameterSlider(Title = "Max Position", Group = "VR Object Handle")]
	[ParameterCondition(nameof(changePosition), 1)]
	private Vec3 handleMaxPos = Vec3.ZERO;

	[ShowInEditor]
	[ParameterSlider(Title = "Change Rotation", Group = "VR Object Handle")]
	private bool changeRotation = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Min Rotation", Group = "VR Object Handle")]
	[ParameterCondition(nameof(changeRotation), 1)]
	private Vec3 handleMinRot = Vec3.ZERO;

	[ShowInEditor]
	[ParameterSlider(Title = "Max Rotation", Group = "VR Object Handle")]
	[ParameterCondition(nameof(changeRotation), 1)]
	private Vec3 handleMaxRot = Vec3.ZERO;

	private HandController grabbedController = null;

	private Object obj;
	private BodyRigid body;

	private Vec3 lastHandlePos;
	private Vec3 grabHandPos;
	private Vec3 grabObjPos;
	private quat grabRotate;

	private bool grabbed = false;
	private Vec3 speed;
	private WorldIntersection intersection = new WorldIntersection();

	private const float speedFactor = 15.0f;
	private const float accelerationFactor = 30.0f;

	protected override void OnReady()
	{
		obj = node as Object;
		body = node.ObjectBodyRigid;
		if (body != null)
			body.Freezable = false;

		speed = Vec3.ZERO;
		grabbed = false;
	}
	
	private void Update()
	{
		if(!grabbed && changePosition && MathLib.Length2(speed) > MathLib.EPSILON)
		{
			Vec3 pos = node.Position;
			Vec3 clampedPos = MathLib.Clamp(pos, handleMinPos, handleMaxPos);
			if(MathLib.Length2(clampedPos - pos) > MathLib.EPSILON)
			{
				speed = Vec3.ZERO;
				node.Position = clampedPos;
			}
			else
			{
				speed = MathLib.Lerp(speed, Vec3.ZERO, MathLib.Saturate(accelerationFactor * Game.IFps));
			}

			if (body)
				body.LinearVelocity = new vec3(speed);
		}

		if(grabbed && grabbedController)
		{
			Vec3 handlePos = lastHandlePos;

			if (changePosition)
			{
				Vec3 handOffset = grabbedController.WorldPosition - grabHandPos;
				Vec3 prevPos = node.WorldPosition;
				Vec3 targetPos = grabObjPos + handOffset;
				Node parent = node.Parent;
				if(parent != null)
				{
					targetPos = parent.IWorldTransform * targetPos;
					targetPos = MathLib.Clamp(targetPos, new Vec3(handleMinPos), new Vec3(handleMaxPos));
					targetPos = parent.WorldTransform * targetPos;
				}
				else
					targetPos = MathLib.Clamp(targetPos, new Vec3(handleMinPos), new Vec3(handleMaxPos));

				handlePos = targetPos;

				vec3 targetSpeed = new vec3(targetPos - prevPos) * speedFactor;
				speed = MathLib.Lerp(speed, targetSpeed, MathLib.Saturate(accelerationFactor * Game.IFps / (MathLib.Length(targetSpeed) + 0.01f)));
				if (body)
					body.LinearVelocity = new vec3(speed);
				else
					node.WorldPosition = targetPos;

				lastHandlePos = handlePos;
			}

			if(changeRotation)
			{
				Vec3 newRotateVec = MathLib.Normalize(grabbedController.WorldPosition - node.WorldPosition);
				Vec3 oldRotateVec = MathLib.Normalize(grabHandPos - node.WorldPosition);
				// up vector will be NaN if newRotateVec and oldRotateVec is equal
				if (newRotateVec != oldRotateVec)
				{
					Vec3 up = MathLib.Cross(newRotateVec, oldRotateVec).Normalized;
					float angle = MathLib.Angle(new vec3(oldRotateVec), new vec3(newRotateVec), new vec3(up));
					node.SetWorldRotation(new quat(new vec3(up), angle) * grabRotate);

					vec3 angles = MathLib.DecomposeRotationXYZ(node.GetRotation().Mat3);
					vec3 cAngles = new vec3(MathLib.Clamp(angles, handleMinRot, handleMaxRot));
					node.SetRotation(new quat(cAngles.x, cAngles.y, cAngles.z));
				}
			}
		}
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		lastHandlePos = node.WorldPosition;

		grabHandPos = controller.WorldPosition;
		grabbedController = controller as HandController;

		grabObjPos = node.WorldPosition;
		grabRotate = node.GetWorldRotation();

		for (int i = 0; i < obj.NumSurfaces; i++)
			obj.SetIntersection(false, i);

		grabbed = true;
	}

	public override void OnGrabEnd(VRBaseInteraction interaction, VRBaseController controller)
	{
		for (int i = 0; i < obj.NumSurfaces; i++)
			obj.SetIntersection(true, i);


		grabbedController = null;
		grabbed = false;
	}

}
