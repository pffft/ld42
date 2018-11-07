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
            //Sequence = new Shoot1(Projectile.DEFAULT_LARGE_SLOW).Wait(0.1f);
            //Sequence = Merge(
            //    Either(new Shoot1(Projectile.DEFAULT_LARGE_SLOW), new Shoot3(Projectile.DEFAULT_MEDIUM_MEDIUM))
            //);
            //Sequence =
            //Either(new Shoot1(Projectile.DEFAULT_LARGE_SLOW), new Shoot3(Projectile.DEFAULT_MEDIUM_MEDIUM));
            //InternalMove move = new InternalMove(() => { Debug.Log("Bad!"); });

            //Sequence = new Shoot1(new Projectile
            //{ 
            //    MaxTime = 0f, 
            //    OnDestroyTimeout = (self) => {
            //        return new InternalMove(() => { Debug.Log("Bad!"); });
            //    } 
            //});

            Sequence = new InternalMove();

        }
    }
}
