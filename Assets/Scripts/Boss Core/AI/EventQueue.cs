﻿using System.Collections;
using System.Collections.Generic;
using CombatCore;
using Moves;
using UnityEngine;
using UnityEngine.Profiling;

namespace AI
{
    public class EventQueue : MonoBehaviour
    {
        private Queue<AISequence> queuedSequences;
        private Queue<AIEvent> events;
        private AIEvent lastEvent;
        private float lastTime;

        private float internalTime;
        private bool paused;

        private bool running;

        /*
        public EventQueue()
        {
            events = new Queue<AIEvent>();
            lastEvent = null;
            lastTime = 0;
            internalTime = 0;
            paused = false;
        }
        */

        public void Awake()
        {
            queuedSequences = new Queue<AISequence>();
            events = new Queue<AIEvent>();
            lastEvent = null;
            lastTime = 0;
            internalTime = 0;
            paused = false;
            running = true;

            StartCoroutine(Execute());
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
            events.Enqueue(new AIEvent(e.startTime, e.duration, e.action));
        }

        public void Add(AISequence sequence) {

            if (sequence == null)
            {
                Debug.LogError("Null AISequence added to queue.");
                return;
            }

            // Generate warning if there's a named sequence without a description.
            //
            // Note that "glue" AISequences are allowed; those that don't subclass AISequence 
            // don't need to provide a description. This includes subclassed AISequences that
            // have additional Wait()s or Then()s called.
            if (sequence.Description == null && !sequence.Name.Equals("AISequence")) {
                //Debug.LogWarning("Found AISequence with a name, but without a description. Name: " + sequence.Name);
            }

            // Generate warning if there's a sequence with too high a difficulty.
            if (sequence.Difficulty >= 8f) {
                Debug.LogWarning("Found AISequence \"" + sequence.Name + "\" with very high difficulty: " + sequence.Difficulty + ". ");
            }

            //Debug.Log("Added AISequence" + 
                      //(sequence.Name.Equals("AISequence") ? " " : " \"" + sequence.Name + "\" ") + 
                      //"to queue. Here's what it says it'll do: \"" +
                      //(sequence.Description ?? sequence.ToString()) + "\".");

            /*
             * TODO: be as lazy as possible when evaluating AISequences for their events.
             * 
             * If we flatten in one update, then later AISequences may not have the latest
             * information. I.e., a generation function that creates 10 sequences, which each
             * wait for 0.05 seconds. Flattening immediate causes the last one to be (10 * 0.05)
             * seconds out of date.
             */
            /*
            AIEvent[] coercedEvents = sequence.Flatten();
            for (int i = 0; i < coercedEvents.Length; i++)
            {
                Add(coercedEvents[i]);
            }
            */
            queuedSequences.Enqueue(sequence);
            //StartCoroutine(Execute(sequence));
        }

        private IEnumerator Execute() {
            while (running)
            {
                //Profiler.BeginSample("Event Queue");
                while (queuedSequences.Count == 0)
                {
                    yield return new WaitForSeconds(0.05f);
                }

                AISequence nextSequence = queuedSequences.Dequeue();
                yield return WalkNext(nextSequence);
                //Profiler.EndSample();
            }
        }

        private IEnumerator WalkNext(AISequence sequence) {
            //Debug.Log("Walking over " + sequence.Name);
            if (sequence.events != null)
            {
                for (int i = 0; i < sequence.events.Length; i++) {
                    Add(sequence.events[i]);
                    yield return new WaitForSeconds(sequence.events[i].duration);
                }
            }
            else
            {
                AISequence[] children = sequence.GetChildren();
                for (int i = 0; i < children.Length; i++) 
                {
                    //Debug.Log("Diving into child node: " + seq.Name);
                    yield return WalkNext(children[i]);
                    //Debug.Log("Finished diving into child node: " + seq.Name);
                    //WalkNext(seq);
                }
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

            // TODO clean up this block
            int eventsExecuted = 0;
            while (events.Count > 0 && eventsExecuted < 50)
            {
                AIEvent iEvent = events.Peek();

                if (internalTime >= iEvent.startTime)
                {
                    // If the event is new, we fire it
                    if (lastEvent != iEvent)
                    {
                        iEvent.action?.Invoke();
                        lastEvent = iEvent;
                    }

                    // If the event is stale, remove it from the q
                    if (internalTime >= iEvent.startTime + iEvent.duration)
                    {
                        events.Dequeue();
                        eventsExecuted++;
                        if (events.Count == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
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
            return this.queuedSequences.Count == 0;
            //return isEmpty;
        }
    }
}