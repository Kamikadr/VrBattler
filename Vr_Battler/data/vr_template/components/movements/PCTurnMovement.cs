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
using static TurnMovement;

[Component(PropertyGuid = "a601f44b4d313d2ebfcd62b572de14908ff75eaf")]
public class PCTurnMovement : VRBaseMovement
{
	[ShowInEditor]
	[ParameterSlider(Title = "Mouse Sensitivity", Min = 0.1f, Max = 1.0f)]
	private float mouseSensitivity = 0.5f;

	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null)
			return;

		if (Input.MouseGrab)
		{
			// vertical rotation
			float currentAngle = player.node.GetRotation().GetAngle(MathLib.Cross(player.node.GetDirection(MathLib.AXIS.Y), player.node.GetDirection(MathLib.AXIS.Z)));
			float additonalAngle = -Input.MouseDeltaPosition.y * Game.IFps * mouseSensitivity * 10;
			if (currentAngle + additonalAngle > 0.0f && currentAngle + additonalAngle < 180.0f)
				player.node.Rotate(-Input.MouseDeltaPosition.y * Game.IFps * mouseSensitivity * 10, 0, 0);

			//horizontal rotation
			player.Turn(-Input.MouseDeltaPosition.x * Game.IFps * mouseSensitivity * 10);
		}
	}
}
