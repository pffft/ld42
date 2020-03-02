using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AIRoutine : IEnumerator
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
        }

        public bool MoveNext() 
        {
            phaseIndex++;
            return CurrentPhase != null;
        }
        
        public object Current
        {
            get {
                return CurrentPhase;
            }
            set {
                throw new System.Exception("Can't explicitly set current phase. Use MoveNext().");
            }
        }

        public void Reset() {
            phaseIndex = -1;
        }
    }
}