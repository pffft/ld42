using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using AOEs;
using Moves.Basic;

namespace Moves.Test
{
    /// <summary>
    /// I really like the lasers.
    /// </summary>
    public class Random_Leading : AISequence
    {
        public Random_Leading(int count = 20) : base
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
            new Shoot1(
                new ProjectileReverse()
                {
                    Start = RANDOM_IN_ARENA,
                    Target = PLAYER_POSITION,
                    Size = Size.HUGE,
                    Speed = BossCore.Speed.FAST,
                    MaxTime = 4f
                }
            ).Wait(0.15f).Times(count),
            new ShootAOE(AOE.New(BossController.self).On(0, 360).Speed(BossCore.Speed.MEDIUM).FixedWidth(5f)).Wait(0.5f),
            new Laser(-60, 480, 5, 60),
            new Laser(60, 480, 5, 45),
            new Laser(120, 480, 5, 30)
        )
        {
        }
    }
}
