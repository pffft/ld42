using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Routines
{
    public class TestLatest : AIRoutine
    {
        public TestLatest()
        {
            Phases = new List<AIPhase> {
                new Phases.Phase_Test_Latest()
            };
        }
    }
}