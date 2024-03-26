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

using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "665a8387579e6b3a46dd8151b81dca7c3abba3ab")]
public class VRPhysicMovableObject : VRBaseInteractable
{
	[ShowInEditor]
	[ParameterSlider(Title = "Linear Velocity Factor", Group = "VR Physics Movable Object", Min = 0.0f)]
	private float linearVelocityFactor = 1.0f;

	[ShowInEditor]
	[ParameterSlider(Title = "Angular Velocity Factor", Group = "VR Physics Movable Object", Min = 0.0f)]
	private float angularVelocityfactor = 1.0f;

	[ShowInEditor]
	[Parameter(Title = "Use Handy Transform", Group = "VR Physics Movable Object")]
	private bool useHandyTransform = false;

	[ShowInEditor]
	[Parameter(Title = "Left Hand Position", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 leftHandPosition = vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "Left Hand Rotation", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 leftHandRotation = vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "Right Hand Position", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 rightHandPosition = vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "Right Hand Rotation", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 rightHandRotation = vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "PC Hand Position", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 pcHandPosition = vec3.ZERO;

	[ShowInEditor]
	[Parameter(Title = "PC Hand Rotation", Group = "VR Physics Movable Object")]
	[ParameterCondition(nameof(useHandyTransform), 1)]
	private vec3 pcHandRotation = vec3.ZERO;

	private Object obj = null;
	private BodyRigid bodyRigid = null;

	private HandController grabbedController = null;
	private PCHandController pcController = null;
	private Mat4 localTransform = Mat4.IDENTITY;
	private Mat4 leftHandyTransform = Mat4.IDENTITY;
	private Mat4 rightHandyTransform = Mat4.IDENTITY;
	private Mat4 pcHandyTransform = Mat4.IDENTITY;

	protected override void OnEnable()
	{
		VRPlayer.onTransformChanged += OnTransformChangedHandler;
	}

	protected override void OnDisable()
	{
		VRPlayer.onTransformChanged -= OnTransformChangedHandler;
	}

	protected override void OnReady()
	{
		obj = node as Object;
		if (obj == null)
		{
			Log.Error($"{nameof(VRPhysicMovableObject)} error: can't cast to object. Check \"{ node.Name}\"" + "\n");
			Enabled = false;
			return;
		}

		bodyRigid = obj.BodyRigid;
		if (bodyRigid == null)
		{
			Log.Error($"{nameof(VRPhysicMovableObject)} error: can't find physic body rigid. Check \"{ node.Name}\"" + "\n");
			Enabled = false;
			return;
		}

		if (useHandyTransform)
		{
			leftHandyTransform = new Mat4(MathLib.Rotate(new quat(leftHandRotation.x, leftHandRotation.y, leftHandRotation.z)));
			leftHandyTransform = new Mat4(MathLib.Translate(leftHandPosition)) * leftHandyTransform;

			rightHandyTransform = new Mat4(MathLib.Rotate(new quat(rightHandRotation.x, rightHandRotation.y, rightHandRotation.z)));
			rightHandyTransform = new Mat4(MathLib.Translate(rightHandPosition)) * rightHandyTransform;

			pcHandyTransform = new Mat4(MathLib.Rotate(new quat(pcHandRotation.x, pcHandRotation.y, pcHandRotation.z)));
			pcHandyTransform = new Mat4(MathLib.Translate(pcHandPosition)) * pcHandyTransform;
		}

		VRPlayer.onTransformChanged += OnTransformChangedHandler;
	}

	private void UpdatePhysics()
	{
		if (VRInteractionManager.IsGrabbed(this) && grabbedController != null)
		{
			Mat4 targetTransform = grabbedController.HandWorldTransform * localTransform;

			// set linear velocity
			vec3 deltaPos = new vec3(targetTransform.Translate - node.ObjectBodyRigid.WorldCenterOfMass);
			bodyRigid.LinearVelocity = deltaPos / Physics.IFps;

			// set angular velocity
			quat deltaRot = targetTransform.GetRotate() * MathLib.Inverse(node.ObjectBodyRigid.Rotation);

			float quatAngle = 0.0f;
			vec3 quatAxis = vec3.ZERO;
			Utils.GetAxisAngle(deltaRot, out quatAxis, out quatAngle);

			bodyRigid.AngularVelocity = (quatAxis * quatAngle * MathLib.DEG2RAD) / Physics.IFps;
		}
	}

	private void Shutdown()
	{
		VRPlayer.onTransformChanged -= OnTransformChangedHandler;
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController)
		{
			grabbedController = controller as HandController;

			if (grabbedController.Device == InputSystem.VRDevice.PC_HAND)
			{
				pcController = grabbedController as PCHandController;
				pcController.Grabbed = true;
			}

			if (useHandyTransform)
			{
				switch (grabbedController.Device)
				{
					case InputSystem.VRDevice.LEFT_CONTROLLER: localTransform = leftHandyTransform; break;
					case InputSystem.VRDevice.RIGHT_CONTROLLER: localTransform = rightHandyTransform; break;
					case InputSystem.VRDevice.PC_HAND: localTransform = pcHandyTransform; break;
					default: break;
				}
			}
			else
				localTransform = grabbedController.HandIWorldTransform * node.WorldTransform;
		}
	}

	public override void OnGrabEnd(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController)
		{
			localTransform = Mat4.IDENTITY;

			quat rot = VRPlayer.LastPlayer.WorldTransform.GetRotate();
			if (InputSystem.CurrentName.Equals("vr_input"))
				rot = rot * new quat(vec3.RIGHT, -90.0f);

			bodyRigid.LinearVelocity = rot * grabbedController.RegressionLinearVelocity * linearVelocityFactor;
			bodyRigid.AngularVelocity = rot * grabbedController.RegressionAngularVelocity * angularVelocityfactor;

			grabbedController = null;
			if(pcController != null)
			{
				pcController.Grabbed = false;
				pcController = null;
			}
		}
	}

	private void OnTransformChangedHandler()
	{
		if (VRInteractionManager.IsGrabbed(this) && grabbedController != null)
			node.WorldTransform = grabbedController.HandWorldTransform * localTransform;
	}
}
