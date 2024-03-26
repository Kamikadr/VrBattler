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
using System.Collections.Generic;
using static Unigine.Plugins.Kinect;

[Component(PropertyGuid = "5f66e994e52dc7e3a2918be62506b20d2bfc2c08")]
public class VRHandController : HandController
{

	private InputVRController controllerDevice = null;
	private List<ObjectMeshStatic> controllerObjects = new List<ObjectMeshStatic>();
	private List<Mesh> controllerMeshes = new List<Mesh>();
	private List<Texture> controllerTextures = new List<Texture>();

	protected override bool Visible
	{
		get { return isVisible; }

		set
		{
			if ((!isVisible && value) || (isVisible && !value))
			{
				for (int i = 0; i < node.NumChildren; i++)
				{
					Object childObj = node.GetChild(i) as Object;
					if (childObj)
						for (int j = 0; j < childObj.NumSurfaces; j++)
							childObj.SetEnabled(value, j);
				}

				Object obj = node as Object;
				if (obj)
					for (int i = 0; i < obj.NumSurfaces; i++)
						obj.SetEnabled(value, i);
			}

			isVisible = value;
		}
	}

	protected override bool ControllerInit()
	{
		if (InputSystem.CurrentName.CompareTo("vr_input") != 0)
			node.Enabled = false;

		return (VRInput.IsLoaded) && node.Enabled;
	}

	[MethodUpdate]
	private void Render()
	{
		if (VRInput.IsLoaded)
		{
			switch (device)
			{
				case InputSystem.VRDevice.LEFT_CONTROLLER:
					controllerDevice = Input.VRControllerLeft;
					break;
				case InputSystem.VRDevice.RIGHT_CONTROLLER:
					controllerDevice = Input.VRControllerRight;
					break;
			}

			if (controllerDevice == null)
				return;

			if (controllerDevice.IsTransformValid == false)
				return;

			if(controllerObjects.Count == 0)
			{
				int num = controllerDevice.NumModels;
				if (num == 0)
				{
					controllerObjects.Add(null);
					controllerMeshes.Add(null);
					controllerTextures.Add(null);
				}
				else
				{
					for(int i =0; i < num; i++)
					{
						controllerObjects.Add(null);
						controllerMeshes.Add(null);
						controllerTextures.Add(null);
					}
				}
			}

			node.WorldTransform = controllerDevice.WorldTransform * MathLib.RotateX(-90.0f);

			bool visible = (VR.IsSteamVRRendersControllers == false);
			int num_components = controllerDevice.NumModels;
			if (num_components == 0)
			{
				if (controllerObjects[0] == null)
				{
					if (controllerMeshes[0] == null)
					{
						controllerMeshes[0] = new Mesh();
					}

					if (controllerMeshes[0] = controllerDevice.CombinedModelMesh)
					{
						controllerTextures[0] = new Texture();
						if (controllerTextures[0] = controllerDevice.CombinedModelTexture)
						{
							controllerObjects[0] = new ObjectMeshStatic();
							for(int i =0; i< controllerObjects[0].NumSurfaces; i++)
								controllerObjects[0].SetIntersectionMask(0, i);
							controllerObjects[0].Parent = node;
							controllerObjects[0].MeshProceduralMode = true;
							controllerObjects[0].ApplyMeshProcedural(controllerMeshes[0]);
							var material = Materials.FindMaterialByPath("vr_template/shaders/vr_controller.mgraph");
							if (material != null)
							{
								controllerObjects[0].SetMaterial(material, "*");
								Material mat = controllerObjects[0].GetMaterialInherit(0);

								mat.SetTexture("albedo", controllerTextures[0]);
							}

							for (int j = 0; j < controllerObjects[0].NumSurfaces; j++)
							{
								controllerObjects[0].SetCastWorldShadow(false, j);
								controllerObjects[0].SetCastShadow(false, j);
								controllerObjects[0].SetCastEnvProbeShadow(false, j);
							}
						}
					}
				}
				else
				{
					controllerObjects[0].WorldTransform = controllerDevice.WorldTransform;
					controllerObjects[0].Enabled = visible;
				}
			}
			else
			{
				int idx = 0;

				for (int i = 0; i < num_components; i++)
				{
					string str = controllerDevice.GetModelName(i);
					if (str == "base" || str == "status")
						continue;

					if (controllerObjects[idx] == null)
					{
						if (controllerMeshes[idx] == null)
							controllerMeshes[idx] = new Mesh();

						if (controllerMeshes[idx] = controllerDevice.GetModelMesh(i))
						{
							controllerTextures[idx] = new Texture();
							if (controllerTextures[idx] = controllerDevice.GetModelTexture(i))
							{
								controllerObjects[idx] = new ObjectMeshStatic();
								for (int j = 0; j < controllerObjects[idx].NumSurfaces; j++)
									controllerObjects[idx].SetIntersectionMask(0, j);
								controllerObjects[idx].Parent = node;
								controllerObjects[idx].MeshProceduralMode = true;
								controllerObjects[idx].ApplyMeshProcedural(controllerMeshes[idx]);
								var material = Materials.FindMaterialByPath("vr_template/shaders/vr_controller.mgraph");
								if (material)
								{
									controllerObjects[idx].SetMaterial(material, "*");
									Material mat = controllerObjects[idx].GetMaterialInherit(0);

									mat.SetTexture("albedo", controllerTextures[idx]);
								}

								for (int j = 0; j < controllerObjects[idx].NumSurfaces; j++)
								{
									controllerObjects[idx].SetCastWorldShadow(false, j);
									controllerObjects[idx].SetCastShadow(false, j);
									controllerObjects[idx].SetCastEnvProbeShadow(false, j);
								}
							}
						}
					}
					else
					{
						controllerObjects[idx].WorldTransform = controllerDevice.GetModelWorldTransform(i);
						controllerObjects[idx].Enabled = visible;
					}

					++idx;
				}
			}
		}
	}

	public void SetOutline(int enabled)
	{
		for (int i = 0; i < controllerObjects.Count; i++)
			if (controllerObjects[i] != null)
				for (int k = 0; k < controllerObjects[i].NumSurfaces; k++)
					controllerObjects[i].SetMaterialParameterInt("auxiliary_enabled", enabled, k);
	}

}
