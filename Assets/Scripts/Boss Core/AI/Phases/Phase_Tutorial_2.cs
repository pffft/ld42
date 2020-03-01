using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Phases
{
    public class Phase_Tutorial_2 : AIPhase
    {
        public Phase_Tutorial_2()
        {
            MaxHealth = 20;

            AddSequence(10, new Moves.Tutorial2.Force_Block());
        }
    }
}