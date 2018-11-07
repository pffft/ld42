using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep_Both : Move
    {
        public Sweep_Both()
        {
            Description = "Sweeps both left and right at the same time.";
            Difficulty = 4f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new PlayerLock(true),
                For(0, 120, 5, 
                    i => Merge(
                        new Shoot1(new Projectile { AngleOffset = i - 60 }),
                        new Shoot1(new Projectile { AngleOffset = 60 - i })
                    ).Wait(0.05f)
                ),
                new PlayerLock(false),
                new Pause(0.5f)
            );
        }
    }
}
