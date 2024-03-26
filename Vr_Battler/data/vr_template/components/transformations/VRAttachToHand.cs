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

[Component(PropertyGuid = "66e64cc27e35dd56fe75bfb9f98840e60c099ef6")]
public class VRAttachToHand : Component
{
	public enum Side
	{ 
		Left = 0,
		Right,
	}

	[ShowInEditor]
	[ParameterSwitch(Title = "Side", Group = "VR Attach To Hand")]
	private Side side = Side.Left;

	public Side ControllerSide { get { return side; } }

	[ShowInEditor]
	[Parameter(Title = "Use Self Transform", Group = "VR Attach To Hand")]
	private bool useSelfTransform = false;

	[ShowInEditor]
	[Parameter(Title = "Position", Group = "VR Attach To Hand")]
	[ParameterCondition(nameof(useSelfTransform), 0)]
	private Vec3 position = Vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "Rotation", Group = "VR Attach To Hand")]
	[ParameterCondition(nameof(useSelfTransform), 0)]
	private vec3 rotation = vec3.ZERO;

	[MethodInit(Order = 2)]
	private void Init()
	{
		if (!VRInput.IsLoaded)
		{
			node.Enabled = false;
			return;
		}

		Mat4 transform = node.Transform;
		if (!useSelfTransform)
		{
			transform.SetColumn3(3, position);

			mat3 rotMat = (new quat(rotation.x, rotation.y, rotation.z)).Mat3;
			transform.SetColumn3(0, rotMat.Column0);
			transform.SetColumn3(1, rotMat.Column1);
			transform.SetColumn3(2, rotMat.Column2);
		}

		//transform = new Mat4(MathLib.RotateX(-90.0f)) * transform; // because of controller basis;

		Node controller = null;
		if(side == Side.Left)
			controller = VRPlayer.LastPlayer.LeftController.node;
		else
			controller = VRPlayer.LastPlayer.RightController.node;

		node.Parent = controller;
		node.Transform = transform;
	}
}
