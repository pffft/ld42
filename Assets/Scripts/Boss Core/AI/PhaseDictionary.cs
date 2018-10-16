﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

using static AI.AISequence;
using static AI.SequenceGenerators;

using System.Reflection;
using System.Linq;

namespace AI
{
    public partial class AIPhase
    {

        public static AIPhase PHASE_TEST = new AIPhase()
            //.AddSequence(10, HOMING_STRAFE_WAVE_SHOOT)
            ;

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

            Profiler.BeginSample("Loading Moves");
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Grab all the distinct namespaces in this project
            List<string> namespaces = assembly
                .GetTypes()
                .Select(t => t.Namespace)
                .Distinct()
                .ToList();

            foreach (string ns in namespaces) {
                // Select the ones that aren't default, and start with "Moves"
                if (ns != null && ns.StartsWith("Moves", System.StringComparison.Ordinal)) {

                    // Exclude the base "Moves" namespace itself
                    if (ns.Equals("Moves")) {
                        continue;
                    }

                    // Ensure that namespace has a Definitions file
                    // TODO: note that we technically don't need a definitions file; that just gives us the
                    // compile-time reassurance of existence. We can still look into the directory and load in
                    // and moves as needed, though accessing those moves without a move dictionary might be hard.
                    System.Type type = assembly.GetType(ns + ".Definitions");
                    if (type == null) {
                        Debug.LogError("Found Move namespace with missing \"Definitions\" file: " + ns);
                        continue;
                    }

                    // Ensure the Definitions file is a valid MoveLoader
                    MoveLoader loader = System.Activator.CreateInstance(type) as MoveLoader;
                    if (loader == null) {
                        Debug.LogError("Found \"Definitions\" file that is not a MoveLoader type: " + ns);
                        continue;
                    }

                    // Load it up!
                    loader.Load();
                }
            }

            Profiler.EndSample();

            //new Moves.Basic.Definitions().Load();

            PHASE_TUTORIAL_1 = new AIPhase()
                .SetMaxHealth(20)
                .SetMaxArenaRadius(0.75f * 50f)
                //.AddSequence(10, Moves.Basic.Definitions.SHOOT_1)
                //.AddSequence(10, SHOOT3_WAVE3.Wait(1f))
                .AddSequence(10, Moves.Basic.Definitions.SWEEP.Wait(1f))
                //.AddSequence(10, Moves.Basic.SWEEP_BACK_AND_FORTH.Wait(1f))
                //.AddSequence(10, Moves.Basic.SWEEP_BOTH.Wait(1f))
                //.AddSequence(10, Moves.Tutorial1.SHOOT_1_SEVERAL)
                //.AddSequence(10, Moves.Tutorial1.SHOOT_3_SEVERAL)
                //.AddSequence(3, Moves.Tutorial1.SHOOT_ARC_70)
                //.AddSequence(4, Moves.Tutorial1.SHOOT_ARC_120)
                //.AddSequence(3, Moves.Tutorial1.SHOOT_ARC_150)
                //.AddSequence(3, Moves.Tutorial1.SHOOT_ARC_70_DENSE)
                //.AddSequence(4, Moves.Tutorial1.SHOOT_ARC_120_DENSE)
                //.AddSequence(3, Moves.Tutorial1.SHOOT_ARC_150_DENSE)
                ;

            PHASE_TUTORIAL_2 = new AIPhase()
                .SetMaxHealth(20)
                //.AddSequence(10, Moves.Tutorial2.FORCE_BLOCK)
                ;

            PHASE_TUTORIAL_3 = new AIPhase()
                .SetMaxHealth(20)
                //.AddSequence(10, SHOOT3_WAVE3.Wait(1f))
                //.AddSequence(10, Moves.Basic.AOE_131_MEDIUM_LONG.Wait(0.5f))
                //.AddSequence(10, Moves.Tutorial3.AOE_90)
                //.AddSequence(10, Moves.Tutorial3.AOE_120)
                //.AddSequence(10, Moves.Tutorial3.AOE_360)
                ;

            PHASE1 = new AIPhase()
                //.AddSequence(10, SHOOT3_WAVE3)
                .AddSequence(10, Moves.Basic.Definitions.SHOOT_2_WAVES)
                .AddSequence(10, Moves.Basic.Definitions.AOE_90)
                .AddSequence(10, Moves.Basic.Definitions.AOE_120)
                .AddSequence(10, Moves.Basic.Definitions.AOE_360)

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
        }

    }
}
