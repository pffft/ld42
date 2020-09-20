using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
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

            Sequence = new AISequence(
                For(200, i => Either(
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.SMALL, Speed = Speed.FAST }),
                        new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.SMALL, Speed = Speed.FAST }),
                        new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.TINY, Speed = Speed.FAST })
                    ),
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.MEDIUM, Speed = Speed.MEDIUM }),
                        new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.MEDIUM, Speed = Speed.MEDIUM })
                    ),
                    new Shoot1(new ProjectileData { AngleOffset = Random.Range(0, 360f), Size = Size.LARGE, Speed = Speed.SLOW }),
                    If(
                        (int)i % 20 == 0, 
                        new Shoot1(ProjectileHoming.DEFAULT)
                    ),
                    If(
                        (int)i % 40 == 0,
                        new Shoot1(
                            new ProjectileData
                            {
                                Size = Size.MEDIUM,
                                Speed = Speed.MEDIUM,
                                AngleOffset = Random.Range(0, 360f),
                                MaxTime = 0.5f,
                                OnDestroyTimeout = CallbackDictionary.SPAWN_WAVE
                            }
                        )
                    ),
                    new Pause(0.05f)
                ))
            );
        }
    }
}
