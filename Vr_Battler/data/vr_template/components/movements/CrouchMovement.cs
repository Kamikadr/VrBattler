﻿/* Copyright (C) 2005-2023, UNIGINE. All rights reserved.
*
* This file is a part of the UNIGINE 2 SDK.
*
* Your use and / or redistribution of this software in source and / or
* binary form, with or without modification, is subject to: (i) your
* ongoing acceptance of and compliance with the terms and conditions of
* the UNIGINE License Agreement; and (ii) your inclusion of this notice
* in any version of this software that you use or redistribute.
* A copy of the UNIGINE License Agreement is available by contacting
* UNIGINE. at http://unigine.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c9afda98e44d9d171836178a510921613fab4b4c")]
public class CrouchMovement : VRBaseMovement
{
	public override void Move(VRPlayer player, float ifps)
	{
		if (player == null)
			return;

		if (InputSystem.IsGeneralButtonDown(InputSystem.GeneralButtons.CROUCH))
			player.OnCrouchBegin();

		if (InputSystem.IsGeneralButtonUp(InputSystem.GeneralButtons.CROUCH))
			player.OnCrouchEnd();
	}
}
