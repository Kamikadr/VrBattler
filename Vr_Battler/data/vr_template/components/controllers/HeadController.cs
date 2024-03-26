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

[Component(PropertyGuid = "5918d5e98bada634bf8d1c03ec0d9186d730bb2d")]
public class HeadController : VRBaseController
{
	protected override bool ControllerInit()
	{
		return InputSystem.IsLoaded;
	}

	static private Mat4 additionalTransform = new Mat4(MathLib.RotateX(-90.0f));

	protected virtual void Update()
	{
		if (VRPlayer.LastPlayer != null)
			node.WorldTransform = VRPlayer.LastPlayer.WorldTransform * new Mat4(Transform) * additionalTransform;
	}
}
