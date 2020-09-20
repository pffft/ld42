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

        /*
         * A delegate used as the action for an AIEvent.
         */
        public delegate void Action();

        /*
         * Create a new AIEvent without a specified start time. The EventQueue will
         * schedule this event as the next one possible.
         */
        public AIEvent(float duration, Action action) : this(-1, duration, action) { }

        public AIEvent(float start, float duration, Action action)
        {
            this.startTime = start;
            this.duration = duration;
            this.action = action;
        }
    }
}
