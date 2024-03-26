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

[Component(PropertyGuid = "7bb0bf379cf7932a38be2881d6d367ec869f425b")]
public class HandController : VRBaseController
{
	[ShowInEditor]
	[Parameter(Title = "Hand Position Offset", Group = "Hand Controller")]
	private vec3 handPositionOffset = new vec3(0.007f, -0.018f, 0.018f);

	[ShowInEditor]
	[Parameter(Title = "Hand Rotation Offset", Group = "Hand Controller")]
	private vec3 handRotationOffset = new vec3(-33.0f, 0, 0);

	[ShowInEditor]
	[Parameter(Title = "Show Hand Transform", Group = "Hand Controller")]
	private bool showHandTransform = false;

	static private Mat4 additionalTransform = new Mat4(MathLib.RotateX(-90.0f));

	private NodeDummy handNode = null;
	private Mat4 additionalHandTransform = Mat4.IDENTITY;

	public Mat4 HandWorldTransform => handNode.WorldTransform;

	public Mat4 HandIWorldTransform => handNode.IWorldTransform;

	public Vec3 HandWorldPosition => handNode.WorldPosition;

	public vec3 HandWorldAxisX => handNode.GetWorldDirection(MathLib.AXIS.X);

	public vec3 HandWorldAxisY => handNode.GetWorldDirection(MathLib.AXIS.Y);

	public vec3 HandWorldAxisZ => handNode.GetWorldDirection(MathLib.AXIS.Z);

	protected override void OnReady()
	{
		additionalHandTransform = new Mat4(MathLib.Translate(handPositionOffset));
		additionalHandTransform *= new Mat4(MathLib.Rotate(new quat(handRotationOffset.x, handRotationOffset.y, handRotationOffset.z)));
	}

	protected override bool ControllerInit()
	{
		return InputSystem.IsLoaded;
	}

	[MethodInit]
	protected void HandInit()
	{
		handNode = new NodeDummy();
		handNode.Parent = node;
		handNode.WorldTransform = node.WorldTransform * additionalHandTransform;

		if (showHandTransform && !Visualizer.Enabled)
			Visualizer.Enabled = true;
	}

	protected virtual void Update()
	{
		if (IsConnected && IsTransformValid)
		{
			Visible = true;

			if (VRPlayer.LastPlayer != null)
				node.WorldTransform = VRPlayer.LastPlayer.node.WorldTransform * new Mat4(Transform) * additionalTransform;

			handNode.WorldTransform = node.WorldTransform * additionalHandTransform;
		}
		else
			Visible = false;

		if (showHandTransform)
		{
			Vec3 pos = handNode.WorldPosition;

			Visualizer.RenderLine3D(pos, pos + handNode.GetWorldDirection(MathLib.AXIS.X), new vec4(1.0f, 0.0f, 0.0f, 1.0f));
			Visualizer.RenderLine3D(pos, pos + handNode.GetWorldDirection(MathLib.AXIS.Y), new vec4(0.0f, 1.0f, 0.0f, 1.0f));
			Visualizer.RenderLine3D(pos, pos + handNode.GetWorldDirection(MathLib.AXIS.Z), new vec4(0.0f, 0.0f, 1.0f, 1.0f));
		}
	}
}
