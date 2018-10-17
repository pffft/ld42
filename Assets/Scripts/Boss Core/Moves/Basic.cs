﻿using AI;
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
    /*
     * A collection of building blocks for more complex moves, and some
     * very simple attacks.
     */
    public class Basic : IMoveDictionary
    {
        #region Declarations

        /// <summary>
        /// Shoots a single small, medium speed projectile at the player.
        /// </summary>
        public static Move SHOOT_1;

        /// <summary>
        /// Shoots three small, medium speed projectiles at the player.
        /// </summary>
        public static Move SHOOT_3;

        /// <summary>
        /// Shoots 2 90 waves as one block, encouraging dodging through them.
        /// </summary>
        public static Move SHOOT_2_WAVES;

        /// <summary>
        /// Shoots two 60 degree waves with a 45 degree gap in the middle.
        /// </summary>
        public static Move SHOOT_WAVE_MIDDLE_GAP;

        /// <summary>
        /// Shoots a 360 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_360;

        /// <summary>
        /// Shoots a 120 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_120;

        /// <summary>
        /// Shoots a 90 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_90;

        /// <summary>
        /// Fires a line of projectiles at the player, then strafes 60 degrees.
        /// </summary>
        public static Move LINE_STRAFE_60;

        /// <summary>
        /// Shoots a medium-slow 360 AOE, then strafes 60 degrees.
        /// </summary>
        public static Move SLOW_WAVE_CIRCLE;

        /// <summary>
        /// Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed 
        /// by another 120 degree AOE; all connected.
        /// </summary>
        public static Move AOE_131_MEDIUM_LONG;

        /// <summary>
        /// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
        /// </summary>
        public static Move SWEEP;

        /// <summary>
        /// Shoots a sweep from +30 degrees to -90 degrees offset from the player's current position.
        /// </summary>
        public static Move SWEEP_REVERSE;

        /// <summary>
        /// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, 
        /// then another from -90 to +30 degrees.
        /// </summary>
        public static Move SWEEP_BACK_AND_FORTH;

        /// <summary>
        /// Sweeps both left and right at the same time.
        /// </summary>
        public static Move SWEEP_BOTH;

        /// <summary>
        /// Shoots a projectile that splits into 6 more, going outwards from the death point.
        /// </summary>
        public static Move SPLIT_6;

        /// <summary>
        /// Shoots a projectile that splits into 6 curving projectiles.
        /// </summary>
        public static Move SPLIT_6_CURVE;

        public static Move LIGHTNING_ARENA;

        public static Move LIGHTNING_WITH_AOE;

        public static Move LIGHTNING_AOE_WAVE;

        public static Move PINCER;

        #endregion

        #region Instantiation

        public void Load()
        {
            SHOOT_1 = new Move(
                1f,
                "SHOOT_1",
                "Shoots a single small, medium speed projectile at the player.",
                Shoot1()
            );

            SHOOT_3 = new Move(
                2f,
                "SHOOT_3",
                "Shoots three small, medium speed projectiles at the player.",
                Shoot3()
            );

            SHOOT_2_WAVES = new Move(
                4f,
                "SHOOT_2_WAVES",
                "Shoots 2 90 waves as one block, encouraging dodging through them.",
                new AISequence(
                    Teleport().Wait(0.5f),
                    ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
                    ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
                    .Wait(1f)
                )
            );

            SHOOT_WAVE_MIDDLE_GAP = new Move(
                3f,
                "SHOOT_WAVE_MIDDLE_GAP",
                "Shoots two 60 degree waves with a 45 degree gap in the middle.",
                Merge(
                    ShootArc(150, 22.5f, 22.5f + 60f),
                    ShootArc(150, -22.5f, -22.5f - 60f)
                )
            );

            AOE_360 = new Move(
                1.5f,
                "AOE_360",
                "Shoots a 360 degree AOE attack with a width of 3.",
                ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))
            );

            AOE_120 = new Move(
                1.5f,
                "AOE_360",
                "Shoots a 120 degree AOE attack with a width of 3.",
                ShootAOE(AOE.New(self).On(-60, 60f).FixedWidth(3f))
            );

            AOE_90 = new Move(
                1.5f,
                "AOE_360",
                "Shoots a 90 degree AOE attack with a width of 3.",
                ShootAOE(AOE.New(self).On(-45f, 45f).FixedWidth(3f))
            );

            LINE_STRAFE_60 = new Move(
                2f,
                "LINE_STRAFE_60",
                "Fires a line of projectiles at the player, then strafes 60 degrees.",
                new AISequence(
                    ShootLine(50, 75f, speed: Speed.SNIPE).Wait(0.2f),
                    Strafe(true, 60f, 50)
                )
            );

            SLOW_WAVE_CIRCLE = new Move(
                2f,
                "SLOW_WAVE_CIRCLE",
                "Shoots a medium-slow 360 AOE, then strafes 60 degrees.",
                new AISequence(
                    ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f)),
                    Strafe(true, 60f, 50).Wait(0.5f)
                )
            );

            AOE_131_MEDIUM_LONG = new Move(
                5f,
                "AOE_131_MEDIUM_LONG",
                "Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed by another 120 degree AOE; all connected.",
                new AISequence(
                    Teleport().Wait(0.5f),
                    PlayerLock(true),
                    ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
                    ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
                    ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
                    PlayerLock(false).Wait(1f)
                )
            );

            SWEEP = new Move(
                2f,
                "SWEEP",
                "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.",
                new AISequence(
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
                )
            );

            SWEEP_REVERSE = new Move(
                2f,
                "SWEEP_REVERSE",
                "Shoots a sweep from +30 degrees to -90 degrees offset from the player's current position.",
                new AISequence(
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
                )
            );

            SWEEP_BACK_AND_FORTH = new Move(
                3f,
                "SWEEP_BACK_AND_FORTH",
                "Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, then another from -90 to +30 degrees.",
                new AISequence(
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
                )
            );

            SWEEP_BOTH = new Move(
                4f,
                "SWEEP_BOTH",
                "Sweeps both left and right at the same time.",
                new AISequence(
                    Teleport().Wait(0.25f),
                    PlayerLock(true),
                    new AISequence(() =>
                    {

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
                )
            );

            SPLIT_6 = new Move(
                4f,
                "SPLIT_6",
                "Shoots a projectile that splits into 6 more, going outwards from the death point.",
                new AISequence(
                    Teleport().Wait(0.5f),
                    Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).OnDestroyTimeout(CallbackDictionary.SPAWN_6).Curving(0f, false)).Wait(0.5f)
                )
            );

            SPLIT_6_CURVE = new Move(
                5f,
                "SPLIT_6_CURVE",
                "Shoots a projectile that splits into 6 curving projectiles.",
                new AISequence(
                    Teleport().Wait(0.5f),
                    Shoot1(Projectile.New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).DeathHex()).Wait(0.5f)
                )
            );

            LIGHTNING_ARENA = new Move(
                5f,
                "LIGHTNING_ARENA",
                "Spawns lightning on the whole arena",
                new AISequence(
                    Teleport(World.Arena.CENTER).Wait(0.25f),
                    new AISequence(() => {
                        List<AISequence> sequences = new List<AISequence>();

                        for (int i = 0; i < 4; i++)
                        {
                            sequences.Add(Shoot1(
                                Projectile
                                    .New(self)
                                    .AngleOffset(i * 90f)
                                    .Lightning(0)
                                    .Speed(Speed.LIGHTNING)
                                    .MaxTime(0.05f)
                                    .OnDestroyTimeout(CallbackDictionary.LIGHTNING_RECUR)
                                ).Wait(0.1f));
                        }
                        return sequences.ToArray();
                    }).Wait(1f)
                )
            );

            LIGHTNING_WITH_AOE = new Move(
                6f,
                "LIGHTNING_WITH_AOE",
                "Test",
                new AISequence(
                    ShootAOE(AOE
                             .New(self)
                             .On(-22.5f, 22.5f)
                             .On(90 - 22.5f, 90 + 22.5f)
                             .On(180 - 22.5f, 180 + 22.5f)
                             .On(270 - 22.5f, 270 + 22.5f)
                             .AngleOffset(-25)
                             .RotationSpeed(15f)
                             .FixedWidth(10f)
                             .Speed(Speed.MEDIUM_SLOW)
                    ).Wait(1.5f),
                    LIGHTNING_ARENA.Wait(0.5f),
                    ShootAOE(AOE
                             .New(self)
                             .On(-22.5f, 22.5f)
                             .On(90 - 22.5f, 90 + 22.5f)
                             .On(180 - 22.5f, 180 + 22.5f)
                             .On(270 - 22.5f, 270 + 22.5f)
                             .AngleOffset(25)
                             .RotationSpeed(-15f)
                             .FixedWidth(10f)
                             .Speed(Speed.MEDIUM_SLOW)
                    ).Wait(1.5f),
                    LIGHTNING_ARENA.Wait(2.5f)
                )
            );

            LIGHTNING_AOE_WAVE = new Move(
                6f,
                "LIGHTNING_AOE_WAVE",
                "Test",
                new AISequence(
                    Teleport().Wait(0.5f),
                    new AISequence(() => {
                    List<AISequence> sequences = new List<AISequence>();

                    for (int i = 0; i < 7; i++) {
                        int nextAttack = Random.Range(0, 5);
                        switch(nextAttack) {
                            case 0: sequences.Add(
                                ShootArc(100, -60, 60, Projectile.New(self).Size(Size.HUGE).Speed(Speed.VERY_FAST)).Wait(0.05f).Times(3)); break;
                            case 1: sequences.Add(
                                ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f).Speed(Speed.VERY_FAST))); break;
                            case 2: sequences.Add(
                                Shoot1(Projectile.New(self).Lightning(0).Speed(Speed.LIGHTNING).MaxTime(0.05f).OnDestroyTimeout(CallbackDictionary.LIGHTNING_RECUR))); break;
                            case 3: sequences.Add(
                                Merge(
                                    ShootArc(150, 22.5f, 22.5f + 60f, Projectile.New(self).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
                                    ShootArc(150, -22.5f, -22.5f - 60f, Projectile.New(self).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
                                )); break;
                            case 4: sequences.Add(
                                ShootArc(100, -60, 60, Projectile.New(self).Speed(Speed.VERY_FAST).Size(Size.TINY)).Wait(0.1f).Times(7)
                                ); break;
                        }
                        sequences.Add(Pause(0.5f));
                    }

                    return sequences.ToArray();
                    })
                )
            );

            PINCER = new Move(
                4f,
                "PINCER",
                "Test",
                new AISequence(() =>
                {
                    List<AISequence> sequences = new List<AISequence>();

                    Speed speed = Speed.SNIPE;

                    //for (int i = 30; i < 120; i += 5)
                    float curve = 135f;
                    for (int i = 0; i < 10; i++) 
                    {
                        float curveAmount =
                            -4f * // base
                            (float)speed * // turning radius is tighter when we go faster
                            (20f / (GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude) * // contribution due to distance from player
                            Mathf.Sqrt(2) * Mathf.Sin(Mathf.Deg2Rad * curve); // contribution due to initial firing offset
                        Debug.Log(curveAmount);

                        //sequences.Add(Shoot1(Projectile.New(self).Speed(Speed.SNIPE).AngleOffset(i).Curving(curveAmount, false).MaxTime(2.2f)));
                        //sequences.Add(Shoot1(Projectile.New(self).Speed(Speed.SNIPE).AngleOffset(-i).Curving(-curveAmount, false).MaxTime(2.2f)));
                        sequences.Add(Shoot1(Projectile.New(self).Speed(Speed.SNIPE).AngleOffset(curve).Curving(curveAmount, false).MaxTime(3.35f)));
                        //sequences.Add(Shoot1(Projectile.New(self).Speed(Speed.SNIPE).AngleOffset(-curve).Curving(-curveAmount, false).MaxTime(1f)));
                        sequences.Add(Pause(0.05f));

                    /*
                     * Note that the best function found for the max time at 50 units away (y),
                     * as a function of the curvature (x), was:
                     * 
                     * on the lower end:
                     * 1 + (x/128)^3
                     * 
                     * on the higher end:
                     * 1 + tan(x/2) [in degrees]
                     * 
                     * Using a linear interpolation of these two (with bounds of [0, 180]) gives:
                     * ( ((180 - x) / 180) * (1 + (x/128)^3) ) + ( (x / 180) * (1 + tan(x/2) )
                     * 
                     * This isn't ideal. Find a better formula.
                     */

                    /*
                     * Better formula for actual max time:
                     * 360 / (2 * curve) * speed * PI * (50 / sin(curveAmount))
                     */
                    }

                    
                    //sequences.Add(Shoot1(Projectile.New(self).Speed(Speed.SNIPE).AngleOffset(-45).Curving(-curveAmount, false)));
                    return sequences.ToArray();
                }).Wait(0.5f)
            );
        }

        #endregion
    }
}
