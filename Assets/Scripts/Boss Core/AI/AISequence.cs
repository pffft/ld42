using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;

namespace AI
{
    public partial class AISequence
    {
        public AIEvent[] events;

        private static Dictionary<string, AISequence> sequenceDictionary = new Dictionary<string, AISequence>();

        public AISequence() { }

        /*
         * Takes an arbitrary length list of AIEvents and combines them into an AISequence.
         */
        public AISequence(AIEvent[] events)
        {
            this.events = events;
        }

        /*
         * Takes an arbitrary length list of AIEvents and AISequences, and makes one
         * large list containing all elements.
         */
        public AISequence(params object[] objects)
        {
            Debug.Log("AISequence constructor called. Is events null?: " + (objects == null));

            List<AIEvent> eventsList = new List<AIEvent>();
            foreach (object o in objects)
            {
                Debug.Log(o.GetType());
                if (o is AIEvent)
                {
                    eventsList.Add((AIEvent)o);
                }
                else if (o is AISequence)
                {
                    if (((AISequence)o).events == null)
                    {
                        Debug.Log("Found AISequence with null events list when creating AISequence.");
                        continue;
                    }
                    eventsList.AddRange(((AISequence)o).events);
                }
            }

            events = eventsList.ToArray();
        }

        /*
         * Remembers the provided AISequence by the given name.
         */
        public static void AddSequence(string name, AISequence sequence)
        {
            sequenceDictionary.Add(name, sequence);
        }

        /*
         * Gets a previously saved AISequence by the given name.
         */
        public static AISequence GetSequence(string name)
        {
            return sequenceDictionary[name];
        }

        public static AISequence Repeat(AIEvent iEvent, int times)
        {
            if (times == 0)
            {
                times = 1;
                Debug.LogError("Cannot repeat event 0 times");
            }

            AIEvent[] newEvents = new AIEvent[times];
            for (int i = 0; i < times; i++)
            {
                newEvents[i] = iEvent;
            }
            AISequence sequence = new AISequence(newEvents);
            return sequence;
        }

        public static AISequence Repeat(AISequence seq, int times)
        {
            if (times == 0)
            {
                times = 1;
                Debug.LogError("Cannot repeat sequence 0 times");
            }
            AIEvent[] newEvents = new AIEvent[seq.events.Length * times];
            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < seq.events.Length; j++)
                {
                    newEvents[(i * seq.events.Length) + j] = seq.events[j];
                }
            }
            return new AISequence(newEvents);
        }

        /*
         * Returns this AISequence repeated "times" number of times.
         */
        public AISequence Times(int times)
        {
            return Repeat(this, times);
        }

        /*
         * Returns this AISequence with an additional delay of length
         * "duration" seconds afterwards.
         */
        public AISequence Wait(float duration)
        {
            AIEvent[] newEvents = new AIEvent[this.events.Length + 1];
            for (int i = 0; i < this.events.Length; i++)
            {
                newEvents[i] = this.events[i];
            }
            newEvents[this.events.Length] = new AIEvent(duration, () => { });
            return new AISequence(newEvents);

        }

        /*
         * Returns this AISequence with an additional AIEvent added to the end.
         * This copies the entire AISequence over. If you want to add multiple events
         * this way, it's better to register an AISequence using "AddSequence".
         */
        public AISequence Then(AIEvent ev)
        {
            AIEvent[] newEvents = new AIEvent[this.events.Length + 1];
            for (int i = 0; i < this.events.Length; i++)
            {
                newEvents[i] = this.events[i];
            }
            newEvents[this.events.Length] = ev;
            return new AISequence(newEvents);
        }

        /*
         * Returns this AISequence, followed by the events in "seq", in order.
         */
        public AISequence Then(AISequence seq)
        {
            AIEvent[] newEvents = new AIEvent[this.events.Length + seq.events.Length];
            for (int i = 0; i < this.events.Length; i++)
            {
                newEvents[i] = this.events[i];
            }
            for (int i = 0; i < seq.events.Length; i++)
            {
                newEvents[i + this.events.Length] = seq.events[i];
            }

            return new AISequence(newEvents);
        }
            
    }
}