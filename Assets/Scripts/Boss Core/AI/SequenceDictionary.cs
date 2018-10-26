using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static BossController;
using static Projectiles.Projectile;
using static World.Arena;
using Projectiles;
using AOEs;
using BossCore;

using Moves.Basic;

// TODO: move all boss controller sequences in here
// make a JSON parser to make this job easier?
namespace AI
{
    public partial class AISequence
    {

        //public static Move DASH_TEST = new Move(
        //    8f,
        //    "DASH_TEST",
        //    "Aggressively tests dashing.",
        //    new AISequence(() => {
        //        return ShootAOE(AOE.New(self).Start(GameManager.Player.transform.position).On(0, 360f).Speed(Speed.FAST).FixedWidth(2f)).Wait(0.75f);
        //    })
        //);

        //#region Building Block Sequences

        //public static AISequence LINE_CIRCLE_STRAFE_60 = new AISequence(
        //    ShootAOE(AOE.New(self).Speed(Speed.MEDIUM_SLOW).FixedWidth(5f))
        //    .Wait(0.1f),
        //    Strafe(true, 30f, 50)
        //    .Wait(0.3f),
        //    ShootLine(50, 100f, Vector3.zero, Speed.VERY_FAST)
        //    .Wait(0.2f),
        //    Strafe(true, 30f, 50)
        //);

        //#endregion

        //#region Full Moveset Sequences

        ///*
        // * 40 basic bullets, with a 360 wave at the start, middle, and end.
        // */
        //public static Move SHOOT3_WAVE3 = new Move(
        //    3, 
        //    "SHOOT3_WAVE3",
        //    "40 basic bullets, with a 360 wave at the start, middle, and end.",
        //    new AISequence(
        //        Teleport().Wait(0.5f),
        //        AOE_360,
        //        Shoot3().Wait(0.1f).Times(20),
        //        AOE_360,
        //        Shoot3().Wait(0.1f).Times(20),
        //        AOE_360.Wait(0.5f)
        //    )
        //);

        //public static Move HEX_CURVE_INTRO = new Move(
        //    4,
        //    "HEX_CURVE_INTRO",
        //    "Introduces the player to the hex curve attack",
        //    new AISequence(
        //        ShootHexCurve(true),
        //        AOE_360.Wait(2.5f),
        //        ShootHexCurve(false),
        //        AOE_360.Wait(2.5f),
        //        ShootHexCurve(true),
        //        AOE_360,
        //        ShootHexCurve(false).Wait(1f),
        //        AOE_360.Wait(1f),
        //        AOE_360.Wait(1.5f),
        //        Teleport().Wait(0.5f)
        //    )
        //);

        //public static AISequence BIG_HOMING_STRAFE = new AISequence(
        //    CameraMove(false, new Vector3(0, 17.5f, -35f)).Wait(1f),
        //    Teleport(NORTH_FAR).Wait(1f),
        //    ShootHomingStrafe(strafeAmount: 65).Times(10),
        //    Teleport(NORTH_FAR).Wait(1f),
        //    ShootHomingStrafe(strafeAmount: 15).Times(15),
        //    CameraMove(true).Wait(2f)
        //);

        //public static AISequence DOUBLE_HEX_CURVE = new AISequence(
        //    Teleport(CENTER).Wait(1.5f),
        //    PlayerLock(true),
        //    ShootHexCurve(true),
        //    AOE_360.Wait(0.5f),
        //    ShootHexCurve(true, New(self).AngleOffset(30f)).Wait(0.5f),
        //    AOE_360.Wait(1f),
        //    AOE_360.Wait(1f),
        //    PlayerLock(false)
        //);

        //public static AISequence HOMING_STRAFE_WAVE_SHOOT = new Move(
        //    5.5f,
        //    "HOMING_STRAFE_WAVE_SHOOT",
        //    "Does a homing strafe, followed by two shoot_2_waves.",
        //    new AISequence(
        //        Teleport().Wait(0.2f),
        //        ShootHomingStrafe(strafeAmount: 15).Wait(0.01f).Times(15).Wait(0.3f), // This is hard; adding wait is reasonable
        //        SHOOT_2_WAVES.Times(2)
        //    )
        //);

