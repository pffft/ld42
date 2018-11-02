using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Routines
{
    public class Test : AIRoutine
    {
        public Test()
        {
            Phases = new List<AIPhase> {
                new Phases.Phase_Test()
            };
        }
    }
}