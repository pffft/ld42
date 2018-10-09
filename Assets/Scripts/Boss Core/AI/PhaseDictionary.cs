using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AI.AISequence;

namespace AI
{
    public partial class AIPhase
    {

        public static AIPhase PHASE_TEST = new AIPhase()
            .AddSequence(10, DASH_TEST)
            ;

        /*
         * Teaches the player basic movement and throwing (to advance).
         */
        public static AIPhase PHASE_TUTORIAL_1 = new AIPhase()
            .AddSequence(10, Moves.Basic.SHOOT_1)
            ;


        public static AIPhase PHASE1 = new AIPhase()
            .AddSequence(10, SHOOT3_WAVE3)
            .AddSequence(10, Moves.Basic.SHOOT_2_WAVES.Times(5))
            .AddSequence(10, HEX_CURVE_INTRO)
            .AddSequence(10, DOUBLE_HEX_CURVE)
            .AddSequence(10, HOMING_STRAFE_WAVE_SHOOT.Times(2))
            .AddSequence(10, SWEEP)
            .AddSequence(10, SWEEP_BACK_AND_FORTH)
            .AddSequence(10, SWEEP_BACK_AND_FORTH_MEDIUM)
            .AddSequence(10, SWEEP_BACK_AND_FORTH_ADVANCED)
            .AddSequence(10, SPLIT_6)
            .AddSequence(10, SPLIT_6_CURVE)
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
