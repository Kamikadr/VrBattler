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

[Component(PropertyGuid = "c2b79fc0e328901f01f19ef220aa71a9e7e10ef4")]
public class VRLaserPointer : VRBaseInteractable
{
	[ShowInEditor]
	[Parameter(Title = "Laser", Group = "VR Laser Pointer")]
	private Node laser = null;
	
	[ShowInEditor]
	[Parameter(Title = "Laser Ray", Group = "VR Laser Pointer")]
	private Node laserRay = null;

	[ShowInEditor]
	[Parameter(Title = "Laser Hit", Group = "VR Laser Pointer")]
	private Node laserHit = null;

	[ShowInEditor]
	[Parameter(Title = "Object Text", Group = "VR Laser Pointer")]
	private ObjectText objText = null;


	private Mat4 laserRayMat;
	private WorldIntersection intersection = new WorldIntersection();
	private float rayOffset = 0.05f;

	private bool grabbed = false;

	protected override void OnReady()
	{
		laserRayMat = new Mat4(laserRay.Transform);
		laser.Enabled = false;
	}

	private void Update()
	{
		if(laser.Enabled && grabbed)
		{
			laserRay.Transform = laserRayMat;

			vec3 dir = laserRay.GetWorldDirection(MathLib.AXIS.Y);
			Vec3 p0 = laserRay.WorldPosition + dir * rayOffset;
			Vec3 p1 = p0 + dir * 1000;
			Unigine.Object hitObj = World.GetIntersection(p0, p1, 1, intersection);
			if(hitObj != null) 
			{
				laserRay.Scale = new vec3(laserRay.Scale.x, MathLib.Length(intersection.Point - p0) + rayOffset, laserRay.Scale.z);
				laserHit.WorldPosition = intersection.Point;
				laserHit.Enabled = true;
			}
			else
			{
				laserHit.Enabled = false;
			}

			if (hitObj != null)
			{
				objText.Enabled = true;
				objText.Text = hitObj.Name;
				float radius = objText.BoundSphere.Radius;
				vec3 shift = vec3.UP * radius;
				objText.WorldTransform = MathLib.SetTo(laserHit.WorldPosition + shift, VRPlayer.LastPlayer.HeadController.WorldPosition, vec3.UP, MathLib.AXIS.Z);
			}
			else
				objText.Enabled = false;
		}
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller) 
	{
		grabbed = true;
	}

	public override void OnGrabEnd(VRBaseInteraction interaction, VRBaseController controller) 
	{
		grabbed = false;
		laser.Enabled = false;
		objText.Enabled = false;
	}

	public override void OnUseBegin(VRBaseInteraction interaction, VRBaseController controller) 
	{
		if(grabbed)
			laser.Enabled = true;
	}

	public override void OnUseEnd(VRBaseInteraction interaction, VRBaseController controller) 
	{
		laser.Enabled = false;
		objText.Enabled = false;
	}
}
