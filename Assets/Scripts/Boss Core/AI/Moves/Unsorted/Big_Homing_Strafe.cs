using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using static World.Arena;
using Projectiles;

namespace Moves.Unsorted 
{
	public class Big_Homing_Strafe : Move 
	{
		public Big_Homing_Strafe()
        {
			Description = "Does a circle along the outside of the arena, shooting homing projectiles at the player.";
			Difficulty = 5f;
            Sequence = new AISequence(
                new MoveCamera(false, new Vector3(0, 17.5f, -35f)).Wait(1f),
                new Teleport(NORTH_FAR).Wait(1f),
                new ShootHomingStrafe(strafeAmount: 65).Times(10),
                new Teleport(NORTH_FAR).Wait(1f),
                new ShootHomingStrafe(strafeAmount: 15).Times(15),
                new MoveCamera(true).Wait(2f)
            );
		}
	}
}
