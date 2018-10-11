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
            .AddSequence(10, HOMING_STRAFE_WAVE_SHOOT)
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
            .SetMaxHealth(20)
            .SetMaxArenaRadius(0.75f * 50f)
            .AddSequence(10, Moves.Basic.SWEEP.Wait(1f))
            .AddSequence(10, Moves.Basic.SWEEP_BACK_AND_FORTH.Wait(1f))
            .AddSequence(10, Moves.Basic.SWEEP_BOTH.Wait(1f))
            .AddSequence(10, Moves.Tutorial1.SHOOT_1_SEVERAL)
            .AddSequence(10, Moves.Tutorial1.SHOOT_3_SEVERAL)
            .AddSequence(3, Moves.Tutorial1.SHOOT_ARC_70)
            .AddSequence(4, Moves.Tutorial1.SHOOT_ARC_120)
            .AddSequence(3, Moves.Tutorial1.SHOOT_ARC_150)
            .AddSequence(3, Moves.Tutorial1.SHOOT_ARC_70_DENSE)
            .AddSequence(4, Moves.Tutorial1.SHOOT_ARC_120_DENSE)
            .AddSequence(3, Moves.Tutorial1.SHOOT_ARC_150_DENSE)
            ;

        /*
         * Teaches the player that the shield exists.
         * 
         * Should consist of both dashing and blocking attacks, with plenty
         * of time to throw shield between.
         */
        public static AIPhase PHASE_TUTORIAL_2 = new AIPhase()
            .SetMaxHealth(20)
            .AddSequence(10, Moves.Tutorial2.FORCE_BLOCK)
            ;

        /*
         * Introduces AOEs (and how shield interacts with them).
         */
        public static AIPhase PHASE_TUTORIAL_3 = new AIPhase()
            .SetMaxHealth(20)
            .AddSequence(10, SHOOT3_WAVE3.Wait(1f))
            .AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Wait(0.5f))
            .AddSequence(10, Moves.Tutorial3.AOE_90)
            .AddSequence(10, Moves.Tutorial3.AOE_120)
            .AddSequence(10, Moves.Tutorial3.AOE_360)
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
            .AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Times(2))
            .AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
            //.AddSequence(10, RANDOM_200_WAVE)
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
