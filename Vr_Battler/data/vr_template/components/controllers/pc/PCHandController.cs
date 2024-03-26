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

[Component(PropertyGuid = "a72a20f4ba87d7c3e7579a46f42faced84e62968")]
public class PCHandController : HandController
{
	[ShowInEditor]
	[ParameterSlider(Title = "Hand Length", Min = 0.1f, Max = 3.0f)]
	private float handLength = 1.3f;

	private WorldIntersection intersection = new WorldIntersection();

	private Scalar distance = 0.0f;

	protected override bool Visible
	{
		get { return isVisible; }

		set
		{
			isVisible = value;
		}
	}

	[HideInEditor]
	public bool Grabbed = false;

	protected override bool ControllerInit()
	{
		if (InputSystem.CurrentName.CompareTo("pc_input") != 0)
			node.Enabled = false;

		return PCInput.IsLoaded && node.Enabled;
	}

	protected override void Update()
	{
		if (IsConnected && IsTransformValid)
		{
			base.Update();

			Visible = true;

			if (VRPlayer.LastPlayer != null)
			{
				Vec3 dir = VRPlayer.LastPlayer.node.GetWorldDirection(MathLib.AXIS.NZ);
				Vec3 p0 = VRPlayer.LastPlayer.node.WorldPosition;
				Vec3 p1 = p0 + dir * handLength;
				World.GetIntersection(p0, p1, ~0, intersection);
				if (Grabbed == false)
				{
					distance = (intersection.Point - VRPlayer.LastPlayer.node.WorldPosition).Length;
				}

				node.Position = Vec3.BACK * distance;
			}
		}
		else
			Visible = false;

	}
}
