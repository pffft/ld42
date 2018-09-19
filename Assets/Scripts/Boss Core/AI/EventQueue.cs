using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace AI
{
    public class EventQueue
    {

        private Entity entity;
        private Queue<AIEvent> events;
        private AIEvent lastEvent;
        private float lastTime;

        private float internalTime;
        private bool paused;

        public EventQueue(Entity reference)
        {
            this.entity = reference;

            events = new Queue<AIEvent>();
            lastEvent = null;
            lastTime = 0;
            internalTime = 0;
            paused = false;
        }

        /*
         * Add a single event to the queue, as soon as possible.
         */
        public void Add(float duration, AIEvent.Action action)
        {
            float start = 0;
            if (internalTime > lastTime)
            {
                start = internalTime;
            }
            else
            {
                start = lastTime;
                lastTime += duration;
            }

            events.Enqueue(new AIEvent(start, duration, action));
        }

        /*
         * Add a single event to the queue as soon as possible.
         */
        public void Add(AIEvent e)
        {
            Add(e.duration, e.action);
        }

        /*
         * Adds a list of events in a sequence.
         */
        public void Add(AISequence sequence)
        {
            if (sequence.events == null)
            {
                Debug.LogError("Sequence had null events!");
                return;
            }

            foreach (AIEvent e in sequence.events)
            {
                if (e == null)
                {
                    Debug.LogError("Found null event!");
                    continue;
                }
                Add(e.duration, e.action);
            }
        }

        /*
         * Adds a sequence with an additional delay afterwards.
         */
        public void Add(float delay, AISequence sequence)
        {
            Add(sequence);
            Add(delay, (AISequence)null);
        }

        /*
         * Adds a single action "times" times to the queue.
         */
        public void AddRepeat(float duration, AIEvent.Action action, int times)
        {
            for (int i = 0; i < times; i++)
            {
                Add(duration, action);
            }
        }

        /*
         * Adds a given sequence to the queue "times" number of times.
         */
        public void AddSequenceRepeat(int times, AISequence sequence)
        {
            for (int i = 0; i < times; i++)
            {
                Add(sequence);
            }
        }

        /*
         * Updates the queue to either execute the current action, or else
         * do nothing this frame.
         */
        public void Update()
        {
            if (paused)
            {
                return;
            }
            internalTime += Time.deltaTime;

            // if the player is too aggressive, you can ignore the q here

            if (events.Count == 0) return;
            AIEvent iEvent = events.Peek();
            //Debug.Log("Top event is " + (iEvent.ability == null ? "null" : iEvent.ability.name));

            if (internalTime >= iEvent.startTime)
            {
                // If the event is new, we fire it
                if (lastEvent != iEvent)
                {
                    if (iEvent.action != null)
                    {
                        iEvent.action();
                    }
                    lastEvent = iEvent;
                }

                // If the event is stale, remove it from the q
                if (internalTime >= iEvent.startTime + iEvent.duration)
                {
                    events.Dequeue();
                }
            }
        }

        public void Pause()
        {
            paused = true;
        }

        public void Unpause()
        {
            paused = false;
        }

        public bool Empty() {
            return this.events.Count == 0;
        }
    }
}