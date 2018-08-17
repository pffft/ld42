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
            if (times == 0)
            {
                times = 1;
                Debug.LogError("Cannot repeat event 0 times");
            }
                
            AIEvent[] events = new AIEvent[times];
            for (int i = 0; i < times; i++)
            {
                events[i] = this;
            }
            AISequence sequence = new AISequence(events);
            return sequence;
        }

        /*
         * Returns this event, with a specified delay afterwards.
         * Equivalent to new AIEvent(duration, this.action, this.parameters).
         */
        public AIEvent Wait(float duration)
        {
            this.duration = duration;
            return this;
        }

        /*
         * Returns a new AISequence representing this event, followed by the
         * specified next event.
         */
        public AISequence Then(AIEvent nextEvent)
        {
            return new AISequence(this, nextEvent);
        }
            
    }
}