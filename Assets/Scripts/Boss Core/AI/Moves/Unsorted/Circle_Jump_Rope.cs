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
	public class Circle_Jump_Rope : Move 
	{
        public Circle_Jump_Rope()
		{
			Description = "Circles that go in and out at an accelerating rate.";
			Difficulty = 8.5f;
            Sequence = new AISequence(
                new Teleport(CENTER).Wait(0.5f),
                new Wave_Reverse_Faster().Wait(1f),
                new Wave_Reverse_Faster(),
                new Shoot1(new Projectile { Size = Size.TINY, Speed = Speed.FAST }).Wait(0.1f).Times(60) // This pushes it a bit over 8.
                // Adding any of the below additional attacks makes it too hard for gameplay purposes.
                //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST, angleOffset: -20f).Wait(0.1f).Times(30)
                //Shoot3(speed: Speed.FAST, size: Size.SMALL).Wait(0.1f).Times(60)
                //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST).Wait(0.25f).Times(6).Then(Shoot1(size: Size.MEDIUM, speed: Speed.FAST, type: Type.HOMING).Wait(0.25f)).Times(4)
            );
		}
	}
}
