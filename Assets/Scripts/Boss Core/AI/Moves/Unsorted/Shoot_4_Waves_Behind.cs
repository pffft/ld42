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
	public class Shoot_4_Waves_Behind : AISequence 
	{
		public Shoot_4_Waves_Behind() : base
        (
            new Teleport(CENTER).Wait(0.5f),
            new Shoot1(New(self).Size(Size.LARGE).Speed(Speed.MEDIUM_SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(220f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
            new Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(200f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
            new Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(160f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
            new Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(140f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(5f)
        )
        {
			Description = "Your description here";
			Difficulty = 1f; 
		}
	}
}
