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

            // TODO refactor me to use standard notation
            Sequence = new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 7; j++)
                    {
                        sequences.Add(new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * 6f, Size = Size.MEDIUM }).Wait(0.1f));
                    }
                    sequences.Add(new Shoot1(new ProjectileHoming { Size = Size.LARGE }));
                    for (int j = 7; j < 15; j++)
                    {
                        sequences.Add(new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * 6f, Size = Size.MEDIUM }).Wait(0.1f));
                    }
                    sequences.Add(new AOE_360());
                }
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        sequences.Add(new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f));
                    }
                    sequences.Add(new Shoot1(new ProjectileHoming { Size = Size.LARGE }));
                    for (int j = 5; j < 10; j++)
                    {
                        sequences.Add(new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f));
                    }
                    sequences.Add(new Shoot1(new ProjectileHoming { Size = Size.LARGE }));
                    for (int j = 10; j < 15; j++)
                    {
                        sequences.Add(new ShootArc(4, 0, 360, new Projectile { Target = Vector3.forward, AngleOffset = j * -6f, Size = Size.MEDIUM }).Wait(0.1f));
                    }
                    sequences.Add(new AOE_360());
                }
                return sequences.ToArray();
            });
        }
    }
}
