using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace AI
{
    public class EventQueue
    {

        private readonly Queue<AIEvent> events;
        private AIEvent lastEvent;
        private float lastTime;

        private float internalTime;
        private bool paused;

        public EventQueue()
        {
            events = new Queue<AIEvent>();
            lastEvent = null;
            lastTime = 0;
            internalTime = 0;
            paused = false;
        }

        /*
         * Add a single event to the queue as soon as possible.
         */
        public void Add(AIEvent e)
        {
            //Add(e.duration, e.action);
            if (BossController.insaneMode)
            {
                e.duration = e.duration / 1.5f;
            }

            // Schedule the event immediately if possible (internalTime), or if
            // there are other events on the queue already (lastTime > internalTime),
            // schedule the event as soon as possible.
            float start = Mathf.Max(internalTime, lastTime);
            lastTime = start + e.duration;

            // Set the start of the event and add it
            e.startTime = start;
            Debug.Log("Setting start time to: " + start);
            events.Enqueue(e);
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
                Add(e);
            }
        }

        /*
         * Adds a sequence with an additional delay afterwards.
         */
        /*
        public void Add(float delay, AISequence sequence)
        {
            Add(sequence);
            Add(delay, (AISequence)null);
        }
        */

        /*
         * Adds a single action "times" times to the queue.
         */
        public void AddRepeat(float duration, AIEvent.Action action, int times)
        {
            for (int i = 0; i < times; i++)
            {
                Add(new AIEvent(duration, action));
            }
        }

        /*
         * Adds a given sequence to the queue "times" number of times.
         */
        public void AddRepeat(int times, AISequence sequence)
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