using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Unsorted 
{
	public class Sweep_Wall : Move
    {
		public Sweep_Wall(bool clockwise=true)
        {
            Description = "Shoots a sweeping wall " + (clockwise ? "clockwise" : "counterclockwise") + ".";
			Difficulty = 5f;

            int start1 = 0;
            int end1 = clockwise ? 72 : -72;
            int step1 = clockwise ? 6 : -6;
            int start2 = clockwise ? 72 : -72;
            int end2 = 0;
            int step2 = clockwise ? -6 : 6;

            Sequence = new AISequence(
                For(start1, end1, step1, angle => new ShootWall(angle).Wait(0.1f)),
                For(start2, end2, step2, angle => new ShootWall(angle).Wait(0.1f))
            );
		}
	}
}