        ///*
        // * A really intricate pattern. 6 projectiles that explode into 6 more projectiles,
        // * repeated twice to forme a lattice. Safe spot is a midpoint between any two of
        // * the first projectiles, near the far edge of the arena.
        // * 
        // * ** This might have changed due to the way ShootDeathHex was implemented.
        // */
        //public static AISequence DEATH_HEX = new AISequence(
        //    9, 
        //    Teleport(CENTER).Wait(0.5f),
        //    ShootDeathHex(2f).Wait(1f),
        //    ShootDeathHex(1f).Wait(2f),
        //    ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(1f),
        //    ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(1f),
        //    ShootArc(skeleton: New(self).MaxTime(0.25f)).Wait(0.75f)
        //);

        ///*
        // * Fires six slow circles around the arena in a circular pattern.
        // * Then repeats twice, with lines appearing on the left and right sides.
        // */
        //public static AISequence WAVE_CIRCLE = new AISequence(5, 
        //    Teleport(WEST_MED),
        //    SLOW_WAVE_CIRCLE.Times(6),
        //    SLOW_WAVE_CIRCLE.Times(3),
        //    ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
        //    SLOW_WAVE_CIRCLE.Times(3),
        //    ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW),
        //    SLOW_WAVE_CIRCLE.Times(3),
        //    ShootLine(50, 75f, Vector3.left, Speed.MEDIUM_SLOW),
        //    SLOW_WAVE_CIRCLE.Times(3),
        //    ShootLine(50, 75f, Vector3.right, Speed.MEDIUM_SLOW)
        //);

        //public static AISequence JUMP_ROPE_FAST = new AISequence(
        //    4, 
        //    CameraMove(false, new Vector3(0, 17.5f, -35)).Wait(1f),
        //    Teleport(WEST_FAR, 35),
        //    ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
        //    Teleport(EAST_FAR, 35),
        //    ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
        //    Teleport(WEST_FAR, 35),
        //    ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
        //    Teleport(EAST_FAR, 35),
        //    ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
        //    CameraMove(true)
        //);

        //public static AISequence DOUBLE_HEX_CURVE_HARD = new AISequence(
        //    10, 
        //    Teleport(CENTER).Wait(1f),
        //    ShootHexCurve(true, New(self).AngleOffset(0f)).Wait(0.5f),
        //    ShootHexCurve(true, New(self).AngleOffset(30f)),
        //    SHOOT3_WAVE3,
        //    Teleport(CENTER),
        //    ShootHexCurve(false, New(self).AngleOffset(0f)),
        //    ShootHexCurve(false, New(self).AngleOffset(30f)),
        //    // This homing might be too hard; especially with this amount of 360s.
        //    Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(10),
        //    AOE_360,
        //    Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(5),
        //    AOE_360,
        //    Shoot3(New(self).Size(Size.MEDIUM).Homing()).Wait(0.1f).Times(5),
        //    AOE_360.Wait(0.5f),
        //    AOE_360.Wait(0.5f)
        //);

        //public static AISequence JUMP_ROPE_SLOW_CIRCLES = new AISequence(5.5f, 
        //    Teleport(WEST_FAR),
        //    LINE_STRAFE_60.Times(6),
        //    LINE_CIRCLE_STRAFE_60.Times(6)
        //);

        //public static AISequence FOUR_WAY_SWEEP_WITH_HOMING = new AISequence(6, () =>
        //{
        //    List<AISequence> sequences = new List<AISequence>();
        //    for (int i = 0; i < 4; i++)
        //    {
        //        for (int j = 0; j < 7; j++)
        //        {
        //            sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * 6f).Size(Size.MEDIUM)).Wait(0.1f));
        //        }
        //        sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
        //        for (int j = 7; j < 15; j++)
        //        {
        //            sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * 6f).Size(Size.MEDIUM)).Wait(0.1f));
        //        }
        //        sequences.Add(AOE_360);
        //    }
        //    for (int i = 0; i < 4; i++)
        //    {
        //        for (int j = 0; j < 5; j++)
        //        {
        //            sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f).Size(Size.MEDIUM)).Wait(0.1f));
        //        }
        //        sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
        //        for (int j = 5; j < 10; j++)
        //        {
        //            sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f).Size(Size.MEDIUM)).Wait(0.1f));
        //        }
        //        sequences.Add(Shoot1(New(self).Size(Size.LARGE).Homing()));
        //        for (int j = 10; j < 15; j++)
        //        {
        //            sequences.Add(ShootArc(4, skeleton: New(self).Target(Vector3.forward).AngleOffset(j * -6f).Size(Size.MEDIUM)).Wait(0.1f));
        //        }
        //        sequences.Add(AOE_360);
        //    }
        //    return sequences.ToArray();
        //});

