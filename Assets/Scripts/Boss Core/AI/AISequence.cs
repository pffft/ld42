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

        public AISequence(params AIEvent[] events)
        {
            this.events = events;
        }

        public AISequence(params object[] events)
        {
            List<AIEvent> eventsList = new List<AIEvent>();
            foreach (object o in events)
            {
                if (o is AIEvent)
                {
                    eventsList.Add((AIEvent)o);
                }
                else if (o is AISequence)
                {
                    eventsList.AddRange(((AISequence)o).events);
                }
            }
        }

        public static AISequence Repeat(AIEvent iEvent, int times)
        {
            AIEvent[] events = new AIEvent[times];
            for (int i = 0; i < times; i++)
            {
                events[i] = iEvent;
            }
            AISequence sequence = new AISequence();
            sequence.events = events;
            return sequence;
        }

        public static AISequence Repeat(AISequence seq, int times)
        {
            AIEvent[] events = new AIEvent[seq.events.Length * times];
            for (int i = 0; i < seq.events.Length; i++)
            {
                for (int j = 0; j < times; j++)
                {
                    events[(i * times) + j] = seq.events[i];
                }
            }
            AISequence sequence = new AISequence();
            sequence.events = events;
            return sequence;
        }

        public static void AddSequence(string name, AISequence sequence)
        {
            sequenceDictionary.Add(name, sequence);
        }

        public static AISequence GetSequence(string name)
        {
            return sequenceDictionary[name];
        }
    }
}