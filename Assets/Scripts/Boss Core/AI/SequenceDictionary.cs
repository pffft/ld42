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

        //public static AISequence JUMP_ROPE_SLOW_CIRCLES = new AISequence(5.5f, 
        //    Teleport(WEST_FAR),
        //    LINE_STRAFE_60.Times(6),
        //    LINE_CIRCLE_STRAFE_60.Times(6)
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
