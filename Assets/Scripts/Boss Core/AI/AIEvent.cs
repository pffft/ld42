using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AIEvent
    {
        public float startTime;
        public float duration;
        public Action action;
        public object[] parameters;

        public delegate void Action();

        /*
         * Create a new AIEvent without a specified start time. The EventQueue will
         * schedule this event as the next one possible.
         */
        public AIEvent(float duration, Action action, params object[] parameters) :
            this(-1, duration, action, parameters)
        { }

        public AIEvent(float start, float duration, Action action, params object[] parameters)
        {
            this.startTime = start;
            this.duration = duration;
            this.action = action;
            this.parameters = parameters;
        }

        /*
         * Duplicate this event "times" number of times.
         * Returns an AISequence representing such a sequence of events.
         */
        public AISequence Times(int times)
        {
            AIEvent[] events = new AIEvent[times];
            for (int i = 0; i < times; i++)
            {
                events[i] = this;
            }
            AISequence sequence = new AISequence();
            sequence.events = events;
            return sequence;
        }
    }
}