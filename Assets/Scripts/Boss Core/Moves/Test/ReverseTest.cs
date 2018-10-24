using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using Moves.Basic;

namespace Moves.Test
{
    public class ReverseTest : AISequence
    {
        public ReverseTest() : base
        (
            new PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = 0; i < 50; i++)
                {
                    sequences.Add(
                        new Shoot1(
                            new ProjectileReverse()
                                .Speed(BossCore.Speed.VERY_FAST)
                                .Start(Quaternion.AngleAxis(i * (360f / 50f), Vector3.up)
                                       * (5 * Vector3.forward)))
                    );
                }
                return Merge(sequences.ToArray());
            }),
            new PlayerLock(false)
        )
        {
        }
    }
}
