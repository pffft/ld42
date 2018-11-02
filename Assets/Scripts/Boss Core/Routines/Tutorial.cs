using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Routines
{
    public class Tutorial : AIRoutine
    {
        public Tutorial()
        {
            Phases = new List<AIPhase> {
                new Phases.Phase_Tutorial_1(),
                new Phases.Phase_Tutorial_2(),
                new Phases.Phase_Tutorial_3()
            };
        }
    }
}