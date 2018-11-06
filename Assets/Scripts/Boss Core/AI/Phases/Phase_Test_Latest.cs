using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Phases
{
    public class Phase_Test_Latest : AIPhase
    {
        public Phase_Test_Latest()
        {
            AddSequence(10, new Moves.Tutorial1.Shoot_1_Several());
        }
    }
}