using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AI.AISequence;

namespace AI
{
    public partial class AIPhase
    {

        /*
         * 
        eventQueue.AddSequence(AISequence.SHOOT3_WAVE3);
        eventQueue.AddSequence(AISequence.SHOOT3_WAVE3);

        eventQueue.AddSequence(AISequence.SHOOT_2_WAVES.Times(5));

        eventQueue.AddSequence(AISequence.HEX_CURVE_INTRO);

        eventQueue.AddSequence(AISequence.BIG_HOMING_STRAFE);

        eventQueue.AddSequence(AISequence.DOUBLE_HEX_CURVE);

        eventQueue.AddSequence(AISequence.HOMING_STRAFE_WAVE_SHOOT.Times(2));

        eventQueue.AddSequence(AISequence.DEATH_HEX);

        eventQueue.AddSequence(AISequence.WAVE_CIRCLE);

        eventQueue.AddSequence(AISequence.JUMP_ROPE_FAST);

        eventQueue.AddSequence(AISequence.DOUBLE_HEX_CURVE_HARD);

        eventQueue.AddSequence(AISequence.JUMP_ROPE_SLOW_CIRCLES);
        */

        public static AIPhase PHASE1 = new AIPhase()
            .AddSequence(10, SHOOT3_WAVE3)
            .AddSequence(10, SHOOT_2_WAVES.Times(5))
            .AddSequence(10, HEX_CURVE_INTRO)
            //.AddSequence(10, BIG_HOMING_STRAFE)
            .AddSequence(10, DOUBLE_HEX_CURVE)
            .AddSequence(10, HOMING_STRAFE_WAVE_SHOOT.Times(2))
            //.AddSequence(10, DEATH_HEX)
            .AddSequence(10, SWEEP)
            .AddSequence(10, SWEEP_BACK_AND_FORTH)
            .AddSequence(10, SWEEP_BACK_AND_FORTH_ADVANCED)
            .AddSequence(10, SPLIT_6)
            .AddSequence(10, SPLIT_6_CURVE)
            .AddScriptedSequence(5, JUMP_ROPE_FAST);

        public static AIPhase HARD_PHASE = new AIPhase()
            //.AddSequence(10, WAVE_CIRCLE)
            //.AddSequence(10, DOUBLE_HEX_CURVE_HARD)
            //.AddSequence(10, DEATH_HEX)
            //.AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
            .AddSequence(10, RANDOM_200_WAVE)
            ;
    }
}
