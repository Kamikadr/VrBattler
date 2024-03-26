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

[Component(PropertyGuid = "e3483b9f616254729b34a9299e82e07459d69143")]
public class VRAttachToHead : Component
{
	[ShowInEditor]
	[ParameterSlider(Title = "Distance", Group = "VR Attach To Head")]
	private Scalar distance = 0.35f;

	[ShowInEditor]
	[ParameterSlider(Title = "Node Forward Direction Axis", Group = "VR Attach To Head")]
	private MathLib.AXIS nodeForwardDirectionAxis = MathLib.AXIS.Y;

	[ShowInEditor]
	[ParameterSlider(Title = "Fixed Position", Group = "VR Attach To Head")]
	private bool fixedPosition = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Update Position On Enable", Tooltip = "Move Node In Front Of Face On Enable", Group = "VR Attach To Head")]
	[ParameterCondition(nameof(fixedPosition), 1)]
	private bool updatePosition = false;

	vec3 fixedDirection;

	[MethodInit(Order = 2)]
	private void Init()
	{
		var headNode = VRPlayer.LastPlayer.HeadController.node;

		fixedDirection = headNode.GetWorldDirection(MathLib.AXIS.Y);

		node.WorldTransform = MathLib.SetTo(headNode.WorldPosition + new Vec3(fixedDirection * distance), headNode.WorldPosition, vec3.UP, nodeForwardDirectionAxis);
	}

	// update after movements update
	[MethodUpdate(Order = 1)]
	private void Update()
	{
		var headNode = VRPlayer.LastPlayer.HeadController.node;

		if (fixedPosition)
			node.WorldTransform = MathLib.SetTo(headNode.WorldPosition + new Vec3(fixedDirection * distance), headNode.WorldPosition, vec3.UP, nodeForwardDirectionAxis);
		else
			node.WorldTransform = MathLib.SetTo(headNode.WorldPosition + new Vec3(headNode.GetWorldDirection(MathLib.AXIS.Y) * distance), headNode.WorldPosition, vec3.UP, nodeForwardDirectionAxis);
	}

	protected override void OnEnable()
	{
		if(updatePosition)
		{
			fixedDirection = VRPlayer.LastPlayer.HeadController.node.GetWorldDirection(MathLib.AXIS.Y);
		}
	}
}
