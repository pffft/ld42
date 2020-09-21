using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Routines
{
    public class Main : AIRoutine
    {
        public Main()
        {
            // TODO finish me (tutorial + main phase)
            Phases = new List<AIPhase> {
                new Phases.Phase_1()
            };
        }
    }
}