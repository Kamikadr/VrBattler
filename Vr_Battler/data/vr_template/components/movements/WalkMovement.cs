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

[Component(PropertyGuid = "4e91baa083d12d7fb795dcd2d588d609808a67d4")]
public class WalkMovement : VRBaseMovement
{
	[ShowInEditor]
	[ParameterSlider(Title = "Speed", Group = "Walk", Min = 0.0f)]
	private float speed = 1.5f;

	[ShowInEditor]
	[ParameterSlider(Title = "Dead Zone", Group = "Walk", Min = 0.0f, Max = 1.0f)]
	private float deadZone = 0.05f;

	[ShowInEditor]
	[Parameter(Title = "Use Controller Transform", Group = "Walk")]
	private bool useControllerTransform = false;

	public float Speed
	{
		get { return speed; }
		set { speed = MathLib.Abs(value); }
	}

	public float DeadZone
	{
		get { return deadZone; }
		set { deadZone = MathLib.Clamp(value, 0.0f, 1.0f); }
	}

	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null || player.RightController == null)
			return;

		InputSystem.VRDevice controller_side = InputSystem.VRDevice.LEFT_CONTROLLER;
		if(InputSystem.GetGeneralAxisSide(InputSystem.GeneralAxes.MAIN_HORIZONTAL) == VRBaseGeneralInput.HandSide.LEFT)
			controller_side = InputSystem.VRDevice.LEFT_CONTROLLER;
		else
			controller_side = InputSystem.VRDevice.RIGHT_CONTROLLER;

		if ((!InputSystem.IsDeviceConnected(controller_side) || !InputSystem.IsTransformValid(controller_side)) && useControllerTransform)
			return;

		Vec3 dir = Vec3.ZERO;
		dir.x = InputSystem.GetGeneralAxis(InputSystem.GeneralAxes.MAIN_HORIZONTAL);
		dir.y = InputSystem.GetGeneralAxis(InputSystem.GeneralAxes.MAIN_VERTICAL);

		if (dir.Length > DeadZone)
		{
			if (useControllerTransform)
			{
				var transform = new Mat4(InputSystem.GetTransform(controller_side));
				transform = player.WorldTransform * transform;
				dir = new Vec3(dir.x, 0.0f, -dir.y);
				dir = transform.GetRotate() * dir;
				dir.z = 0;
			}
			else
			{
				float angle = player.WorldTransform.GetRotate().GetAngle(vec3.UP);
				dir = new quat(vec3.UP, angle) * dir;
			}

			player.WorldPosition = player.WorldPosition + dir * Speed * ifps;
		}
	}
}
