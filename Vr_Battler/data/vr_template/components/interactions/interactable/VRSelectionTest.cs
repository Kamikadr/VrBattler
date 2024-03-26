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

[Component(PropertyGuid = "804a381ccd4af4cb6e9943ae872cb735cee05f10")]
public class VRSelectionTest : VRBaseInteractable
{
	private Object obj = null;

	private int hoveredControllerCount = 0;
	private HandController grabbedController = null;
	private Mat4 localTransform = Mat4.IDENTITY;

	protected override void OnReady()
	{
		obj = node as Object;
		if (!obj)
		{
			Enabled = false;
			return;
		}
	}

	private void Update()
	{
		if (VRInteractionManager.IsGrabbed(this) && grabbedController != null)
		{
			SetOutline(0);
		}
	}

	public override void OnHoverBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController)
		{
			// enable visual outline
			if (hoveredControllerCount == 0)
				SetOutline(1);

			hoveredControllerCount++;
		}
	}

	public override void OnHoverEnd(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController)
		{
			hoveredControllerCount--;

			// enable visual outline
			if (hoveredControllerCount == 0)
				SetOutline(0);
		}
	}

	public override void OnGrabBegin(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController)
		{
			SetOutline(0);
			grabbedController = controller as HandController;
		}
	}

	public override void OnGrabEnd(VRBaseInteraction interaction, VRBaseController controller)
	{
		if (controller is HandController && hoveredControllerCount > 0)
		{
			SetOutline(1);
			grabbedController = null;
		}
	}

	private void SetOutline(int enabled)
	{
		for (int i = 0; i < obj.NumSurfaces; i++)
			obj.SetMaterialState("auxiliary", enabled, i);
	}
}
