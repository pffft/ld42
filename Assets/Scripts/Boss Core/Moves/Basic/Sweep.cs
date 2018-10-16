using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using static AI.SequenceGenerators;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Sweep : Move
    {
        public override float GetDifficulty()
        {
            return 2f;
        }

        public override string GetDescription()
        {
            return "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.";
        }

        public override AISequence GetSequence()
        {
            return new AISequence(
                Teleport().Wait(0.25f),
                PlayerLock(true),
                new AISequence(() =>
                {
                    List<AISequence> sequences = new List<AISequence>();
                    for (int i = -30; i < 90; i += 5)
                    {
                        sequences.Add(new Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                    }
                    return sequences.ToArray();
                }),
                PlayerLock(false),
                Pause(0.25f)
            );
        }
    }
}
