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
    public class Sweep_Back_And_Forth_Advanced : Move
    {
        public Sweep_Back_And_Forth_Advanced() : base
        (
            // TODO refactor me to use standard notation
            new Teleport().Wait(0.25f),
            new PlayerLock(true),
            new AISequence(() => {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 80; i += 5)
                {
                    sequences.Add(new Shoot1(new Projectile { AngleOffset = i }).Wait(0.01f));
                }
                        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -80; i -= 5)
                {
                    sequences.Add(Merge(
                        new Shoot1(new Projectile { AngleOffset = i }),
                        new Shoot1(new Projectile { AngleOffset = i, Size = Size.MEDIUM, Speed = Speed.SLOW })
                    ).Wait(0.02f));
                }
                for (int i = -80; i < 80; i += 5)
                {
                    sequences.Add(Merge(
                        new Shoot1(new Projectile { AngleOffset = i }),
                        new Shoot1(new Projectile { AngleOffset = i, Size = Size.TINY, Speed = Speed.FAST })
                    ).Wait(0.02f));
                }
                        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -30; i -= 5)
                {
                    sequences.Add(Merge(
                        new Shoot1(new Projectile { AngleOffset = i }),
                        new Shoot1(new Projectile { AngleOffset = i, Size = Size.MEDIUM, Speed = Speed.SLOW })
                    ).Wait(0.02f));
                }
                return sequences.ToArray();
            }).Wait(0.75f),
            new PlayerLock(false)
        )
        {
            Description = "Sweeps back and forth with additional medium and tiny projectiles.";
            Difficulty = 6.5f;
        }
    }
}