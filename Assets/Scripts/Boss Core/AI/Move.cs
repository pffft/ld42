using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Move : AISequence
    {
        private AISequence _sequence;
        public AISequence Sequence 
        { 
            get { 
                if (_sequence == null) {
                    throw new System.Exception("Move created without a Sequence defined. " +
                                               "Either set \"Sequence\" in the constructor or use the base(...) notation.");
                }
                return _sequence; 
            }

            protected set { _sequence = value; } 
        }

        public Move() {
            this.GetChildren = () => new AISequence[] { Sequence };
        }

        // TODO this is provided for backwards compatibility with the base(...)
        // style of building moves. Later we should move away from it.
        [System.Obsolete]
        public Move(params AISequence[] sequences) : base(sequences) { 
            Debug.LogWarning("Using the base(...) notation is deprecated! Name: " + Name); 
        }

        // Hide the AISequence constructors so that we get a compilation error for using them
        private Move(AIEvent[] events) { }
        private Move(GenerateSequence sequence) { }
        private Move(GenerateSequences sequence) { }
    }
}
