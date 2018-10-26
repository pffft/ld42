using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Basic 
{
	public class AOE_131 : AISequence 
	{
		public AOE_131() : base
        (
             new Teleport().Wait(0.5f),
             new PlayerLock(true),
             new ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
             new ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
             new ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
             new PlayerLock(false).Wait(1f)
        )
        {
			Description = "Shoots an AOE attack in a 1-3-1 pattern.";
			Difficulty = 5f; 
		}
	}
}