        //public static AISequence SWEEP_BACK_AND_FORTH_MEDIUM = new AISequence(
        //    5.5f,
        //    Teleport().Wait(0.25f),
        //    PlayerLock(true),
        //    new AISequence(() => {
        //        List<AISequence> sequences = new List<AISequence>();
        //        for (int i = -30; i < 80; i += 5)
        //        {
        //            sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
        //        }
        //        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
        //        for (int i = 80; i > -80; i -= 5)
        //        {
        //            sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(i)),
        //                Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
        //            ).Wait(0.02f));
        //        }
        //        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
        //        for (int i = -80; i < 80; i += 5)
        //        {
        //            sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
        //        }
        //        //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
        //        for (int i = 80; i > -80; i -= 5)
        //        {
        //            sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(i)),
        //                Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
        //            ).Wait(0.02f));
        //        }
        //        return sequences.ToArray();
        //    }).Wait(0.75f),
        //    PlayerLock(false)
        //);

        //public static AISequence SWEEP_BACK_AND_FORTH_ADVANCED = new AISequence(
        //    6.5f,
        //    Teleport().Wait(0.25f),
        //    PlayerLock(true),
        //    new AISequence(() => {
        //        List<AISequence> sequences = new List<AISequence>();
        //        for (int i = -30; i < 80; i += 5)
        //        {
        //            sequences.Add(Shoot1(New(self).AngleOffset(i)).Wait(0.01f));
        //        }
        //                //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
        //        for (int i = 80; i > -80; i -= 5)
        //        {
        //            sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(i)),
        //                Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
        //            ).Wait(0.02f));
        //        }
        //        for (int i = -80; i < 80; i += 5)
        //        {
        //            sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(i)),
        //                Shoot1(New(self).AngleOffset(i).Size(Size.TINY).Speed(Speed.FAST))
        //            ).Wait(0.02f));
        //        }
        //                //sequences.Add(Shoot1(speed: Speed.MEDIUM, size: Size.LARGE, type: Type.HOMING).Wait(0.01f));
        //        for (int i = 80; i > -30; i -= 5)
        //        {
        //            sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(i)),
        //                Shoot1(New(self).AngleOffset(i).Size(Size.MEDIUM).Speed(Speed.SLOW))
        //            ).Wait(0.02f));
        //        }
        //        return sequences.ToArray();
        //    }).Wait(0.75f),
        //    PlayerLock(false)
        //    );

        //public static AISequence RANDOM_200_WAVE = new AISequence(7, () => {
        //    List<AISequence> sequences = new List<AISequence>();
        //    for (int j = 0; j < 200; j++) {
        //        switch (Random.Range(0, 3))
        //        {
        //            case 0: sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.SMALL).Speed(Speed.FAST)),
        //                Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.SMALL).Speed(Speed.FAST)),
        //                Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.TINY).Speed(Speed.FAST))
        //            )); break;
        //            case 1: sequences.Add(Merge(
        //                Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.MEDIUM).Speed(Speed.MEDIUM)),
        //                Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.MEDIUM).Speed(Speed.MEDIUM)
        //            ))); break;
        //            case 2: sequences.Add(Shoot1(New(self).AngleOffset(Random.Range(0, 360f)).Size(Size.LARGE).Speed(Speed.SLOW))); break;
        //        }
        //        if (j % 20 == 0) {
        //            sequences.Add(Shoot1(New(self).Size(Size.MEDIUM).Homing()));
        //        }
        //        if (j % 40 == 0) {
        //            sequences.Add(new AISequence(() =>
        //            {
        //                Projectile
        //                    .New(self)
        //                    .Size(Size.MEDIUM)
        //                    .Speed(Speed.MEDIUM)
        //                    .AngleOffset(Random.Range(0, 360f))
        //                    .MaxTime(0.5f)
        //                    .OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)
        //                    .Create();
        //            }));
        //        }
        //        sequences.Add(Pause(0.05f));
        //    }
        //    return sequences.ToArray();
        //});

        //public static AISequence SWEEP_WALL_CLOCKWISE = new AISequence(5, () =>
        //{
        //    List<AISequence> sequences = new List<AISequence>();

        //    for (int angle = 0; angle < 72; angle += 6)
        //    {
        //        sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
        //    }

        //    for (int angle = 72; angle >= 0; angle -= 6)
        //    {
        //        sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
        //    }
        //    return sequences.ToArray();
        //});

        //public static AISequence SWEEP_WALL_COUNTERCLOCKWISE = new AISequence(5, () =>
        //{
        //    List<AISequence> sequences = new List<AISequence>();

        //    for (int angle = 0; angle >= -72; angle -= 6)
        //    {
        //        sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
        //    }

