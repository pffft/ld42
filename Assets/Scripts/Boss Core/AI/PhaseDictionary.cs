using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

using static AI.AISequence;

using System.Reflection;
using System.Linq;

namespace AI
{
    public partial class AIPhase
    {

        //public static AIPhase PHASE_TEST = new AIPhase()
        //.AddSequence(10, HOMING_STRAFE_WAVE_SHOOT)
        //   ;
        public static AIPhase PHASE_TEST;

        /*
         * Teaches the player basic movement and throwing.
         * 
         * Should consist of moves that are easy to dodge by running or dashing,
         * and have plenty of time in between to throw a shield.
         * 
         * Should probably disable shield blocking for this phase.
         */
        public static AIPhase PHASE_TUTORIAL_1;

        /*
         * Teaches the player that the shield exists.
         * 
         * Should consist of both dashing and blocking attacks, with plenty
         * of time to throw shield between.
         */
        public static AIPhase PHASE_TUTORIAL_2;

        /*
         * Introduces AOEs (and how shield interacts with them).
         */
        public static AIPhase PHASE_TUTORIAL_3;

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
         * TODO: Make this automatically check the "Moves" namespace and instantiate/load
         * all the classes within it (except Move and IMoveDictionary). Make it throw warnings
         * if a Move is declared in a class but never initialized (null), and if a Move is
         * initialized before it's loaded through Load().
         * 
         * TODO: Add some form of progress indicator to this.
         */
        public static void Load() {
            AISequence.ShouldAllowInstantiation = true;

            PHASE_TEST = new AIPhase()
                //.AddSequence(10, Moves.Basic.PINCER)
                //.AddSequence(10, new Moves.Test.Lightning_Arena().Times(2))
                //.AddSequence(10, new Moves.Test.Quick_Waves())
                //.AddSequence(10, new Moves.Test.Double_Laser_Sweep_AOE())
                //.AddSequence(10, new Moves.Test.Double_Laser_Sweep())
                //.AddSequence(10, new Moves.Test.Pincer_Sweep())
                //.AddSequence(10, new Moves.Test.Test())
                ;

            PHASE_TUTORIAL_1 = new AIPhase()
                .SetMaxHealth(20)
                .SetMaxArenaRadius(0.75f * 50f)
                .AddSequence(10, new Moves.Basic.Sweep().Wait(1f))
                .AddSequence(10, new Moves.Basic.Sweep(reverse: true).Wait(1f))
                .AddSequence(10, new Moves.Basic.Sweep_Back_And_Forth().Wait(1f))
                .AddSequence(10, new Moves.Basic.Sweep_Both().Wait(1f))
                .AddSequence(10, new Moves.Tutorial1.Shoot_1_Several())
                .AddSequence(10, new Moves.Tutorial1.Shoot_3_Several())
                .AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70))
                .AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120))
                .AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150))
                .AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70, true))
                .AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120, true))
                .AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150, true))
                ;

            PHASE_TUTORIAL_2 = new AIPhase()
                .SetMaxHealth(20)
                .AddSequence(10, new Moves.Tutorial2.Force_Block())
                ;

            PHASE_TUTORIAL_3 = new AIPhase()
                .SetMaxHealth(20)
                //.AddSequence(10, SHOOT3_WAVE3.Wait(1f))
                //.AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Wait(0.5f))
                .AddSequence(10, new Moves.Tutorial3.Shoot_AOE(90))
                .AddSequence(10, new Moves.Tutorial3.Shoot_AOE(120))
                .AddSequence(10, new Moves.Tutorial3.Shoot_AOE(360))
                ;

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
        }

    }
}
