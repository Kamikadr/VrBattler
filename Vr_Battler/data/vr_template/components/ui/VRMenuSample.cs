using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "4b656d682954f1a7c756c2daf4c3b471434d231c")]
public class VRMenuSample : VRBaseUI
{
	private WidgetSprite background;
	private WidgetButton pressMe;
	private WidgetWindow window;
	private WidgetButton button;

	protected override void InitGui()
	{
		if (gui == null)
			return;

		background= new WidgetSprite(gui, "core/textures/common/black.texture");
		background.Color = new vec4(1.0f, 1.0f, 1.0f, 0.5f);
		gui.AddChild(background, Gui.ALIGN_BACKGROUND | Gui.ALIGN_EXPAND);

		pressMe = new WidgetButton(gui, "Press me!");
		gui.AddChild(pressMe, Gui.ALIGN_CENTER);
		pressMe.EventClicked.Connect(PressMeClicked);
		pressMe.FontSize = 20;
		pressMe.Toggleable = true;

		window = new WidgetWindow(gui, "Congratulations!");
		window.FontSize = 20;
		window.EventChanged.Connect(WindowChanged);

		button = new WidgetButton(gui, "OK");
		button.FontSize = 20;
		window.AddChild(button, Gui.ALIGN_CENTER);
		button.EventClicked.Connect(ButtonClicked);

		window.Arrange();
		window.Sizeable = true;
	}

	private void PressMeClicked()
	{
		if(pressMe.Toggled)
			gui.AddChild(window, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
		else
			gui.RemoveChild(window);
	}

	private void ButtonClicked()
	{
		pressMe.Toggled = false;
	}

	private void WindowChanged()
	{
		int x = MathLib.Clamp(window.PositionX, 0, window.Gui.Width - window.Width);
		int y = MathLib.Clamp(window.PositionY, 0, window.Gui.Height - window.Height);
		window.SetPosition(x, y);
	}

}
