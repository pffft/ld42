using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AIRoutine 
    {
        public List<AIPhase> Phases { get; protected set; }
        public AIPhase CurrentPhase
        {
            get
            {
                return phaseIndex > Phases.Count ? null : Phases[phaseIndex];
            }
        }
        private int phaseIndex = -1;

        public AIRoutine() {
            Phases = new List<AIPhase>();
            //AISequence.ShouldAllowInstantiation = true;
        }

        public AIPhase NextPhase() {
            phaseIndex++;
            return CurrentPhase;
        }

        public void Reset() {
            phaseIndex = -1;
        }
    }
}