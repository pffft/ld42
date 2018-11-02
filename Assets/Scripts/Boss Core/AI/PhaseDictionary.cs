using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

namespace AI
{
    public partial class AIPhase
    {

        public static AIPhase PHASE1;

        public static AIPhase PHASE_UNIT_TEST;

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

        /*
         * TODO: Add some form of progress indicator to this.
         */
        public static void Load() {
            //AISequence.ShouldAllowInstantiation = true;

            /*
            PHASE1 = new AIPhase()
                //.AddSequence(10, SHOOT3_WAVE3)
                .AddSequence(10, new Moves.Basic.Shoot_2_Waves())
                .AddSequence(10, new Moves.Basic.AOE_90())
                .AddSequence(10, new Moves.Basic.AOE_120())
                .AddSequence(10, new Moves.Basic.AOE_360())

                //.AddSequence(10, HEX_CURVE_INTRO)
                //.AddSequence(10, DOUBLE_HEX_CURVE)
                //.AddSequence(10, HOMING_STRAFE_WAVE_SHOOT.Times(2))
                //.AddMove(10, Moves.Basic.Definitions.SWEEP)
                //.AddSequence(10, Moves.Basic.SWEEP_BACK_AND_FORTH)
                //.AddSequence(10, SWEEP_BACK_AND_FORTH_MEDIUM)
                //.AddSequence(10, SWEEP_BACK_AND_FORTH_ADVANCED)
                //.AddSequence(10, Moves.Basic.SPLIT_6)
                //.AddSequence(10, Moves.Basic.SPLIT_6_CURVE)
                //.AddSequence(10, CIRCLE_IN_OUT)
                //.AddSequence(10, SWEEP_WALL_CLOCKWISE)
                //.AddSequence(10, SWEEP_WALL_COUNTERCLOCKWISE)
                //.AddSequence(5, SWEEP_WALL_BACK_AND_FORTH)
                //.AddSequence(10, WAVE_REVERSE) // maybe add a new color for reversal attacks
                //.AddSequence(10, WAVE_REVERSE_TARGET)
                //.AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Times(2))
                //.AddSequence(10, FOUR_WAY_SWEEP_WITH_HOMING)
                //.AddSequence(10, RANDOM_200_WAVE) // Kind of hard. Move to a later phase.
                //.AddSequence(10, SHOOT_2_WAVES_45)
                //.AddSequence(10, SHOOT_4_WAVES_BEHIND)

                ;

            PHASE_UNIT_TEST = new AIPhase()
                .AddScriptedSequence(0, new Moves.Basic.Shoot1(new Projectiles.Projectile()).Wait(1f))
                .AddScriptedSequence(1, new Moves.Basic.Shoot1(new Projectiles.ProjectileCurving(30f, true)).Wait(1f))
                .AddScriptedSequence(2, new Moves.Basic.Shoot1(new Projectiles.ProjectileCurving(-30f, false)).Wait(1f))
                .AddScriptedSequence(3, new Moves.Basic.Shoot1(new Projectiles.ProjectileHoming()).Wait(1f))
                .AddScriptedSequence(4, new Moves.Basic.Shoot1(new Projectiles.ProjectileLightning()).Wait(1f));
            ;
            */
        }

    }
}
