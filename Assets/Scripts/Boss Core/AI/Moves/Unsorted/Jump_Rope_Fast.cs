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
	public class Jump_Rope_Fast : Move
    {
		public Jump_Rope_Fast() : base
        (
            new MoveCamera(false, new Vector3(0, 17.5f, -35)).Wait(1f),
            new Teleport(WEST_FAR, 35),
            new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            new Teleport(EAST_FAR, 35),
            new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            new Teleport(WEST_FAR, 35),
            new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            new Teleport(EAST_FAR, 35),
            new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            new MoveCamera(true)
        )
        {
			Description = "Fires lines at the player from the left and right.";
			Difficulty = 4f; 
		}
	}
}
