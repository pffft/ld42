using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Phases
{
    public class Phase_Tutorial_1 : AIPhase
    {
        public Phase_Tutorial_1()
        {
            MaxHealth = 20;
            MaxArenaRadius = 0.75f * 50f;

            AddSequence(10, new Moves.Basic.Sweep().Wait(1f));
            AddSequence(10, new Moves.Basic.Sweep(reverse: true).Wait(1f));
            AddSequence(10, new Moves.Basic.Sweep_Back_And_Forth().Wait(1f));
            AddSequence(10, new Moves.Basic.Sweep_Both().Wait(1f));
            AddSequence(10, new Moves.Tutorial1.Shoot_1_Several());
            AddSequence(10, new Moves.Tutorial1.Shoot_3_Several());
            AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70));
            AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120));
            AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150));
            AddSequence(3, new Moves.Tutorial1.Shoot_Arc(70, true));
            AddSequence(4, new Moves.Tutorial1.Shoot_Arc(120, true));
            AddSequence(3, new Moves.Tutorial1.Shoot_Arc(150, true));
        }
    }
}