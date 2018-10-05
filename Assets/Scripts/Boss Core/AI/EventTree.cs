using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class EventTree
    {
        private static AISequence currentSequence;
        private static Queue<AIEvent> events;

        public void SetNext(AISequence sequence) {
            currentSequence = sequence;
        }

        public void Add(AISequence sequence) {
            AISequence[] children = currentSequence.GetChildren();
            if (children != null)
            {
                for (int i = 0; i < children.Length; i++) 
                {
                    Add(children[i]);
                }
            }

            if (sequence.events != null) {
                for (int i = 0; i < sequence.events.Length; i++) {
                    Add(sequence.events[i]);
                }
            }
        }

        public void Add(AIEvent e) {

        }

        public void Step() {
        }
    }
}
