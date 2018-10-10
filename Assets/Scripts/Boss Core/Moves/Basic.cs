using AI;
using AOEs;
using BossCore;
using Projectiles;
using System.Collections.Generic;

using static AI.AISequence;
using static AI.SequenceGenerators;
using static BossController;

namespace Moves
{
    /*
     * A collection of building blocks for more complex moves, and some
     * very simple attacks.
     */
    public static class Basic
    {
        /*
         * Shoots a single small, medium speed projectile at the player.
         */
        public static AISequence SHOOT_1 = new AISequence(1f, Shoot1());

        /*
         * Shoots three small, medium speed projectiles at the player.
         */
        public static AISequence SHOOT_3 = new AISequence(2f, Shoot3());

        /*
         * Shoots 2 90 waves as one block, encouraging dodging through them.
         */
        public static AISequence SHOOT_2_WAVES = new AISequence(
            4f,
            Teleport().Wait(0.5f),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
            .Wait(1f)
        );

        /*
         * Shoots two 60 degree waves with a 45 degree gap in the middle.
         */
        public static AISequence SHOOT_WAVE_MIDDLE_GAP = new AISequence(
            3f,
            Merge(
                ShootArc(150, 22.5f, 22.5f + 60f),
                ShootArc(150, -22.5f, -22.5f - 60f)
            )
        );

        /* 
         * Shoots a 360 degree AOE attack with a width of 3.
         */
        public static AISequence AOE_360 = new AISequence(
            1.5f,
            ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))
        );

        /* 
         * Shoots a 120 degree AOE attack with a width of 3.
         */
        public static AISequence AOE_120 = new AISequence(
            1.5f,
            ShootAOE(AOE.New(self).On(-60, 60f).FixedWidth(3f))
        );

        /* 
         * Shoots a 90 degree AOE attack with a width of 3.
         */
        public static AISequence AOE_90 = new AISequence(
            1.5f,
            ShootAOE(AOE.New(self).On(-45, 45f).FixedWidth(3f))
        );

        /*
         * Fires a line of projectiles at the player, then strafes 60 degrees.
         */
        public static AISequence LINE_STRAFE_60 = new AISequence(
            ShootLine(50, 75f, speed: Speed.SNIPE).Wait(0.2f),
            Strafe(true, 60f, 50)
        );

        /*
         * Shoots a medium-slow 360 AOE, then strafes 60 degrees.
         */
        public static AISequence SLOW_WAVE_CIRCLE = new AISequence(
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f)),
            Strafe(true, 60f, 50).Wait(0.5f)
        );

        /*
         * Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed
         * by another 120 degree AOE; all connected.
         */
        public static AISequence AOE_131_MEDIUM_LONG = new AISequence(
            5f,
            Teleport().Wait(0.5f),
            PlayerLock(true),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
            PlayerLock(false).Wait(1f)
        );

        /*
         * Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
         */
        public static AISequence SWEEP = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 90; i += 5)
                {
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            PlayerLock(false),
            Pause(0.25f)
        );

        public static AISequence SWEEP_REVERSE = new AISequence(
            2f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = 30; i > -90; i -= 5)
                {
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            PlayerLock(false),
            Pause(0.25f)
        );

        /*
         * Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, 
         * then another from -90 to +30 degrees.
         */
        public static AISequence SWEEP_BACK_AND_FORTH = new AISequence(
            3f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 90; i += 5)
                {
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                sequences.Add(Pause(0.25f));
                for (int i = 30; i > -90; i -= 5)
                {
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
                }
                return sequences.ToArray();
            }),
            PlayerLock(false),
            Pause(0.5f)
        );

        /*
         * Sweeps both left and right at the same time.
         */
        public static AISequence SWEEP_BOTH = new AISequence(
            4f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() => {

                List<AISequence> sequences = new List<AISequence>();
                for (int i = 0; i < 120; i += 5)
                {
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i - 60)));
                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(60 - i)));
                    sequences.Add(Pause(0.05f));
                }
                return sequences.ToArray();
            }),
            PlayerLock(false).Wait(0.5f)
        );

        /*
         * Shoots a projectile that splits into 6 more, going outwards from the death point.
         */
        public static AISequence SPLIT_6 = new AISequence(
            4,
            Teleport().Wait(0.5f),
            Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).OnDestroyTimeout(CallbackDictionary.SPAWN_6).Curving(0f, true)).Wait(0.5f)
        );

        /*
         * Shoots a projectile that splits into 6 curving projectiles.
         */
        public static AISequence SPLIT_6_CURVE = new AISequence(
            5f, 
            Teleport().Wait(0.5f),
            Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).DeathHex()).Wait(0.5f)
        );
    }

}