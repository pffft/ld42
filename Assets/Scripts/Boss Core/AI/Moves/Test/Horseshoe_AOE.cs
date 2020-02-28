using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;

// TODO unfinished
namespace Moves.Test 
{
	public class Horseshoe_AOE : Move 
	{
		public Horseshoe_AOE()
		{
			Description = "Your description here";
			Difficulty = 1f;
            Sequence = new AISequence(
                //Pause(1f),
                For(10, i => 
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = 15 + (6 * i) }).Wait(0.05f), 
                        new Shoot1(new ProjectileData { AngleOffset = -15 - (6 * i) }).Wait(0.05f)
                    )
                ),
                For(10, i =>
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = 75 - (6 * i) }).Wait(0.05f),
                        new Shoot1(new ProjectileData { AngleOffset = -75 + (6 * i) }).Wait(0.05f)
                    )
                ),
                new ShootAOE(new AOEData { FixedWidth = 3f }.On(-60, 60))
            );
		}
	}
}
