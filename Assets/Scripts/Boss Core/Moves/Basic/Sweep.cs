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
    public class Sweep : AISequence
    {

        public override float Difficulty => 2f;

        public override string Description => 
            "Shoots a sweep from " + (reverse ? -30 : 90) + 
            " degrees to " + (reverse ? 90 : -30) + " degrees offset from the player's current position.";

        private readonly bool reverse;

        public Sweep(bool reverse = false) : base
        (
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() =>
            {
                int start = reverse ? 90 : -30;
                int end = reverse ? -30 : 90;
                int step = reverse ? -5 : 5;

                List<AISequence> sequences = new List<AISequence>();
                for (int i = start; i != end; i += step)
                {
                    sequences.Add(new Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            PlayerLock(false),
            Pause(0.25f)
        )
        {
            this.reverse = reverse;
        }
    }
}