        //    for (int angle = -72; angle <= 0; angle += 6)
        //    {
        //        sequences.Add(ShootWall(angleOffset: angle).Wait(0.1f));
        //    }
        //    return sequences.ToArray();

        //});

        //public static AISequence SWEEP_WALL_BACK_AND_FORTH = new AISequence(
        //    6,
        //    PlayerLock(true),
        //    SWEEP_WALL_CLOCKWISE,
        //    SWEEP_WALL_COUNTERCLOCKWISE,
        //    PlayerLock(false)
        //);

        //public static AISequence WAVE_REVERSE_TARGET = new AISequence(
        //    5,
        //    new AISequence(() =>
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            Projectile
        //                .New(self)
        //                .Speed(Speed.FAST)
        //                .Size(Size.MEDIUM)
        //                .MaxTime(1f)
        //                .AngleOffset(i * (360f / 50f))
        //                .OnDestroyTimeout(CallbackDictionary.SPAWN_1_TOWARDS_PLAYER)
        //                .Create();
        //        }
        //    }).Wait(2f)
        //);

        //public static AISequence WAVE_REVERSE_TARGET_HOMING = new AISequence(
        //    7,
        //    new AISequence(() =>
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            Projectile
        //                .New(self)
        //                .Speed(Speed.FAST)
        //                .Size(Size.MEDIUM)
        //                .MaxTime(1f)
        //                .AngleOffset(i * (360f / 50f))
        //                .Homing()
        //                .OnDestroyTimeout(CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER)
        //                .Create();
        //        }
        //    }).Wait(2f)
        //);


        //public static AISequence WAVE_REVERSE = new AISequence(
        //    4,
        //    new AISequence(() =>
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            Projectile
        //                .New(self)
        //                .Speed(Speed.FAST)
        //                .Size(Size.MEDIUM)
        //                .MaxTime(1.5f)
        //                .AngleOffset(i * (360f / 50f))
        //                .OnDestroyTimeout(CallbackDictionary.REVERSE)
        //                .Create();
        //        }
        //    })
        //);

        //public static AISequence WAVE_REVERSE_FASTER = new AISequence(
        //    4,
        //    new AISequence(() =>
        //    {
        //        for (int i = 0; i < 50; i++)
        //        {
        //            Projectile
        //                .New(self)
        //                .Speed(Speed.FAST)
        //                .Size(Size.MEDIUM)
        //                .MaxTime(1.5f)
        //                .AngleOffset(i * (360f / 50f))
        //                .OnDestroyTimeout(CallbackDictionary.REVERSE_FASTER)
        //                .Create();
        //        }
        //    })
        //);

        //public static AISequence CIRCLE_IN_OUT = new AISequence(
        //    6,
        //    Teleport(CENTER).Wait(0.5f),
        //    WAVE_REVERSE.Wait(1.5f).Times(5).Wait(3f)
        //);

        //public static AISequence CIRCLE_JUMP_ROPE = new AISequence(
        //    8.5f,
        //    Teleport(CENTER).Wait(0.5f),
        //    WAVE_REVERSE_FASTER.Wait(1f),
        //    WAVE_REVERSE_FASTER,
        //    Shoot1(New(self).Size(Size.TINY).Speed(Speed.VERY_FAST)).Wait(0.1f).Times(60) // This pushes it a bit over 8.
        //    // Adding any of the below additional attacks makes it too hard for gameplay purposes.
        //    //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST, angleOffset: -20f).Wait(0.1f).Times(30)
        //    //Shoot3(speed: Speed.FAST, size: Size.SMALL).Wait(0.1f).Times(60)
        //    //Shoot1(size: Size.TINY, speed: Speed.VERY_FAST).Wait(0.25f).Times(6).Then(Shoot1(size: Size.MEDIUM, speed: Speed.FAST, type: Type.HOMING).Wait(0.25f)).Times(4)
        //);

        //public static AISequence SHOOT_2_WAVES_45 = new AISequence(
        //    4f,
        //    Teleport().Wait(0.25f),
        //    ShootArc(4, -45f, 45f, New(self).Size(Size.LARGE).Speed(Speed.MEDIUM_SLOW).MaxTime(1f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(2f)
        //);

        //public static AISequence SHOOT_4_WAVES_BEHIND = new AISequence(
        //    5.5f,
        //    Teleport(CENTER).Wait(0.5f),
        //    Shoot1(New(self).Size(Size.LARGE).Speed(Speed.MEDIUM_SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(220f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
        //    Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(200f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
        //    Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(160f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(0.5f),
        //    Shoot1(New(self).Size(Size.LARGE).Speed(Speed.SLOW).MaxTime(2f).Target(SOUTH_FAR).AngleOffset(140f).OnDestroyTimeout(CallbackDictionary.SPAWN_WAVE)).Wait(5f)
        //);

