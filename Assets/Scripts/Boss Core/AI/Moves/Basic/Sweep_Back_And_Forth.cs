using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep_Back_And_Forth : Move
    {
        public Sweep_Back_And_Forth()
        {
            Description = "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, then another from -90 to +30 degrees.";
            Difficulty = 3f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new PlayerLock(true),
                For(-30, 90, 5, i => new Shoot1(new Projectile { AngleOffset = i }).Wait(0.05f)),
                new Pause(0.25f),
                For(30, -90, -5, i => new Shoot1(new Projectile { AngleOffset = i }).Wait(0.05f)),
                new PlayerLock(false),
                new Pause(0.5f)
            );
        }
    }
}
