using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep : Move
    {
        public Sweep(bool reverse = false)
        {
            Description = "Shoots a sweep from " + (reverse ? -30 : 90) +
                " degrees to " + (reverse ? 90 : -30) + " degrees offset from the player's current position.";
            Difficulty = 2f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new PlayerLock(true),
                For(reverse ? 90 : -30,
                    reverse ? -30 : 90,
                    reverse ? -5 : 5,
                    i => new Shoot1(new Projectile { AngleOffset = i }).Wait(0.05f)
                ),
                new PlayerLock(false),
                Pause(0.25f)
            );
        }
    }
}
