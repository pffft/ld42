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
    /*
     * A really intricate pattern. 6 projectiles that explode into 6 more projectiles,
     * repeated twice to forme a lattice. Safe spot is a midpoint between any two of
     * the first projectiles, near the far edge of the arena.
     * 
     * ** This might have changed due to the way ShootDeathHex was implemented.
     */
    public class Death_Hex : AISequence 
	{
		public Death_Hex() : base
        (
            new Teleport(CENTER).Wait(0.5f),
            new Shoot_Death_Hex(2f).Wait(1f),
            new Shoot_Death_Hex(1f).Wait(2f),
            new ShootArc(50, 0, 360, new Projectile { MaxTime = 0.25f }).Wait(1f),
            new ShootArc(50, 0, 360, new Projectile { MaxTime = 0.25f }).Wait(1f),
            new ShootArc(50, 0, 360, new Projectile { MaxTime = 0.25f }).Wait(0.75f)
        )
        {
            Description = "Fires 6 projectiles that explode into 6 more projectiles, repeated twice to form a lattice.";
            Difficulty = 9f; 
		}
	}
}
