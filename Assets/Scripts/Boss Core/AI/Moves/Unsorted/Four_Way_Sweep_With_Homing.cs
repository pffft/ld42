using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;

namespace Moves.Unsorted
{
    public class Four_Way_Sweep_With_Homing : Move
    {
        public Four_Way_Sweep_With_Homing()
        {
            Description = "Shoots a 4-directional sweep with homing projectiles in between.";
            Difficulty = 6f;

            Sequence = new AISequence(
                For(4, i => new AISequence
                (
                    For(0, 7, j =>
                        new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * 6f, Size = Size.MEDIUM }).Wait(0.1f)
                    ),
                    new Shoot1(new ProjectileHoming { Size = Size.LARGE }),
                    For(8, 15, j =>
                        new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * 6f, Size = Size.MEDIUM }).Wait(0.1f)
                    ),
                    new AOE_360()
                )),
                For(4, i => new AISequence(
                    For(0, 5, j =>
                        new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f)
                    ),
                    new Shoot1(new ProjectileHoming { Size = Size.LARGE }),
                    For(5, 10, j =>
                        new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f)
                    ),
                    new Shoot1(new ProjectileHoming { Size = Size.LARGE }),
                    For(10, 15, j =>
                        new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f)
                    ),
                    new AOE_360()
                ))
            );
        }
    }
}
