using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Unsorted 
{
	public class Random_200 : Move
    {
		public Random_200()
		{
			Description = "Spawns 200 random projectiles.";
			Difficulty = 8f;
            Sequence = new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int j = 0; j < 200; j++)
                {
                    switch (Random.Range(0, 3))
                    {
                        case 0:
                            sequences.Add(Merge(
                        new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.SMALL, Speed = Speed.FAST }),
                        new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.SMALL, Speed = Speed.FAST }),
                        new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.TINY, Speed = Speed.FAST })
                    )); break;
                        case 1:
                            sequences.Add(Merge(
                        new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.MEDIUM, Speed = Speed.MEDIUM }),
                        new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.MEDIUM, Speed = Speed.MEDIUM })
                    )); break;
                        case 2:
                            sequences.Add(
                                new Shoot1(new Projectile { AngleOffset = Random.Range(0, 360f), Size = Size.LARGE, Speed = Speed.SLOW })
                            );
                            break;
                    }
                    if (j % 20 == 0)
                    {
                        sequences.Add(new Shoot1(ProjectileHoming.DEFAULT));
                    }
                    if (j % 40 == 0)
                    {
                        sequences.Add(new Shoot1(
                            new Projectile
                            {
                                Size = Size.MEDIUM,
                                Speed = Speed.MEDIUM,
                                AngleOffset = Random.Range(0, 360f),
                                MaxTime = 0.5f,
                                OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE
                            }
                        ));
                    }
                    sequences.Add(Pause(0.05f));
                }
                return sequences.ToArray();
            });
		}
    }
}
