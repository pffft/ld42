using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;
using static World.Arena;

namespace Moves.Unsorted 
{
	public class Shoot_4_Waves_Behind : Move
    {
		public Shoot_4_Waves_Behind()
        {
			Description = "Shoots 4 projectiles behind the boss which explode into AOE attacks.";
			Difficulty = 5.5f;
            Sequence = new AISequence(
                new Teleport(CENTER).Wait(0.5f),
                new Shoot1(new Projectile { Size = Size.LARGE, Speed = Speed.MEDIUM_SLOW, MaxTime = 2f, Target = SOUTH_FAR, AngleOffset = 220, OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE }).Wait(0.5f),
                new Shoot1(new Projectile { Size = Size.LARGE, Speed = Speed.SLOW, MaxTime = 2f, Target = SOUTH_FAR, AngleOffset = 200, OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE }).Wait(0.5f),
                new Shoot1(new Projectile { Size = Size.LARGE, Speed = Speed.SLOW, MaxTime = 2f, Target = SOUTH_FAR, AngleOffset = 160, OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE }).Wait(0.5f),
                new Shoot1(new Projectile { Size = Size.LARGE, Speed = Speed.SLOW, MaxTime = 2f, Target = SOUTH_FAR, AngleOffset = 140, OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE }).Wait(5f)
            );
		}
	}
}
