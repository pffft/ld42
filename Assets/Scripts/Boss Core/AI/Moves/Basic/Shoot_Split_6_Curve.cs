using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.User 
{
	public class Shoot_Split_6_Curve : Move 
	{
        public Shoot_Split_6_Curve()
        {
            Description = "Shoots a projectile that splits into 6 curving projectiles.";
            Difficulty = 5f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                new Shoot1(
                    new ProjectileDeathHex
                    {
                        MaxTime = 0.25f,
                        Speed = Speed.VERY_FAST
                    }
                ),
                new Pause(0.5f)
            );
		}
	}
}
