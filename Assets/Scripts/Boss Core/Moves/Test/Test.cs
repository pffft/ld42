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
            new ShootArc(100, 0, 360, new Projectile().Size(Size.SMALL)).Wait(0.1f)
        )
        {

        }
    }
}
