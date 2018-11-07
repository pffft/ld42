using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Unsorted 
{
	public class Shoot_2_Waves_45 : Move 
	{
		public Shoot_2_Waves_45()
		{
			Description = "Shoots two projectiles at a 45 degree angle that turn into AOE waves on death.";
			Difficulty = 4f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new ShootArc(4, -45f, 45f, new Projectile { 
                    Size = Size.LARGE, 
                    Speed = Speed.MEDIUM_SLOW, 
                    MaxTime = 1f, 
                    OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE 
                }).Wait(2f)
            );
        }
    }
}
