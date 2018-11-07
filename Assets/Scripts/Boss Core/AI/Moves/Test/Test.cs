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
    public class Test : Move
    {
        public Test()
        {
            Sequence = new Shoot1(Projectile.DEFAULT_LARGE_SLOW).Wait(0.1f);
        }
    }
}
