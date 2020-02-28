using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;
using static BossController;
using static Constants.Positions;

namespace Moves.Unsorted 
{
	public class Circle_In_Out : Move 
	{
		public Circle_In_Out()
		{
			Description = "Shoots a circle in and out five times.";
			Difficulty = 6f; 
            Sequence = new AISequence(
                new Teleport(CENTER).Wait(0.5f),
                new Wave_Reverse().Wait(1.5f).Times(5).Wait(3f)
			);
		}
	}
}
