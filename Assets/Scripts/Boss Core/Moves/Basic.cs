using AI;
using AOEs;
using BossCore;
using Projectiles;

using static AI.AISequence;
using static AI.SequenceGenerators;
using static BossController;

namespace Moves
{
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
            Teleport().Wait(0.5f),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
            .Wait(1f)
        );

        /*
         * Shoots two 60 degree waves with a 45 degree gap in the middle.
         */
        public static AISequence SHOOT_WAVE_MIDDLE_GAP = Merge(
            ShootArc(150, 22.5f, 22.5f + 60f),
            ShootArc(150, -22.5f, -22.5f - 60f)
        );

        /* 
         * Shoots a 360 degree AOE attack.
         */
        public static AISequence SHOOT_360 = new AISequence(
            ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))
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
            Teleport().Wait(0.5f),
            PlayerLock(true),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
            PlayerLock(false).Wait(1f)
        );

    }

}