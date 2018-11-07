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
	public class Jump_Rope_Slow_Circles : Move 
	{

        private static AISequence Line_Strafe_60 = new AISequence(
            new ShootLine(50, 75f, speed: Speed.SNIPE),
            new Pause(0.2f),
            new Strafe(true, 60f, 50)
        );

        private static AISequence Line_Circle_Strafe_60 = new AISequence(
            new ShootAOE(new AOE { OuterSpeed = Speed.MEDIUM_SLOW, FixedWidth = 5f }.On(0, 360f)),
            new Pause(0.1f),
            new Strafe(true, 30f, 50),
            new Pause(0.3f),
            new ShootLine(50, 100f, Vector3.zero, Speed.VERY_FAST),
            new Pause(0.2f),
            new Strafe(true, 30f, 50)
        );

        public Jump_Rope_Slow_Circles()
		{
            Description = "Dashes 6 times around shooting lines; then dashes 6 times shooting lines and AOE waves.";
			Difficulty = 5.5f;
            Sequence = new AISequence(

                new Teleport(WEST_FAR),
                Line_Strafe_60.Times(6),
                Line_Circle_Strafe_60.Times(6)
            );
		}
	}
}
