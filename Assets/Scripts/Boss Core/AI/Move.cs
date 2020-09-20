using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Move : AISequence
    {
        // Hide the parent's Children property so that no subclass can use it.
        public new AISequenceGenerator Children {
            get; private set;
        }

        public new AIEvent[] Events {
            get; private set;
        }

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
            base.Children = () => new AISequence[] { Sequence };
        }
    }
}
