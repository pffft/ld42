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
    //public class Basic
    //{
        //#region Declarations

        ///// <summary>
        ///// Shoots a single small, medium speed projectile at the player.
        ///// </summary>
        //public static Move SHOOT_1;

        ///// <summary>
        ///// Shoots three small, medium speed projectiles at the player.
        ///// </summary>
        //public static Move SHOOT_3;

        ///// <summary>
        ///// Shoots 2 90 waves as one block, encouraging dodging through them.
        ///// </summary>
        //public static Move SHOOT_2_WAVES;

        ///// <summary>
        ///// Shoots two 60 degree waves with a 45 degree gap in the middle.
        ///// </summary>
        //public static Move SHOOT_WAVE_MIDDLE_GAP;

        ///// <summary>
        ///// Shoots a 360 degree AOE attack with a width of 3.
        ///// </summary>
        //public static Move AOE_360;

        ///// <summary>
        ///// Shoots a 120 degree AOE attack with a width of 3.
        ///// </summary>
        //public static Move AOE_120;

        ///// <summary>
        ///// Shoots a 90 degree AOE attack with a width of 3.
        ///// </summary>
        //public static Move AOE_90;

        ///// <summary>
        ///// Fires a line of projectiles at the player, then strafes 60 degrees.
        ///// </summary>
        //public static Move LINE_STRAFE_60;

        ///// <summary>
        ///// Shoots a medium-slow 360 AOE, then strafes 60 degrees.
        ///// </summary>
        //public static Move SLOW_WAVE_CIRCLE;

        ///// <summary>
        ///// Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed 
        ///// by another 120 degree AOE; all connected.
        ///// </summary>
        //public static Move AOE_131_MEDIUM_LONG;

        ///// <summary>
        ///// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
        ///// </summary>
        //public static Move SWEEP;

        ///// <summary>
        ///// Shoots a sweep from +30 degrees to -90 degrees offset from the player's current position.
        ///// </summary>
        //public static Move SWEEP_REVERSE;

        ///// <summary>
        ///// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, 
        ///// then another from -90 to +30 degrees.
        ///// </summary>
        //public static Move SWEEP_BACK_AND_FORTH;

        ///// <summary>
        ///// Sweeps both left and right at the same time.
        ///// </summary>
        //public static Move SWEEP_BOTH;

        ///// <summary>
        ///// Shoots a projectile that splits into 6 more, going outwards from the death point.
        ///// </summary>
        //public static Move SPLIT_6;

        ///// <summary>
        ///// Shoots a projectile that splits into 6 curving projectiles.
        ///// </summary>
        //public static Move SPLIT_6_CURVE;

        //#endregion

        //#region Instantiation

        //public void Load()
        //{
        //    SHOOT_1 = new Move(
        //        1f,
        //        "SHOOT_1",
        //        "Shoots a single small, medium speed projectile at the player.",
        //        Shoot1()
        //    );

        //    SHOOT_3 = new Move(
        //        2f,
        //        "SHOOT_3",
        //        "Shoots three small, medium speed projectiles at the player.",
        //        Shoot3()
        //    );

        //    SHOOT_2_WAVES = new Move(
        //        4f,
        //        "SHOOT_2_WAVES",
        //        "Shoots 2 90 waves as one block, encouraging dodging through them.",
        //        new AISequence(
        //            Teleport().Wait(0.5f),
        //            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
        //            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
        //            .Wait(1f)
        //        )
        //    );

        //    SHOOT_WAVE_MIDDLE_GAP = new Move(
        //        3f,
        //        "SHOOT_WAVE_MIDDLE_GAP",
        //        "Shoots two 60 degree waves with a 45 degree gap in the middle.",
        //        Merge(
        //            ShootArc(150, 22.5f, 22.5f + 60f),
        //            ShootArc(150, -22.5f, -22.5f - 60f)
        //        )
        //    );

        //    AOE_360 = new Move(
        //        1.5f,
        //        "AOE_360",
        //        "Shoots a 360 degree AOE attack with a width of 3.",
        //        ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))
        //    );

        //    AOE_120 = new Move(
        //        1.5f,
        //        "AOE_360",
        //        "Shoots a 120 degree AOE attack with a width of 3.",
        //        ShootAOE(AOE.New(self).On(-60, 60f).FixedWidth(3f))
        //    );

        //    AOE_90 = new Move(
        //        1.5f,
        //        "AOE_360",
        //        "Shoots a 90 degree AOE attack with a width of 3.",
        //        ShootAOE(AOE.New(self).On(-45f, 45f).FixedWidth(3f))
        //    );

        //    LINE_STRAFE_60 = new Move(
        //        2f,
        //        "LINE_STRAFE_60",
        //        "Fires a line of projectiles at the player, then strafes 60 degrees.",
        //        new AISequence(
        //            ShootLine(50, 75f, speed: Speed.SNIPE).Wait(0.2f),
        //            Strafe(true, 60f, 50)
        //        )
        //    );

        //    SLOW_WAVE_CIRCLE = new Move(
        //        2f,
        //        "SLOW_WAVE_CIRCLE",
        //        "Shoots a medium-slow 360 AOE, then strafes 60 degrees.",
        //        new AISequence(
        //            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f)),
        //            Strafe(true, 60f, 50).Wait(0.5f)
        //        )
        //    );

        //    AOE_131_MEDIUM_LONG = new Move(
        //        5f,
        //        "AOE_131_MEDIUM_LONG",
        //        "Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed by another 120 degree AOE; all connected.",
        //        new AISequence(
        //            Teleport().Wait(0.5f),
        //            PlayerLock(true),
        //            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
        //            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
        //            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
        //            PlayerLock(false).Wait(1f)
        //        )
        //    );

        //    SWEEP = new Move(
        //        2f,
        //        "SWEEP",
        //        "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.",
        //        new AISequence(
        //            Teleport().Wait(0.25f),
        //            PlayerLock(true),
        //            new AISequence(() =>
        //            {
        //                List<AISequence> sequences = new List<AISequence>();
        //                for (int i = -30; i < 90; i += 5)
        //                {
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
        //                }
        //                return sequences.ToArray();
        //            }),
        //            PlayerLock(false),
        //            Pause(0.25f)
        //        )
        //    );

        //    SWEEP_REVERSE = new Move(
        //        2f,
        //        "SWEEP_REVERSE",
        //        "Shoots a sweep from +30 degrees to -90 degrees offset from the player's current position.",
        //        new AISequence(
        //            Teleport().Wait(0.25f),
        //            PlayerLock(true),
        //            new AISequence(() =>
        //            {
        //                List<AISequence> sequences = new List<AISequence>();
        //                for (int i = 30; i > -90; i -= 5)
        //                {
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
        //                }
        //                return sequences.ToArray();
        //            }),
        //            PlayerLock(false),
        //            Pause(0.25f)
        //        )
        //    );

        //    SWEEP_BACK_AND_FORTH = new Move(
        //        3f,
        //        "SWEEP_BACK_AND_FORTH",
        //        "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, then another from -90 to +30 degrees.",
        //        new AISequence(
        //            Teleport().Wait(0.25f),
        //            PlayerLock(true),
        //            new AISequence(() =>
        //            {
        //                List<AISequence> sequences = new List<AISequence>();
        //                for (int i = -30; i < 90; i += 5)
        //                {
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
        //                }
        //                sequences.Add(Pause(0.25f));
        //                for (int i = 30; i > -90; i -= 5)
        //                {
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i)).Wait(0.05f));
        //                }
        //                return sequences.ToArray();
        //            }),
        //            PlayerLock(false),
        //            Pause(0.5f)
        //        )
        //    );

        //    SWEEP_BOTH = new Move(
        //        4f,
        //        "SWEEP_BOTH",
        //        "Sweeps both left and right at the same time.",
        //        new AISequence(
        //            Teleport().Wait(0.25f),
        //            PlayerLock(true),
        //            new AISequence(() =>
        //            {

        //                List<AISequence> sequences = new List<AISequence>();
        //                for (int i = 0; i < 120; i += 5)
        //                {
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(i - 60)));
        //                    sequences.Add(Shoot1(Projectile.New(self).AngleOffset(60 - i)));
        //                    sequences.Add(Pause(0.05f));
        //                }
        //                return sequences.ToArray();
        //            }),
        //            PlayerLock(false).Wait(0.5f)
        //        )
        //    );

        //    SPLIT_6 = new Move(
        //        4f,
        //        "SPLIT_6",
        //        "Shoots a projectile that splits into 6 more, going outwards from the death point.",
        //        new AISequence(
        //            Teleport().Wait(0.5f),
        //            Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).OnDestroyTimeout(CallbackDictionary.SPAWN_6).Curving(0f, false)).Wait(0.5f)
        //        )
        //    );

        //    SPLIT_6_CURVE = new Move(
        //        5f,
        //        "SPLIT_6_CURVE",
        //        "Shoots a projectile that splits into 6 curving projectiles.",
        //        new AISequence(
        //            Teleport().Wait(0.5f),
        //            Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).DeathHex()).Wait(0.5f)
        //        )
        //    );
        //}

        //#endregion
    //}
}
