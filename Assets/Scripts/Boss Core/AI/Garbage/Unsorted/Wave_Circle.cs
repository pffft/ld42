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
	public class Wave_Circle : AISequence 
	{

        private class Slow_Wave_Circle : AISequence {
            public Slow_Wave_Circle() : base 
            (
                new ShootAOE(new AOE { OuterSpeed = Speed.MEDIUM_SLOW, FixedWidth = 5f }.On(0, 360)),
                new Strafe(true, 60f, 50).Wait(0.5f)
            ) 
            {
                Description = "Fires a medium-slow AOE, and then strafes 60 degrees clockwise.";
                Difficulty = 2f;
            }
        }

		public Wave_Circle() : base
		(
            new Teleport(WEST_MED),
            new Slow_Wave_Circle().Times(6),
            new Slow_Wave_Circle().Times(3),
            new ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
            new Slow_Wave_Circle().Times(3),
            new ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW),
            new Slow_Wave_Circle().Times(3),
            new ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
            new Slow_Wave_Circle().Times(3),
            new ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW)
		)
		{
			Description = "Fires six slow circles around the arena in a circular pattern. Then repeats twice, with lines appearing on the left and right sides.";
            Difficulty = 5f; 
		}
	}
}
