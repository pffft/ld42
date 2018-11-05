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
    public class Test : AISequence
    {
        public Test() : base
        (
            //Pause(1f),
            For(10, i =>
                Merge(
                    new Shoot1(new Projectile { AngleOffset = 15 + (6 * i) }).Wait(0.05f),
                    new Shoot1(new Projectile { AngleOffset = -15 - (6 * i) }).Wait(0.05f)
                )
            ),
            For(10, i =>
                Merge(
                    new Shoot1(new Projectile { AngleOffset = 75 - (6 * i) }).Wait(0.05f),
                    new Shoot1(new Projectile { AngleOffset = -75 + (6 * i) }).Wait(0.05f)
                )
            ),
            new ShootAOE(new AOE { FixedWidth = 3f }.On(-60, 60))
        )
        {
        }
    }
}
