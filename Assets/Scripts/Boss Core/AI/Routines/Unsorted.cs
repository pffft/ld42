using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Routines
{
    public class Unsorted : AIRoutine
    {
        public Unsorted()
        {
            Phases = new List<AIPhase>
            {
                new Phases.Phase_Unsorted()
            };
        }
    }
}