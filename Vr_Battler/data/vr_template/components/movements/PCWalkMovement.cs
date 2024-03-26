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

[Component(PropertyGuid = "ce41cfa5359f3cc05679891f714443f2d29ad2a9")]
public class PCWalkMovement : VRBaseMovement
{
	[ShowInEditor]
	[ParameterSlider(Title = "Speed", Group = "Walk", Min = 0.0f)]
	private float speed = 1.5f;

	public float Speed
	{
		get { return speed; }
		set { speed = MathLib.Abs(value); }
	}

	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null || VRInput.IsLoaded)
			return;

		Vec3 dir = Vec3.ZERO;

		if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.UP) || InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.ADDITIONAL_UP))
			dir.y += 1;
		if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.DOWN) || InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.ADDITIONAL_DOWN))
			dir.y -= 1;

		if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.LEFT) || InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.ADDITIONAL_LEFT))
			dir.x -= 1;
		if (InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.RIGHT) || InputSystem.IsGeneralButtonPress(InputSystem.GeneralButtons.ADDITIONAL_RIGHT))
			dir.x += 1;

		float angle = player.HeadController.WorldTransform.GetRotate().GetAngle(vec3.UP);
		dir = new quat(vec3.UP, angle) * dir;
		if (dir.Length2 > 0.0f)
			dir.Normalize();

		player.WorldPosition = player.WorldPosition + dir * Speed * ifps;
	}
}
