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
        public void Add(float duration, AIEvent.Action action, params object[] pars)
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

            events.Enqueue(new AIEvent(start, duration, action, pars));
        }

        public void Add(AIEvent e)
        {
            Add(e.duration, e.action, e.parameters);
        }

        /*
         * Adds a list of events in a sequence.
         */
        public void AddSequence(AISequence sequence)
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
                Add(e.duration, e.action, e.parameters);
            }
        }

        /*
         * Adds a sequence with an additional delay afterwards.
         */
        public void AddSequence(float delay, AISequence sequence)
        {
            AddSequence(sequence);
            Add(delay, null);
        }

        /*
         * Adds a sequence that is in the Sequence dictionary.
         */
        public void AddSequence(string sequenceName)
        {
            AddSequence(AISequence.GetSequence(sequenceName));
        }

        /*
         * Adds a sequence with an additional delay afterwards.
         */
        public void AddSequence(float delay, string sequenceName)
        {
            AddSequence(AISequence.GetSequence(sequenceName));
            Add(delay, null);
        }

        /*
         * Adds a single action "times" times to the queue.
         */
        public void AddRepeat(float duration, AIEvent.Action action, int times, params object[] pars)
        {
            for (int i = 0; i < times; i++)
            {
                Add(duration, action, pars);
            }
        }

        /*
         * Adds a given sequence to the queue "times" number of times.
         */
        public void AddSequenceRepeat(int times, AISequence sequence)
        {
            for (int i = 0; i < times; i++)
            {
                AddSequence(sequence);
            }
        }


        /*
         * Adds a sequence that is in the Sequence dictionary, "times" times.
         */
        public void AddSequenceRepeat(int times, string sequenceName)
        {
            AddSequenceRepeat(times, AISequence.GetSequence(sequenceName));
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
                    if (iEvent.parameters != null)
                    {
                        foreach (object o in iEvent.parameters)
                        {
                            // Debug.Log("Parameter: " + o);
                        }
                    }
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
    }
}