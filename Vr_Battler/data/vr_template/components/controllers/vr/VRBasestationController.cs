using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "d95d757dfc32b8b84a93a4b7ef6e3aaa0b01aa01")]
public class VRBasestationController : BasestationController
{
	private InputVRBaseStation baseStationDevice = null;
	private List<ObjectMeshStatic> baseStationObjects = new List<ObjectMeshStatic>();
	private List<Mesh> baseSationMeshes = new List<Mesh>();
	private List<Texture> baseStationTextures = new List<Texture>();

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

	private InputVRBaseStation GetBaseStation(int num)
	{
		int baseStation_index = -1;

		for(int i =0; i < Input.NumVRDevices; i++)
		{
			var device = Input.GetVRDevice(i);
			if(device.Type == InputVRDevice.TYPE.INPUT_VR_BASE_STATION)
			{
				++baseStation_index;
				if (baseStation_index == num)
					return device as InputVRBaseStation;
			}
		}

		return null;
	}

	[MethodUpdate]
	private void Render()
	{
		if (VRInput.IsLoaded)
		{
			switch (device)
			{
				case InputSystem.VRDevice.BASESTATION_0:
					baseStationDevice = GetBaseStation(0);
					break;
				case InputSystem.VRDevice.BASESTATION_1:
					baseStationDevice = GetBaseStation(1);
					break;
				case InputSystem.VRDevice.BASESTATION_2:
					baseStationDevice = GetBaseStation(2);
					break;
				case InputSystem.VRDevice.BASESTATION_3:
					baseStationDevice = GetBaseStation(3);
					break;
			}

			if (baseStationDevice == null)
				return;

			if (baseStationObjects.Count == 0)
			{
				int num = baseStationDevice.NumModels;
				if (num == 0)
				{
					baseStationObjects.Add(null);
					baseSationMeshes.Add(null);
					baseStationTextures.Add(null);
				}
				else
				{
					for (int i = 0; i < num; i++)
					{
						baseStationObjects.Add(null);
						baseSationMeshes.Add(null);
						baseStationTextures.Add(null);
					}
				}
			}

			node.WorldTransform = baseStationDevice.WorldTransform * MathLib.RotateX(-90.0f);

			bool visible = (VR.IsSteamVRRendersControllers == false);
			int num_components = baseStationDevice.NumModels;
			if (num_components == 0)
			{
				if (baseStationObjects[0] == null)
				{
					if (baseSationMeshes[0] == null)
					{
						baseSationMeshes[0] = new Mesh();
					}

					if (baseSationMeshes[0] = baseStationDevice.CombinedModelMesh)
					{
						baseStationTextures[0] = new Texture();
						if (baseStationTextures[0] = baseStationDevice.CombinedModelTexture)
						{
							baseStationObjects[0] = new ObjectMeshStatic();
							for (int i = 0; i < baseStationObjects[0].NumSurfaces; i++)
								baseStationObjects[0].SetIntersectionMask(0, i);
							baseStationObjects[0].Parent = node;
							baseStationObjects[0].MeshProceduralMode = true;
							baseStationObjects[0].ApplyMeshProcedural(baseSationMeshes[0]);
							var material = Materials.FindMaterialByPath("vr_template/shaders/vr_controller.mgraph");
							if (material != null)
							{
								baseStationObjects[0].SetMaterial(material, "*");
								Material mat = baseStationObjects[0].GetMaterialInherit(0);

								mat.SetTexture("albedo", baseStationTextures[0]);
							}
						}
					}
				}
				else
				{
					baseStationObjects[0].WorldTransform = baseStationDevice.WorldTransform;
					baseStationObjects[0].Enabled = visible;
				}
			}
			else
			{
				int idx = 0;

				for (int i = 0; i < num_components; i++)
				{
					string str = baseStationDevice.GetModelName(i);
					if (str == "base" || str == "status")
						continue;

					if (baseStationObjects[idx] == null)
					{
						if (baseSationMeshes[idx] == null)
							baseSationMeshes[idx] = new Mesh();

						if (baseSationMeshes[idx] = baseStationDevice.GetModelMesh(i))
						{
							baseStationTextures[idx] = new Texture();
							if (baseStationTextures[idx] = baseStationDevice.GetModelTexture(i))
							{
								baseStationObjects[idx] = new ObjectMeshStatic();
								for (int j = 0; j < baseStationObjects[idx].NumSurfaces; j++)
									baseStationObjects[idx].SetIntersectionMask(0, j);
								baseStationObjects[idx].Parent = node;
								baseStationObjects[idx].MeshProceduralMode = true;
								baseStationObjects[idx].ApplyMeshProcedural(baseSationMeshes[idx]);
								var material = Materials.FindMaterialByPath("vr_template/shaders/vr_controller.mgraph");
								if (material)
								{
									baseStationObjects[idx].SetMaterial(material, "*");
									Material mat = baseStationObjects[idx].GetMaterialInherit(0);

									mat.SetTexture("albedo", baseStationTextures[idx]);
								}
							}
						}
					}
					else
					{
						baseStationObjects[idx].WorldTransform = baseStationDevice.GetModelWorldTransform(i);
						baseStationObjects[idx].Enabled = visible;
					}

					++idx;
				}
			}
		}
	}
}
