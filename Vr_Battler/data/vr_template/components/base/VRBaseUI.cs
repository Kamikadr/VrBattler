using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "3b473469bc2f72b4e61f7e83d33b9206f539c1ad")]
public class VRBaseUI : Component
{
	protected ObjectGui objectGui = null;
	protected ObjectGuiMesh objectGuiMesh = null;
	protected Gui gui = null;

	private void Init()
	{
		objectGui = node as ObjectGui;
		objectGuiMesh = node as ObjectGuiMesh;

		if(objectGui != null)
		{
			gui = objectGui.GetGui();
			objectGui.MouseMode = ObjectGui.MOUSE_VIRTUAL;
			objectGui.Background = false;
		}

		if (objectGuiMesh != null)
		{
			gui = objectGuiMesh.Gui;
			objectGuiMesh.MouseMode = ObjectGui.MOUSE_VIRTUAL;
			objectGuiMesh.Background = false;
		}

		if (gui == null)
			return;

		InitGui();
	}

	protected virtual void InitGui() { }
}
