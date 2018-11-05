using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Phases
{
    public class Phase_Tutorial_3 : AIPhase
    {
        public Phase_Tutorial_3()
        {
            MaxHealth = 20;

            AddSequence(10, new Moves.Basic.AOE_131());
            AddSequence(10, new Moves.Tutorial3.Shoot_AOE(90));
            AddSequence(10, new Moves.Tutorial3.Shoot_AOE(120));
            AddSequence(10, new Moves.Tutorial3.Shoot_AOE(360));
        }
    }
}