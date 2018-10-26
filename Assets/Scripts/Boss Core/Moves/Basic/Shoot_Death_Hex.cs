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
	public class Shoot_Death_Hex : AISequence 
	{
		public Shoot_Death_Hex() : base
		(
            new Shoot1(new ProjectileDeathHex()).Wait(1f)
		)
		{
			Description = "Shoots a high-damage projectile that splits into 6 curving projectiles with trails.";
			Difficulty = 4f; 
		}
	}
}
