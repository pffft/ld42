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
	public class Sweep_Wall : Move
    {
		public Sweep_Wall(bool clockwise=true)
        {
            Description = "Shoots a sweeping wall " + (clockwise ? "clockwise" : "counterclockwise") + ".";
			Difficulty = 5f;
            Sequence = new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();

                for (int angle = 0; angle != (clockwise ? 72 : -72); angle += clockwise ? 6 : -6)
                {
                    sequences.Add(new ShootWall(angleOffset: angle).Wait(0.1f));
                }

                for (int angle = clockwise ? 72 : -72; angle != 0; angle -= clockwise ? 6 : -6)
                {
                    sequences.Add(new ShootWall(angleOffset: angle).Wait(0.1f));
                }
                return sequences.ToArray();
            });
		}
	}
}
