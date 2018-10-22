using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep : AISequence
    {
        public Sweep(bool reverse = false) : base
        (
            new Teleport().Wait(0.25f),
            new PlayerLock(true),
            For(reverse ? 90 : -30, 
                reverse ? -30 : 90, 
                reverse ? -5 : 5, 
                i => new Shoot1(new Projectile().AngleOffset(i)).Wait(0.05f)
               ),
            /*
            new AISequence(() =>
            {
                int start = reverse ? 90 : -30;
                int end = reverse ? -30 : 90;
                int step = reverse ? -5 : 5;

                List<AISequence> sequences = new List<AISequence>();
                for (int i = start; i != end; i += step)
                {
                    sequences.Add(new Shoot1(new Projectile().AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            */
            new PlayerLock(false),
            Pause(0.25f)
        )
        {
            Description = "Shoots a sweep from " + (reverse ? -30 : 90) +
                " degrees to " + (reverse ? 90 : -30) + " degrees offset from the player's current position.";
            Difficulty = 2f;
        }
    }
}
