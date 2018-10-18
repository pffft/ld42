using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using BossCore;
using Moves.Basic;

namespace Moves.Test
{
    public class Lightning_Arena : AISequence
    {
        public Lightning_Arena() : base
        (
            new Teleport(World.Arena.CENTER).Wait(0.25f),
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
        { 
            Description = "Spawns lightning on the whole arena";
            Difficulty = 5f;
        }
    }
}
