using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep_Both : AISequence
    {
        public Sweep_Both() : base
        (
            new Teleport().Wait(0.25f),
            new PlayerLock(true),
            new AISequence(() =>
            {

                List<AISequence> sequences = new List<AISequence>();
                for (int i = 0; i < 120; i += 5)
                {
                    sequences.Add(new Shoot1(new Projectile().AngleOffset(i - 60)));
                    sequences.Add(new Shoot1(new Projectile().AngleOffset(60 - i)));
                    sequences.Add(new Pause(0.05f));
                }
                return sequences.ToArray();
            }),
            new PlayerLock(false),
            new Pause(0.5f)
        )
        {
            Description = "Sweeps both left and right at the same time.";
            Difficulty = 4f;
        }
    }
}
