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

[Component(PropertyGuid = "3e92c875792ebf50b2d88a6f1d6d35026b4b7079")]
public class VRObjectSwitch : VRBaseInteractable
{
	[ShowInEditor]
	[ParameterSlider(Title = "Play Ping-Pong Animation", Group = "VR Object Switch")]
	private bool playPingPong = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Use Quaternions", Group = "VR Object Switch")]
	private bool useQuaternions = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Animation Duration", Group = "VR Object Switch", Min = 0.1f)]
	private Scalar animationDuration = 0.5f;

	[ShowInEditor]
	[ParameterSlider(Title = "Change Position", Group = "VR Object Switch")]
	private bool changePosition = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Disabled Position", Group = "VR Object Switch")]
	[ParameterCondition(nameof(changePosition), 1)]
	private Vec3 disabledPosition = new Vec3();

	[ShowInEditor]
	[ParameterSlider(Title = "Enabled Position", Group = "VR Object Switch")]
	[ParameterCondition(nameof(changePosition), 1)]
	private Vec3 enabledPosition = new Vec3();

	[ShowInEditor]
	[ParameterSlider(Title = "Change Rotation", Group = "VR Object Switch")]
	private bool changeRotaion = false;

	[ShowInEditor]
	[ParameterSlider(Title = "Disabled Rotation", Group = "VR Object Switch")]
	[ParameterCondition(nameof(changeRotaion), 1)]
	private Vec3 disabledRotationEuler = new Vec3();

	[ShowInEditor]
	[ParameterSlider(Title = "Enabled Rotation", Group = "VR Object Switch")]
	[ParameterCondition(nameof(changeRotaion), 1)]
	private Vec3 enabledRotationEuler = new Vec3();

	int direction = 0;
	quat disabledRotationQuat, enabledRotationQuat;

	Scalar time;
	Vec3 fromPos, toPos;
	quat fromRotQuat, toRotQuat;
	Vec3 fromRotEuler, toRotEuler;

	protected override void OnReady()
	{
		disabledRotationQuat = new quat((float)disabledRotationEuler.x, (float)disabledRotationEuler.y, (float)disabledRotationEuler.z);
		enabledRotationQuat = new quat((float)enabledRotationEuler.x, (float)enabledRotationEuler.y, (float)enabledRotationEuler.z);
		direction = 0;

		if (changePosition)
			node.Position = disabledPosition;
		if (changeRotaion)
			node.SetRotation(disabledRotationQuat);

		time = 1;
	}

	private void Update()
	{
		if(time < 1)
		{
			time = MathLib.Saturate(time + (Game.IFps / Game.Scale) / animationDuration);

			if(playPingPong)
			{
				Scalar percent = 1.0f - Math.Abs(time - 0.5f) * 2.0f;

				if (changePosition)
					node.Position = new Vec3(MathLib.Lerp(time < 0.5f ? fromPos : disabledPosition, enabledPosition, percent));
				if(changeRotaion)
				{
					if (useQuaternions)
						node.SetRotation(MathLib.Slerp(time < 0.5f ? fromRotQuat : disabledRotationQuat, enabledRotationQuat, (float)percent));
					else
					{
						vec3 curRot = new vec3(MathLib.Lerp(time < 0.5f ? fromRotEuler : disabledRotationEuler, enabledRotationEuler, percent));
						node.SetRotation(new quat(curRot.x, curRot.y, curRot.z));
					}
				}
			}
			else
			{
				if (changePosition)
					node.Position = new Vec3(MathLib.Lerp(fromPos, toPos, time));
				if(changeRotaion)
				{
					if(useQuaternions)
						node.SetRotation(MathLib.Slerp(fromRotQuat, toRotQuat, (float)time));
					else
					{
						vec3 curRot = new vec3(MathLib.Lerp(fromRotEuler, toRotEuler, time));
						node.SetRotation(new quat(curRot.x, curRot.y, curRot.z));
					}
				}
			}
		}
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		direction = 1 - direction;
		fromPos = node.Position;
		fromRotQuat = node.GetRotation();
		fromRotEuler = MathLib.DecomposeRotationXYZ(new mat3(node.Transform));
		toPos = new Vec3(direction > 0 ? enabledPosition : disabledPosition);
		toRotQuat = direction > 0 ? enabledRotationQuat : disabledRotationQuat;
		toRotEuler = direction > 0 ? disabledRotationEuler : disabledRotationEuler;
		time = 0;
	}
}
