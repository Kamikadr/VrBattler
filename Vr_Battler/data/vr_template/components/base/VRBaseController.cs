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

[Component(PropertyGuid = "5a15dc4c08bd6b3b9f89e75ee0cd2725370fae40")]
public class VRBaseController : Component
{
	[ShowInEditor]
	[Parameter(Group = "VR Controller", Title = "Device")]
	protected InputSystem.VRDevice device = InputSystem.VRDevice.LEFT_CONTROLLER;

	[ShowInEditor]
	[ParameterSlider(Group = "VR Controller", Title = "Velocity Buffer Size", Min = 1, Max = 20)]
	private int velocityBufferSize = 4;

	private vec3[] linearVelocityBuffer = null;
	private vec3[] angularVelocityBuffer = null;

	public InputSystem.VRDevice Device => device;

	public bool IsConnected => InputSystem.IsDeviceConnected(device);

	protected bool isVisible = false;
	protected virtual bool Visible
	{
		get { return isVisible; }
		set { isVisible = value; }
	}

	public bool IsEnabled { get; set; }

	public bool IsTransformValid => InputSystem.IsTransformValid(device);

	public mat4 Transform => InputSystem.GetTransform(device);

	public Mat4 WorldTransform => node.WorldTransform;

	public Mat4 IWorldTransform => node.IWorldTransform;

	public Vec3 WorldPosition => WorldTransform.GetColumn3(3);

	public Vec3 WorldAxisX => WorldTransform.GetColumn3(0);

	public Vec3 WorldAxisY => WorldTransform.GetColumn3(1);

	public Vec3 WorldAxisZ => WorldTransform.GetColumn3(2);

	public vec3 LinearVelocity => InputSystem.GetLinearVelocity(device);

	public vec3 AngularVelocity => InputSystem.GetAngularVelocity(device);

	public vec3 RegressionLinearVelocity => Utils.LinearRegression(linearVelocityBuffer);

	public vec3 RegressionAngularVelocity => Utils.LinearRegression(angularVelocityBuffer);

	static public event Action<VRBaseController> onInit;

	protected void Init()
	{
		bool res = ControllerInit();

		if (res)
			onInit?.Invoke(this);
		else
		{
			Enabled = false;
			return;
		}

		linearVelocityBuffer = new vec3[velocityBufferSize];
		angularVelocityBuffer = new vec3[velocityBufferSize];
	}

	[MethodUpdate]
	protected void VelocityBuffersUpdate()
	{
		for (int i = 1; i < velocityBufferSize; i++)
			linearVelocityBuffer[i - 1] = linearVelocityBuffer[i];
		linearVelocityBuffer[velocityBufferSize - 1] = LinearVelocity;

		for (int i = 1; i < velocityBufferSize; i++)
			angularVelocityBuffer[i - 1] = angularVelocityBuffer[i];
		angularVelocityBuffer[velocityBufferSize - 1] = AngularVelocity;
	}

	protected virtual bool ControllerInit() { return false; }
}
