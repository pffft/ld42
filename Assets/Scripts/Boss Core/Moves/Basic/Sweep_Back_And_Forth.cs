using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep_Back_And_Forth : AISequence
    {
        public Sweep_Back_And_Forth() : base
        (
            new Teleport().Wait(0.25f),
            new PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 90; i += 5)
                {
                    sequences.Add(new Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                sequences.Add(Pause(0.25f));
                for (int i = 30; i > -90; i -= 5)
                {
                    sequences.Add(new Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            new PlayerLock(false),
            new Pause(0.5f)
        )
        {
            Description = "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, then another from -90 to +30 degrees.";
            Difficulty = 3f;
        }
    }
}
