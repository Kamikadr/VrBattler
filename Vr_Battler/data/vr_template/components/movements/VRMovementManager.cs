using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "b7b50bc8d9da3c8729d0d03ae4aaa251afae777c")]
public class VRMovementManager : Component
{
	[ShowInEditor]
	private VRPlayer player = null;

	[ShowInEditor]
	private List<VRBaseMovement> movements = null;

	// first need to update movements
	[MethodUpdate(Order = 0)]
	private void Update()
	{
		if (movements == null)
			return;

		float ifps = Game.IFps;

		foreach (var m in movements)
			if (m != null && m.Enabled)
				m.Move(player, ifps);
	}
}
