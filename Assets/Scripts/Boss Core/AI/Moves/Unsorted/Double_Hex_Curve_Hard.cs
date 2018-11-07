using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;

using static World.Arena;

namespace Moves.Unsorted 
{
	public class Double_Hex_Curve_Hard : Move
    {
		public Double_Hex_Curve_Hard() : base
        (
            new Teleport(CENTER).Wait(0.5f),
            new Shoot_Hex_Curve(false, 0f),
            new Shoot_Hex_Curve(false, 30f),
            // This homing might be too hard; especially with this amount of 360s.
            new Shoot3(ProjectileHoming.DEFAULT).Wait(0.1f).Times(10),
            new AOE_360(),
            new Shoot3(ProjectileHoming.DEFAULT).Wait(0.1f).Times(5),
            new AOE_360(),
            new Shoot3(ProjectileHoming.DEFAULT).Wait(0.1f).Times(5),
            new AOE_360().Wait(0.5f),
            new AOE_360().Wait(0.5f)
        )
        {
			Description = "A harder variant of the double hex curve.";
			Difficulty = 10f; 
		}
	}
}
