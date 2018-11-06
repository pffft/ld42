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
	public class Double_Hex_Curve : Move
    {
		public Double_Hex_Curve() : base
        (
            new Teleport(CENTER).Wait(1.5f),
            new PlayerLock(true),
            new Shoot_Hex_Curve(true),
            new AOE_360().Wait(0.5f),
            new Shoot_Hex_Curve(true, 30f).Wait(0.5f),
            new AOE_360().Wait(1f),
            new AOE_360().Wait(1f),
            new PlayerLock(false)
        )
        {
			Description = "Fires two hex curves, the second offset 30 degrees from the first.";
            Difficulty = 5f; 
		}
	}
}
