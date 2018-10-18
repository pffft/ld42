using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using BossCore;
using static BossController;

namespace Moves.Test
{
    public class Quick_Waves : AISequence
    {
        public Quick_Waves() : base
        (
            () => {
                List<AISequence> sequences = new List<AISequence>();

                for (int i = 0; i < 7; i++)
                {
                    int nextAttack = Random.Range(0, 5);
                    switch (nextAttack)
                    {
                        case 0:
                            sequences.Add(
                        new ShootArc(100, -60, 60, Projectile.New(self).Size(Size.HUGE).Speed(Speed.VERY_FAST)).Wait(0.05f).Times(3)); break;
                        case 1:
                            sequences.Add(
                        new ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f).Speed(Speed.VERY_FAST))); break;
                        case 2:
                            sequences.Add(
                        new Shoot1(Projectile.New(self).Lightning(0).Speed(Speed.LIGHTNING).MaxTime(0.05f).OnDestroyTimeout(CallbackDictionary.LIGHTNING_RECUR))); break;
                        case 3:
                            sequences.Add(
                        Merge(
                            new ShootArc(150, 22.5f, 22.5f + 60f, Projectile.New(self).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
                            new ShootArc(150, -22.5f, -22.5f - 60f, Projectile.New(self).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
                        )); break;
                        case 4:
                            sequences.Add(
                        new ShootArc(100, -60, 60, Projectile.New(self).Speed(Speed.VERY_FAST).Size(Size.TINY)).Wait(0.1f).Times(7)
                        ); break;
                    }
                    sequences.Add(Pause(0.6f));
                }

                return sequences.ToArray();
            }
        )
        {
            Description = "Fires a quick barrage of random wave-based attacks.";
            Difficulty = 6f;
        }
    }
}
