using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BossController;
using Projectiles;

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
            ShootWave(50, 360f, 0f)
        );

        /*
         * Shoots 2 90 waves as one block, encouraging dodging through them.
         */
        public static AISequence SHOOT_2_WAVES = new AISequence(
            Teleport(),
            ShootWave(25, 90f, 2.5f, speed: Speed.VERY_FAST),
            ShootWave(25, 90f, -2.5f, speed: Speed.VERY_FAST)
            .Wait(1f)
        );

        /*
         * Shoots two 60 degree waves with a 45 degree gap in the middle.
         */
        public static AISequence SHOOT_WAVE_MIDDLE_GAP = AISequence.Merge(
            ShootArc(150,  22.5f,  22.5f + 60f, speed: Speed.MEDIUM),
            ShootArc(150, -22.5f, -22.5f - 60f, speed: Speed.MEDIUM)
        );

        /*
         * Fires a homing projectile, then strafes 5 degrees.
         */
        public static AISequence HOMING_STRAFE_5 = new AISequence(
            Strafe(true, 5f, 100),
            Shoot1(type: Type.HOMING, size: Size.MEDIUM)
        );

        /*
         * Fires a homing projectile, then strafes 10 degrees.
         */
        public static AISequence HOMING_STRAFE_10 = new AISequence(
            Strafe(true, 10f, 50),
            Shoot1(type: Type.HOMING, size: Size.MEDIUM)
        );

        /*
         * Fires a homing projectile, then strafes 15 degrees.
         */
        public static AISequence HOMING_STRAFE_15 = new AISequence(
            Strafe(true, 15f, 30),
            Shoot1(type: Type.HOMING, size: Size.MEDIUM)
        );

        /*
         * Fires a homing projectile, then strafes 65 degrees.
         */
        public static AISequence HOMING_STRAFE_65 = new AISequence(
            Strafe(true, 65f, 50),
            Shoot1(type: Type.HOMING, size: Size.MEDIUM)
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
            ShootWave(75, 360f, 0f, speed: Speed.MEDIUM_SLOW, size: Size.LARGE),
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
            ShootWave(75, 360f, 0f, speed: Speed.SLOW, size: Size.LARGE)
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
        public static AISequence SHOOT3_WAVE3 = new AISequence(3, 
            Teleport().Wait(0.5f),
            SHOOT_360,
            Shoot3().Wait(0.1f).Times(20),
            SHOOT_360,
            Shoot3().Wait(0.1f).Times(20),
            SHOOT_360
        );

        public static AISequence HEX_CURVE_INTRO = new AISequence(4, 
            ShootHexCurve(true),
            ShootWave(50, 360f, 0).Wait(2.5f),
            ShootHexCurve(false),
            ShootWave(50, 360f, 0).Wait(2.5f),
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
            ShootHexCurve(true),
            ShootWave(50, 360, 0f).Wait(0.5f),
            ShootHexCurve(true, angleOffset: 30f).Wait(0.5f),
            SHOOT_360.Wait(1f),
            SHOOT_360.Wait(1f)
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
         */
        public static AISequence DEATH_HEX = new AISequence(
            8, 
            Teleport(CENTER).Wait(0.5f),
            ShootDeathHex(2f).Wait(1f),
            ShootDeathHex(1f).Wait(2f),
            ShootWave(50, 360, 0, 0.25f).Wait(1f),
            ShootWave(50, 360, 0, 0.25f).Wait(1f),
            ShootWave(50, 360, 0, 0.25f).Wait(0.75f)
        );

        public static AISequence SPLIT_6 = new AISequence(
            4, () => { 
                return new AISequence(0f, () => { 
                    Projectile.Create(self, null, null, 0, 0.25f, Speed.VERY_FAST).DeathHex(); 
                // TODO: have a builder notation for this so that it's possible to override ondeath trigger like this:
                // Projectile.Create(...).DeathHex().OnDeathTrigger(() => {});
                // Maybe add an UnfinishedProjectile so that we don't build a projectile until it has enough info?
                // Maybe add delegates instead of functions to override, so you can just set the delegate?
                // ..or instead just have setters that take a delegate as an input, and call those as triggers
                }); 
            }
        );

        public static AISequence SPLIT_6_CURVE = new AISequence(
            4, () => {
                return new AISequence(0f, () => {
                    Projectile.Create(self, null, null, 0, 0.25f, Speed.VERY_FAST).DeathHex();
                });
            }
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

        public static AISequence JUMP_ROPE_FAST = new AISequence(4, 
            CameraMove(false, new Vector3(0, 17.5f, -35)).Wait(1f),
            Teleport(WEST_FAR, 200),
            ShootLine(50, 100f, speed: Speed.SNIPE),
            Teleport(EAST_FAR, 200),
            ShootLine(50, 100f, speed: Speed.SNIPE),
            Teleport(WEST_FAR, 200),
            ShootLine(50, 100f, speed: Speed.SNIPE),
            Teleport(EAST_FAR, 200),
            ShootLine(50, 100f, speed: Speed.SNIPE),
            CameraMove(true)
        );

        public static AISequence DOUBLE_HEX_CURVE_HARD = new AISequence(
            10, 
            Teleport(CENTER).Wait(1f),
            ShootHexCurve(true, angleOffset: 0f).Wait(0.5f),
            ShootHexCurve(true, angleOffset: 30f).Wait(0.5f),
            SHOOT3_WAVE3,
            Teleport(CENTER),
            ShootHexCurve(false, angleOffset: 0f),
            ShootHexCurve(false, angleOffset: 30f),
            // This homing might be too hard; especially with this amount of 360s.
            Shoot3(type: Type.HOMING, size: Size.MEDIUM).Wait(0.1f).Times(10),
            SHOOT_360,
            Shoot3(type: Type.HOMING, size: Size.MEDIUM).Wait(0.1f).Times(5),
            SHOOT_360,
            Shoot3(type: Type.HOMING, size: Size.MEDIUM).Wait(0.1f).Times(5),
            SHOOT_360.Wait(0.5f),
            SHOOT_360.Wait(0.5f)
        );

        public static AISequence test = new AISequence(
            LINE_CIRCLE_STRAFE_60.Times(6)
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
                    sequences.Add(ShootWave(4, 360, j * 6f, target: Vector3.forward).Wait(0.1f));
                }
                sequences.Add(Shoot1(type: Type.HOMING, size: Size.LARGE));
                for (int j = 7; j < 15; j++)
                {
                    sequences.Add(ShootWave(4, 360, j * 6f, target: Vector3.forward).Wait(0.1f));
                }
                sequences.Add(SHOOT_360);
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    sequences.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
                }
                sequences.Add(Shoot1(type: Type.HOMING, size: Size.LARGE));
                for (int j = 5; j < 10; j++)
                {
                    sequences.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
                }
                sequences.Add(Shoot1(type: Type.HOMING, size: Size.LARGE));
                for (int j = 10; j < 15; j++)
                {
                    sequences.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
                }
                sequences.Add(SHOOT_360);
            }
            return sequences.ToArray();
        });

        /*
         * Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
         * This doesn't lock onto the player's old position, so it will follow the player.
         */
        public static AISequence SWEEP = new AISequence(2, () =>
        {
            List<AISequence> sequences = new List<AISequence>();
            for (int i = -30; i < 90; i += 5)
            {
                sequences.Add(Shoot1(angleOffset: i).Wait(0.01f));
            }
            return sequences.ToArray();
        });

        public static AISequence SWEEP_BACK_AND_FORTH = new AISequence(3, () =>
        {
            List<AISequence> sequences = new List<AISequence>();
            for (int i = -30; i < 90; i += 5)
            {
                sequences.Add(Shoot1(angleOffset: i).Wait(0.01f));
            }
            sequences.Add(Pause(0.75f));
            for (int i = 30; i > -90; i -= 5)
            {
                sequences.Add(Shoot1(angleOffset: i).Wait(0.01f));
            }
            return sequences.ToArray();
        });

        public static AISequence RANDOM_200_WAVE = new AISequence(6, () =>
        {
            List<AISequence> sequences = new List<AISequence>();
            for (int j = 0; j < 10; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    sequences.Add(ShootWave(Random.Range(3, 9), 360, speed: Speed.SLOW, size: Size.MEDIUM));
                }
                sequences.Add(Pause(1f));
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

        public static AISequence SWEEP2 = new AISequence(5, () =>
        {
            return Merge(
                ShootArc(100, -20, -10, size: Size.SMALL), 
                ShootArc(100, 10, 90, size: Size.SMALL));
        });
        
        public static AISequence SWEEP_WALL_BACK_AND_FORTH = new AISequence(
            6,
            PlayerLock(true),
            SWEEP_WALL_CLOCKWISE,
            SWEEP_WALL_COUNTERCLOCKWISE,
            PlayerLock(false)
        );

        public static AISequence SHOOT_HARD = new AISequence(
            6,
            ShootWall(exceptMin: 10, exceptMax: 20).Wait(0.5f),
            ShootWall(exceptMin: 20, exceptMax: 30).Wait(0.5f),
            ShootWall(exceptMin: 0, exceptMax: 10).Wait(0.5f)
        );


        #endregion
    }
}
