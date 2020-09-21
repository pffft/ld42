using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using static AI.AISequence;
using Projectiles;
using Moves.Basic;
using Constants;

namespace Phases
{
    public class Phase_1 : AIPhase
    {
        public Phase_1()
        {
            MaxHealth = 200;
            MaxArenaRadius = 1f * 50f;

            // AddSequence(10, new Moves.Basic.Sweep().Wait(1f));
            // AddSequence(10, new Moves.Basic.Sweep(reverse: true).Wait(1f));
            // AddSequence(10, new Moves.Basic.Sweep_Back_And_Forth().Wait(1f));
            // AddSequence(10, new Moves.Basic.Sweep_Both().Wait(1f));
            // AddSequence(10, new Moves.Tutorial1.Shoot_1_Several());
            // AddSequence(10, new Moves.Tutorial1.Shoot_3_Several());
            // AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70));
            // AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120));
            // AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150));
            // AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70, true));
            // AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120, true));
            // AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150, true));
            // AddSequence(10, new Moves.Basic.Shoot_Wave_Middle_Gap());
            // AddSequence(10, new Moves.Basic.Shoot_2_Waves());
            AddSequence(10, 
                new AISequence(
                    new Teleport().Wait(0.5f),
                    Either(
                        new ShootArc(100, -45f, 45f, new ProjectileData { AngleOffset = -2.5f, Size = Size.MEDIUM, Speed = Speed.VERY_FAST }),
                        Merge(
                            new ShootArc(150, 22.5f, 22.5f + 60f),
                            new ShootArc(150, -22.5f, -22.5f - 60f)
                        )
                    ).Wait(1f).Times(3)
                )
            );
            
        }
    }
}