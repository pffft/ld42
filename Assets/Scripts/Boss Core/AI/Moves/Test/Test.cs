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
            //new Shoot1(new Projectile { Target = LEADING_PLAYER_POSITION, Speed = Speed.VERY_FAST }).Wait(0.5f)
            new Shoot1(new ProjectileHoming(difficulty: 1) { Speed = Speed.VERY_FAST }).Wait(0.1f)
        )
        {
        }
    }
}
