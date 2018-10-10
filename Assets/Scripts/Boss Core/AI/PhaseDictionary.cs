using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AI.AISequence;
using static AI.SequenceGenerators;

namespace AI
{
    public partial class AIPhase
    {

        public static AIPhase PHASE_TEST = new AIPhase()
            .AddSequence(10, DASH_TEST)
            ;

        /*
         * Teaches the player basic movement and throwing.
         * 
         * Should consist of moves that are easy to dodge by running or dashing,
         * and have plenty of time in between to throw a shield.
         * 
         * Should probably disable shield blocking for this phase.
         */
        public static AIPhase PHASE_TUTORIAL_1 = new AIPhase()
            .AddSequence(10, Moves.Basic.SWEEP.Wait(1f))
            .AddSequence(10, Moves.Basic.SWEEP_BACK_AND_FORTH.Wait(1f))
            .AddSequence(10, Moves.Basic.SWEEP_BOTH.Wait(1f))
            .AddSequence(10, Teleport().Wait(0.25f).Then(Shoot1().Wait(0.1f).Times(7)).Wait(1.5f))
            .AddSequence(10, Teleport().Wait(0.25f).Then(Shoot3().Wait(0.1f).Times(15)).Wait(1.5f))
            .AddSequence(3, Teleport().Wait(0.25f).Then(ShootArc(50, -35, 35).Wait(1.5f)))
            .AddSequence(4, Teleport().Wait(0.25f).Then(ShootArc(50, -60, 60).Wait(1.5f)))
            .AddSequence(3, Teleport().Wait(0.25f).Then(ShootArc(50, -75, 75).Wait(1.5f)))
            .AddSequence(3, Teleport().Wait(0.25f).Then(ShootArc(100, -35, 35).Wait(1.5f)))
            .AddSequence(4, Teleport().Wait(0.25f).Then(ShootArc(100, -60, 60).Wait(1.5f)))
            .AddSequence(3, Teleport().Wait(0.25f).Then(ShootArc(100, -75, 75).Wait(1.5f)))
            ;

        /*
         * Teaches the player that the shield exists.
         * 
         * Should consist of both dashing and blocking attacks, with plenty
         * of time to throw shield between.
         */
        public static AIPhase PHASE_TUTORIAL_2 = new AIPhase()
            .AddSequence(10, Teleport().Wait(0.25f).Then(ShootArc(100, -90, 90).Wait(0.1f).Times(10)))
            ;

        /*
         * Introduces AOEs (and how shield interacts with them).
         */
        public static AIPhase PHASE_TUTORIAL_3 = new AIPhase()
            ;

        public static AIPhase PHASE1 = new AIPhase()
            .AddSequence(10, SHOOT3_WAVE3)
            .AddSequence(10, Moves.Basic.SHOOT_2_WAVES.Times(5))
            .AddSequence(10, HEX_CURVE_INTRO)
            .AddSequence(10, DOUBLE_HEX_CURVE)
            .AddSequence(10, HOMING_STRAFE_WAVE_SHOOT.Times(2))
            .AddSequence(10, Moves.Basic.SWEEP)
            .AddSequence(10, Moves.Basic.SWEEP_BACK_AND_FORTH)
            .AddSequence(10, SWEEP_BACK_AND_FORTH_MEDIUM)
            .AddSequence(10, SWEEP_BACK_AND_FORTH_ADVANCED)
            .AddSequence(10, Moves.Basic.SPLIT_6)
            .AddSequence(10, Moves.Basic.SPLIT_6_CURVE)
            .AddSequence(10, CIRCLE_IN_OUT)
            .AddSequence(10, SWEEP_WALL_CLOCKWISE)
            .AddSequence(10, SWEEP_WALL_COUNTERCLOCKWISE)
            .AddSequence(5, SWEEP_WALL_BACK_AND_FORTH)
            .AddSequence(10, WAVE_REVERSE) // maybe add a new color for reversal attacks
            .AddSequence(10, WAVE_REVERSE_TARGET)
            //.AddSequence(10, WAVE_REVERSE_TARGET_HOMING) // is this too hard? 
            .AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Times(2))
            //.AddSequence(10, CIRCLE_JUMP_ROPE) // Too hard! Esp. with small arena
            .AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
            .AddSequence(10, RANDOM_200_WAVE)
            .AddSequence(10, SHOOT_2_WAVES_45)
            .AddSequence(10, SHOOT_4_WAVES_BEHIND)

            ;

        //static AIPhase() {
        //    if (BossController.insaneMode)
        //    {
        //        PHASE1
        //            .AddSequence(10, DOUBLE_HEX_CURVE_HARD)
        //            .AddSequence(10, DEATH_HEX)
        //            .AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
        //            .AddSequence(10, RANDOM_200_WAVE)
        //            .AddSequence(10, CIRCLE_JUMP_ROPE)
        //            ;
        //    }
        //}

        //public static AIPhase HARD_PHASE = new AIPhase()
        ////.AddSequence(10, WAVE_CIRCLE)
        //.AddSequence(10, DOUBLE_HEX_CURVE_HARD)
        //.AddSequence(10, DEATH_HEX)
        ////.AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
        //.AddSequence(10, RANDOM_200_WAVE)
        //.AddSequence(10, CIRCLE_JUMP_ROPE)
        //;

    }
}
