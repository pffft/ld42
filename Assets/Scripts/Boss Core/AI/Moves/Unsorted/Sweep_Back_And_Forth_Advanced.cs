﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;

namespace Moves.Unsorted
{
    public class Sweep_Back_And_Forth_Advanced : Move
    {
        public Sweep_Back_And_Forth_Advanced()
        {
            Description = "Sweeps back and forth with additional medium and tiny projectiles.";
            Difficulty = 6.5f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new PlayerLock(true),
                For(-30, 80, 5, i => new Shoot1(new ProjectileData { AngleOffset = i }).Wait(0.01f)),
                For(80, -80, -5, i => 
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = i }),
                        new Shoot1(new ProjectileData { AngleOffset = i, Size = Size.MEDIUM, Speed = Speed.SLOW })
                       ).Wait(0.02f)
                ),
                For(-80, 80, 5, i =>
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = i }),
                        new Shoot1(new ProjectileData { AngleOffset = i, Size = Size.TINY, Speed = Speed.FAST })
                       ).Wait(0.02f)
                ),
                For(80, -30, -5, i =>
                    Merge(
                        new Shoot1(new ProjectileData { AngleOffset = i }),
                        new Shoot1(new ProjectileData { AngleOffset = i, Size = Size.MEDIUM, Speed = Speed.SLOW })
                       ).Wait(0.02f)
                ),
                new Pause(0.75f),
                new PlayerLock(false)
            );
        }
    }
}