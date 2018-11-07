using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;

namespace Moves.Unsorted 
{
	public class Dash_Test : Move 
	{
		public Dash_Test()
		{
			Description = "Aggressively tests dashing.";
			Difficulty = 8f;
            Sequence = new AISequence(
                new ShootAOE(new AOE { Start = GameManager.Player.transform.position, OuterSpeed = Speed.FAST, FixedWidth = 2f }.On(0, 360f)),
                new Pause(0.75f)
            );
		}
	}
}