        //// Testing if we can modify AOE attacks at runtime (the answer is a mind-numbing yes!)
        //public static AISequence AOE_TEST = new AISequence(
        //    4f,
        //    () =>  {
        //        List<AISequence> sequences = new List<AISequence>();
        //        AOE a = AOE.New(self).Speed(Speed.MEDIUM).InnerSpeed(Speed.SNAIL).On(0, 360f);
        //        AOE.AOEComponent created = null;
        //        sequences.Add(new AISequence(() => { created = a.Create(); }));
        //        sequences.Add(Pause(2f));
        //        sequences.Add(new AISequence(() => { created.data = created.data.InnerSpeed(Speed.FROZEN).Speed(Speed.FROZEN); }));
        //        sequences.Add(Pause(10f));
        //        return sequences.ToArray();
        //    }
        //);

        //// Lets the player know the cardinal directions will be dangerous soon.
        //public static AISequence TELEGRAPH_CARDINAL = new AISequence(
        //    4f,
        //    Merge(
        //        ShootArc(100, -7, 7, New(self).Target(SOUTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -7, 7, New(self).Target(WEST_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -7, 7, New(self).Target(NORTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -7, 7, New(self).Target(EAST_FAR)).Wait(0.2f).Times(3)
        //    ),
        //    Merge(
        //        ShootArc(100, -15, 15, New(self).Target(SOUTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -15, 15, New(self).Target(WEST_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -15, 15, New(self).Target(NORTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -15, 15, New(self).Target(EAST_FAR)).Wait(0.2f).Times(3)
        //    ),
        //    Merge(
        //        ShootArc(100, -25, 25, New(self).Target(SOUTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -25, 25, New(self).Target(WEST_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -25, 25, New(self).Target(NORTH_FAR)).Wait(0.2f).Times(3),
        //        ShootArc(100, -25, 25, New(self).Target(EAST_FAR)).Wait(0.2f).Times(3)
        //    )
        //);

        ///*
        // * Shoots 4 waves at cardinal directions; when they hit the edge, they spin around
        // * slowly enough to outrun without dashing. Projectile waves come in that require shield.
        // */
        //public static AISequence AOE_TEST_2 = new AISequence(
        //    6f,
        //    TELEGRAPH_CARDINAL,

        //    ShootAOE(AOE.New(self)
        //             .Speed(Speed.MEDIUM)
        //             .InnerSpeed(Speed.SLOW)
        //             .Target(Vector3.forward)
        //             .MaxTime(2f)
        //             .On(-22.5f, 22.5f)
        //             .On(90 - 22.5f, 90 + 22.5f)
        //             .On(180 - 22.5f, 180 + 22.5f)
        //             .On(270 - 22.5f, 270 + 22.5f)
        //             .OnDestroyTimeout((self) => self.Clone().Freeze().RotationSpeed(20f).MaxTime(12.6f).Create())
        //             .OnDestroyOutOfBounds(AOECallbackDictionary.DONT_DESTROY_OOB)
        //            )
        //    .Wait(3.2f),


        //    ShootAOE(AOE.New(self)
        //             .Speed(Speed.FAST)
        //             .InnerSpeed(Speed.SNAIL)
        //             .Target(Vector3.forward)
        //             .MaxTime(1f)
        //             .On(-22.5f, 22.5f)
        //             .On(90 - 22.5f, 90 + 22.5f)
        //             .On(180 - 22.5f, 180 + 22.5f)
        //             .On(270 - 22.5f, 270 + 22.5f)
        //             .OnDestroyTimeout((self) => self.Clone().Freeze().RotationSpeed(20f).MaxTime(10.4f).Create())
        //            )
        //    .Wait(1.2f),

        //    ShootAOE(AOE.New(self)
        //             .Speed(Speed.FAST)
        //             .InnerScale(0f)
        //             .MaxTime(0.6f)
        //             .InnerSpeed(Speed.FROZEN)
        //             .Target(Vector3.forward)
        //             .On(0, 360f)
        //             .OnDestroyTimeout((self) => self.Clone().Freeze().MaxTime(9.6f).Create())
        //            )
        //    .Wait(0.6f),

        //    ShootArc(100, 0, 360).Wait(1.5f).Times(6).Wait(5f)
        //);

        //#endregion

    }
}
