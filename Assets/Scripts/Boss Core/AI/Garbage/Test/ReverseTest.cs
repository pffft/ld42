using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using Moves.Basic;

namespace Moves.Test
{
    public class SpinReverse : AISequence
    {
        public SpinReverse(int count = 50) : base
        (
            /*
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
            */
            ForConcurrent(count, i =>
                new Shoot1(
                    new ProjectileCurving(187, false)
                    {
                        Start = (PLAYER_POSITION + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.forward)),
                        Target = (PLAYER_POSITION + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.forward) + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.right)),
                        Speed = (BossCore.Speed.FAST),
                        MaxTime = (0.75f),
                        OnDestroyTimeout = self =>
                            new Shoot1(
                                new ProjectileReverse()
                                {
                                    Start = (self.transform.position),
                                    Target = (PLAYER_POSITION),
                                    Speed = (BossCore.Speed.SNIPE),
                                    MaxTime = (0.75f)
                                }
                           )
                    }
               )
            )
        )
        {
        }
    }
}
