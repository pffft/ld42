using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using static AI.SequenceGenerators;
using Projectiles;
using static BossController;
using BossCore;
using Moves.Basic;

namespace Moves.Test
{
    public class Lightning_Arena : AISequence
    {
        public override string Description => "Spawns lightning on the whole arena";
        public override float Difficulty => 5f;

        public Lightning_Arena() : base
        (
            Teleport(World.Arena.CENTER).Wait(0.25f),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();

                for (int i = 0; i < 4; i++)
                {
                    sequences.Add(new Shoot1(
                       Projectile
                           .New(self)
                           .AngleOffset(i * 90f)
                           .Lightning(0)
                           .Speed(Speed.LIGHTNING)
                           .MaxTime(0.05f)
                           .OnDestroyTimeout(CallbackDictionary.LIGHTNING_RECUR)
                       ).Wait(0.1f));
                }
                return sequences.ToArray();
            }),
            Pause(1f)
        )
        { }
    }
}
