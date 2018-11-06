using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using BossCore;

namespace Moves.Test
{
    public class Quick_Waves : Move
    {
        public Quick_Waves()
        {
            Description = "Fires a quick barrage of random wave-based attacks.";
            Difficulty = 6f;
            Sequence = new AISequence(
                Either(
                    new ShootArc(100, -60, 60, new Projectile { Size = Size.HUGE, Speed = Speed.VERY_FAST }).Wait(0.05f).Times(3),
                    new ShootAOE(new AOE { OuterSpeed = Speed.VERY_FAST, FixedWidth = 3 }.On(-60, 60)),
                    new Shoot1(new ProjectileLightning()),
                    Merge(
                        new ShootArc(150, 22.5f, 22.5f + 60f, new Projectile { Size = Size.MEDIUM, Speed = Speed.VERY_FAST }),
                        new ShootArc(150, -22.5f, -22.5f - 60f, new Projectile { Size = Size.MEDIUM, Speed = Speed.VERY_FAST })
                    ),
                    new ShootArc(100, -60, 60, new Projectile { Speed = Speed.VERY_FAST, Size = Size.SMALL }).Wait(0.1f).Times(5)
                ).Wait(0.6f).Times(7)
            );
        }
    }
}
