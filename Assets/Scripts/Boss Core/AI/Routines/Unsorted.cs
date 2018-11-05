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
                new AIPhase().AddSequence(10, new Moves.Unsorted.Double_Hex_Curve_Hard())
            };
        }
    }
}