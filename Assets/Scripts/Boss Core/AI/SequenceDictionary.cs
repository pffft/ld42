using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BossController;
using static AI.SequenceGenerators;
using static Projectiles.Projectile;
using Projectiles;
using BossCore;

// TODO: move all boss controller sequences in here
// make a JSON parser to make this job easier?
//
// TODO: Make the generators in BossController return an AIEvent, which then
// can be extended using calls in the following format:
// bc.Teleport().Wait(0.5f).Times(20);
namespace AI
{
    public partial class AISequence
    {

        #region Building Block Sequences

        public static AISequence SHOOT_360 = new AISequence(
            ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))
        );

        /*
         * Shoots 2 90 waves as one block, encouraging dodging through them.
         */
        public static AISequence SHOOT_2_WAVES = new AISequence(
            Teleport(),
            ShootArc(100, -45f, 45f, New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            ShootArc(100, -45f, 45f, New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
            .Wait(1f)
        );

        /*
         * Shoots two 60 degree waves with a 45 degree gap in the middle.
         */
        public static AISequence SHOOT_WAVE_MIDDLE_GAP = AISequence.Merge(
            ShootArc(150,  22.5f,  22.5f + 60f),
            ShootArc(150, -22.5f, -22.5f - 60f)
        );

        /*
         * Fires a homing projectile, then strafes 5 degrees.
         */
        public static AISequence HOMING_STRAFE_5 = new AISequence(
            Strafe(true, 5f, 100),
            //Shoot1(type: Type.HOMING, size: Size.MEDIUM)
            Shoot1(New(self).Size(Size.MEDIUM).Homing())
        );

        /*
         * Fires a homing projectile, then strafes 10 degrees.
         */
        public static AISequence HOMING_STRAFE_10 = new AISequence(
            Strafe(true, 10f, 50),
            Shoot1(New(self).Size(Size.MEDIUM).Homing())
        );

        /*
         * Fires a homing projectile, then strafes 15 degrees.
         */
        public static AISequence HOMING_STRAFE_15 = new AISequence(
            Strafe(true, 15f, 30),
            Shoot1(New(self).Size(Size.MEDIUM).Homing())
        );

        /*
         * Fires a homing projectile, then strafes 65 degrees.
         */
        public static AISequence HOMING_STRAFE_65 = new AISequence(
            Strafe(true, 65f, 50),
            Shoot1(New(self).Size(Size.MEDIUM).Homing())
        );

        /*
         * Fires a line at the player, then strafes 60 degrees.
         */
        public static AISequence LINE_STRAFE_60 = new AISequence(
            ShootLine(50, 75f, speed: Speed.SNIPE).Wait(0.2f),
            Strafe(true, 60f, 50)
        );

        /*
         * Shoots a medium-slow 360, then strafes 60 degrees.
         */
        public static AISequence SLOW_WAVE_CIRCLE = new AISequence(
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f)),
            Strafe(true, 60f, 50).Wait(0.5f)
        );

        /* 
         * Shoots a slow 360 wave, strafes 30, fires a line at center, and strafes 30 again.
         * Total strafe: 60 degrees.
         */
        /*
        public static AISequence LINE_CIRCLE_STRAFE_60 = new AISequence(
            ShootWave(75, 360f, 0f, speed: Speed.SLOW, size: Size.LARGE)
            .Wait(0.1f)
            .Then(
                Strafe(true, 30f, 50))
            .Wait(0.3f)
            .Then(
                ShootLine(50, 100f, Speed.VERY_FAST, Vector3.zero))
            .Wait(0.2f)
            .Then(
                Strafe(true, 30f, 50))
        );
        */

        public static AISequence LINE_CIRCLE_STRAFE_60 = new AISequence(
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f))
            .Wait(0.1f),
            Strafe(true, 30f, 50)
            .Wait(0.3f),
            ShootLine(50, 100f, Vector3.zero, Speed.VERY_FAST)
            .Wait(0.2f),
            Strafe(true, 30f, 50)
        );

        #endregion

        #region Full Moveset Sequences
        /*
         * 40 basic bullets, with a 360 wave at the start, middle, and end.
         */
        public static AISequence SHOOT3_WAVE3 = new AISequence(
            3, 
            Teleport().Wait(0.5f),
            SHOOT_360,
            Shoot3().Wait(0.1f).Times(20),
            SHOOT_360,
            Shoot3().Wait(0.1f).Times(20),
            SHOOT_360.Wait(0.5f) 
        );

        public static AISequence HEX_CURVE_INTRO = new AISequence(
            4, 
            ShootHexCurve(true),
            SHOOT_360.Wait(2.5f),
            ShootHexCurve(false),
            SHOOT_360.Wait(2.5f),
            ShootHexCurve(true),
            SHOOT_360,
            ShootHexCurve(false).Wait(1f),
            SHOOT_360.Wait(1f),
            SHOOT_360.Wait(1.5f),
            Teleport().Wait(0.5f),
            HOMING_STRAFE_10.Times(12),
            SHOOT_2_WAVES.Times(3)
        );

        public static AISequence BIG_HOMING_STRAFE = new AISequence(
            CameraMove(false, new Vector3(0, 17.5f, -35f)).Wait(1f),
            Teleport(NORTH_FAR).Wait(1f),
            HOMING_STRAFE_65.Times(10),
            Teleport(NORTH_FAR).Wait(1f),
            HOMING_STRAFE_15.Times(15),
            CameraMove(true).Wait(2f)
        );

        public static AISequence DOUBLE_HEX_CURVE = new AISequence(
            Teleport(CENTER).Wait(1.5f),
            PlayerLock(true),
            ShootHexCurve(true),
            SHOOT_360.Wait(0.5f),
            ShootHexCurve(true, New(self).AngleOffset(30f)).Wait(0.5f),
            SHOOT_360.Wait(1f),
            SHOOT_360.Wait(1f),
            PlayerLock(false)
        );

        public static AISequence HOMING_STRAFE_WAVE_SHOOT = new AISequence(6.5f,
            Teleport().Wait(0.2f),
            HOMING_STRAFE_15.Times(15),//.Wait(0.3f) // This is hard; adding wait is reasonable
            SHOOT_2_WAVES.Times(2)
        );

        /*
         * A really intricate pattern. 6 projectiles that explode into 6 more projectiles,
         * repeated twice to forme a lattice. Safe spot is a midpoint between any two of
         * the first projectiles, near the far edge of the arena.
         * 
         * ** This might have changed due to the way ShootDeathHex was implemented.
         */
        public static AISequence DEATH_HEX = new AISequence(
            9, 
            Teleport(CENTER).Wait(0.5f),
            ShootDeathHex(2f).Wait(1f),
            ShootDeathHex(1f).Wait(2f),
            ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(1f),
            ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(1f),
            ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(0.75f)
        );

        public static AISequence SPLIT_6 = new AISequence(
            4,
            Teleport().Wait(0.5f),
            new AISequence(() => {
                Projectile proj = Projectile.New(self)
                                            .MaxTime(0.25f)
                                            .Speed(Speed.VERY_FAST)
                                            .OnDestroyTimeout(CallbackDictionary.SPAWN_6)
                                            .Curving(0f, false)
                                            .Create();

            }).Wait(0.25f)
        );

        public static AISequence SPLIT_6_CURVE = new AISequence(
            5f, 
            Teleport().Wait(0.5f),
            Shoot1(New(self).MaxTime(0.25f).Speed(Speed.VERY_FAST).DeathHex()).Wait(0.5f)
        );

        /*
         * Fires six slow circles around the arena in a circular pattern.
         * Then repeats twice, with lines appearing on the left and right sides.
         */
        public static AISequence WAVE_CIRCLE = new AISequence(5, 
            Teleport(WEST_MED),
            SLOW_WAVE_CIRCLE.Times(6),
            SLOW_WAVE_CIRCLE.Times(3),
            ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
            SLOW_WAVE_CIRCLE.Times(3),
            ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW),
            SLOW_WAVE_CIRCLE.Times(3),
            ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
            SLOW_WAVE_CIRCLE.Times(3),
            ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW)
        );

        public static AISequence JUMP_ROPE_FAST = new AISequence(
            4, 
            CameraMove(false, new Vector3(0, 17.5f, -35)).Wait(1f),
            Teleport(WEST_FAR, 35),
            ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            Teleport(EAST_FAR, 35),
            ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            Teleport(WEST_FAR, 35),
            ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            Teleport(EAST_FAR, 35),
            ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
            CameraMove(true)
        );

        public static AISequence DOUBLE_HEX_CURVE_HARD = new AISequence(
            10, 
            Teleport(CENTER).Wait(1f),
            ShootHexCurve(true, New(self).AngleOffset(0f)).Wait(0.5f),
            ShootHexCurve(true, New(self).AngleOffset(30f)),
            SHOOT3_WAVE3,
            Teleport(CENTER),
            ShootHexCurve(false, New(self).AngleOffset(0f)),
            ShootHexCurve(false, New(self).AngleOffset(30f)),
            // This homing might be too hard; especially with this amount of 360s.
            Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(10),
            SHOOT_360,
            Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(5),
            SHOOT_360,
            Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(5),
            SHOOT_360.Wait(0.5f),
            SHOOT_360.Wait(0.5f)
        );

        public static AISequence JUMP_ROPE_SLOW_CIRCLES = new AISequence(5.5f, 
            Teleport(WEST_FAR),
            LINE_STRAFE_60.Times(6),
            LINE_CIRCLE_STRAFE_60.Times(6)
        );

        public static AISequence FOUR_WAY_SWEEP_WITH_HOMING = new AISequence(6, () =>
        {
            List<AISequence> sequences = new List<AISequence>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * 6f)).Wait(0.1f));
                }
                sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
                for (int j = 7; j < 15; j++)
                {
                    sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * 6f)).Wait(0.1f));
                }
                sequences.Add(SHOOT_360);
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f)).Wait(0.1f));
                }
                sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
                for (int j = 5; j < 10; j++)
                {
                    sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f)).Wait(0.1f));
                }
                sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
                for (int j = 10; j < 15; j++)
                {
                    sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f)).Wait(0.1f));
                }
                sequences.Add(SHOOT_360);
            }
            return sequences.ToArray();
        });

        /*
         * Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
         * This doesn't lock onto the player's old position, so it will follow the player.
         */
        public static AISequence SWEEP = new AISequence(
            2,
            Teleport(),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 90; i += 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                return sequences.ToArray();
            }).Wait(0.25f)
        );

        public static AISequence SWEEP_BACK_AND_FORTH = new AISequence(
            3,
            Teleport(), 
            new AISequence(() => {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 90; i += 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                sequences.Add(Pause(0.75f));
                for (int i = 30; i > -90; i -= 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                return sequences.ToArray();
            }).Wait(0.5f)
        );

        public static AISequence SWEEP_BACK_AND_FORTH_MEDIUM = new AISequence(
            5.5f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() => {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 80; i += 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -80; i -= 5)
                {
                    sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(i)),
                        Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
                    ).Wait(0.01f));
                }
                //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = -80; i < 80; i += 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -80; i -= 5)
                {
                    sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(i)),
                        Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
                    ).Wait(0.01f));
                }
                return sequences.ToArray();
            }).Wait(0.75f),
            PlayerLock(false)
        );

        public static AISequence SWEEP_BACK_AND_FORTH_ADVANCED = new AISequence(
            6.5f,
            Teleport().Wait(0.25f),
            PlayerLock(true),
            new AISequence(() => {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = -30; i < 80; i += 5)
                {
                    sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
                }
                        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -80; i -= 5)
                {
                    sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(i)),
                        Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
                    ).Wait(0.01f));
                }
                for (int i = -80; i < 80; i += 5)
                {
                    sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(i)),
                        Shoot1(New(self).AngleOffset(i).Size(Size.TINY).Speed(Speed.FAST))
                    ).Wait(0.01f));
                }
                        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
                for (int i = 80; i > -30; i -= 5)
                {
                    sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(i)),
                        Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
                    ).Wait(0.01f));
                }
                return sequences.ToArray();
            }).Wait(0.75f),
            PlayerLock(false)
            );

        public static AISequence RANDOM_200_WAVE = new AISequence(5, () => {
            List<AISequence> sequences = new List<AISequence>();
            for (int j = 0; j < 200; j++) {
                switch (Random.Range(0, 3))
                {
                    case 0: sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.SMALL).Speed(Speed.FAST)),
                        Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.SMALL).Speed(Speed.FAST)),
                        Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.TINY).Speed(Speed.FAST))
                    )); break;
                    case 1: sequences.Add(Merge(
                        Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.MEDIUM).Speed(Speed.MEDIUM)),
                        Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.MEDIUM).Speed(Speed.MEDIUM)
                    ))); break;
                    case 2: sequences.Add(Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.LARGE).Speed(Speed.SLOW))); break;
                }
                if (j % 20 == 0) {
                    sequences.Add(Shoot1(New(self).Size(Size.MEDIUM).Homing()));
                }
                if (j % 40 == 0) {
                    sequences.Add(new AISequence(() =>
                    {
                        Projectile
                            .New(self)
                            .Size(Size.MEDIUM)
                            .Speed(Speed.MEDIUM)
                            .AngleOffset(Random.Range(0, 360f))
                            .MaxTime(0.5f)
                            .OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)
                            .Create();
                    }));
                }
                sequences.Add(Pause(0.05f));
            }
            return sequences.ToArray();
        });

        public static AISequence SWEEP_WALL_CLOCKWISE = new AISequence(5, () =>
        {
            List<AISequence> sequences = new List<AISequence>();

            for (int angle = 0; angle < 72; angle += 6)
            {
                sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
            }

            for (int angle = 72; angle >= 0; angle -= 6)
            {
                sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
            }
            return sequences.ToArray();
        });

        public static AISequence SWEEP_WALL_COUNTERCLOCKWISE = new AISequence(5, () =>
        {
            List<AISequence> sequences = new List<AISequence>();

            for (int angle = 0; angle >= -72; angle -= 6)
            {
                sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
            }

            for (int angle = -72; angle <= 0; angle += 6)
            {
                sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
            }
            return sequences.ToArray();

        });
        
        public static AISequence SWEEP_WALL_BACK_AND_FORTH = new AISequence(
            6,
            PlayerLock(true),
            SWEEP_WALL_CLOCKWISE,
            SWEEP_WALL_COUNTERCLOCKWISE,
            PlayerLock(false)
        );

        public static AISequence WAVE_REVERSE_TARGET = new AISequence(
            5,
            new AISequence(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Projectile
                        .New(self)
                        .Speed(Speed.FAST)
                        .Size(Size.MEDIUM)
                        .MaxTime(1f)
                        .AngleOffset(i * (360f / 50f))
                        .OnDestroyTimeout(CallbackDictionary.SPAWN_1_TOWARDS_PLAYER)
                        .Create();
                }
            }).Wait(2f)
        );

        public static AISequence WAVE_REVERSE_TARGET_HOMING = new AISequence(
            7,
            new AISequence(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Projectile
                        .New(self)
                        .Speed(Speed.FAST)
                        .Size(Size.MEDIUM)
                        .MaxTime(1f)
                        .AngleOffset(i * (360f / 50f))
                        .Homing()
                        .OnDestroyTimeout(CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER)
                        .Create();
                }
            }).Wait(2f)
        );


        public static AISequence WAVE_REVERSE = new AISequence(
            4,
            new AISequence(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Projectile
                        .New(self)
                        .Speed(Speed.FAST)
                        .Size(Size.MEDIUM)
                        .MaxTime(1.5f)
                        .AngleOffset(i * (360f / 50f))
                        .OnDestroyTimeout(CallbackDictionary.REVERSE)
                        .Create();
                }
            })
        );
                
        public static AISequence WAVE_REVERSE_FASTER = new AISequence(
            4,
            new AISequence(() =>
            {
                for (int i = 0; i < 50; i++)
                {
                    Projectile
                        .New(self)
                        .Speed(Speed.FAST)
                        .Size(Size.MEDIUM)
                        .MaxTime(1.5f)
                        .AngleOffset(i * (360f / 50f))
                        .OnDestroyTimeout(CallbackDictionary.REVERSE_FASTER)
                        .Create();
                }
            })
        );

        public static AISequence CIRCLE_IN_OUT = new AISequence(
            6,
            Teleport(CENTER).Wait(0.5f),
            WAVE_REVERSE.Wait(1.5f).Times(5).Wait(3f)
        );

        public static AISequence CIRCLE_JUMP_ROPE = new AISequence(
            8.5f,
            Teleport(CENTER).Wait(0.5f),
            WAVE_REVERSE_FASTER.Wait(1f),
            WAVE_REVERSE_FASTER,
            Shoot1(New(self).Size(Size.TINY).Speed(Speed.VERY_FAST)).Wait(0.1f).Times(60) // This pushes it a bit over 8.
            // Adding any of the below additional attacks makes it too hard for gameplay purposes.
            //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST, angleOffset: -20f).Wait(0.1f).Times(30)
            //Shoot3(speed: Speed.FAST, size: Size.SMALL).Wait(0.1f).Times(60)
            //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST).Wait(0.25f).Times(6).Then(Shoot1(size: Size.MEDIUM, speed: Speed.FAST, type: Type.HOMING).Wait(0.25f)).Times(4)
        );

        public static AISequence AOE_TEST = new AISequence(
            3f,
            new AISequence(() => {
                AOE.New(self).AngleOffset(60f);
            }).Wait(0.1f)
        /*
        new AISequence(() => {
            AOE.Create(self, true).Speed(15f);
        }).Wait(0.5f)
        */
        );

        public static AISequence AOE_131_MEDIUM = new AISequence(
            Teleport().Wait(0.5f),
            PlayerLock(true),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(10)).Wait(0.38f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
            PlayerLock(false).Wait(1f)
        );

        public static AISequence AOE_131_FAST = new AISequence(
            Teleport().Wait(0.5f),
            PlayerLock(true),
            ShootAOE(AOE.New(self).Speed(Speed.FAST).On(-60, 60).FixedWidth(5)).Wait(0.135f),
            ShootAOE(AOE.New(self).Speed(Speed.FAST).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(10)).Wait(0.27f),
            ShootAOE(AOE.New(self).Speed(Speed.FAST).On(-60, 60).FixedWidth(5)).Wait(0.135f),
            PlayerLock(false).Wait(1f)
        );


        public static AISequence AOE_131_MEDIUM_LONG = new AISequence(
            Teleport().Wait(0.5f),
            PlayerLock(true),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.19f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, -40).On(-10, 10).On(40, 60).FixedWidth(20)).Wait(0.76f),
            ShootAOE(AOE.New(self).Speed(Speed.MEDIUM).On(-60, 60).FixedWidth(5)).Wait(0.2f),
            PlayerLock(false).Wait(1f)
        );

        #endregion

    }
}
