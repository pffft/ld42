using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using Constants;
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

            /*
            Sequence = new Shoot1(new Projectile
            { 
                MaxTime = 0f, 
                OnDestroyTimeout = self => {
                    // Delegates allow for arbitrary code execution.
                    Debug.Log("Bad!");
                    return new AISequence();
                }
            });
            */

            /*
            Sequence = new Shoot1(new Projectile
            {
                //OnDestroyTimeout = self => new AISequence()
                OnDestroyTimeout = self => new Shoot1(new ProjectileHoming())
            });
            */

            //Sequence = new AISequence(
            //    For(-30f, 30f, 1f,
            //        i => new Shoot1(new ProjectileData { Size = Size.MEDIUM, Speed = Speed.MEDIUM, AngleOffset = i })
            //    )
            //);
            Sequence = new AISequence(
                ForConcurrent(0, 360, 
                    i => new Shoot1(new ProjectileData { Size = Size.MEDIUM, Speed = Speed.MEDIUM, AngleOffset = i })
                )
            );
        }
    }
}
