using AI;
using AOEs;
using BossCore;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;

using static AI.AISequence;
using static AI.SequenceGenerators;
using static BossController;

namespace Moves
{
    public static class Tutorial1
    {
        public static AISequence SHOOT_1_SEVERAL = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            new AISequence(() => {
                return Shoot1().Wait(0.1f).Times(Random.Range(5, 10));
            }).Wait(1.5f)
        );


        public static AISequence SHOOT_3_SEVERAL = new AISequence(
            3f,
            Teleport().Wait(0.25f),
            new AISequence(() => {
                return Shoot1().Wait(0.1f).Times(Random.Range(7, 12));
            }).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_70 = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -35, 35).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_120 = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -60, 60).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_150 = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -75, 75).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_70_DENSE = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -35, 35).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_120_DENSE = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -60, 60).Wait(1.5f)
        );

        public static AISequence SHOOT_ARC_150_DENSE = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            ShootArc(50, -75, 75).Wait(1.5f)
        );
    }
}
