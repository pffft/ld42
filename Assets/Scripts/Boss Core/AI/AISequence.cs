using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;

namespace AI
{
    /*
     * Some delegates used to generate a sequence of events that need more dynamic
     * information. This allows for for loops over sequences and events.
     */
    public delegate AISequence[] GenerateSequences();
    public delegate AISequence GenerateSequence();

    public partial class AISequence
    {
        public AIEvent[] events;

        /*
         * A relative difficulty parameter. 
         * 
         * This is from a scale of 0 - 10, where a "10" is the point where any person
         * would call the move "actual bullshit". That means that the move may guarantee
         * damage, might not have safespots, might be too fast, or all of the above.
         * 
         * Most moves that make it to the game should be at most an 8.
         * 
         * This can go above 10, but that's for testing purposes (or masochism).
         */
        public float difficulty;

        public AISequence() { }

        private AISequence(AIEvent[] events)
        {
            this.events = events;
        }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         */
        public AISequence(AIEvent.Action a)
        {
            this.events = new AIEvent[] { new AIEvent(0f, a) };
        }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This will have an additional delay afterwards.
         */
        public AISequence(float duration, AIEvent.Action action)
        {
            this.events = new AIEvent[] { new AIEvent(duration, action) };
        }
       
        public AISequence(params AISequence[] sequences) : this(-1, sequences) { }

        /*
         * Takes an arbitrary length list of AISequences and combines them into an AISequence.
         */
        public AISequence(float difficulty, params AISequence[] sequences)
        {
            this.difficulty = difficulty;
            List<AIEvent> eventsList = new List<AIEvent>();
            foreach (AISequence sequence in sequences)
            {
                eventsList.AddRange(sequence.events);
            }
            this.events = eventsList.ToArray();
        }

        /*
         * "Explodes" the generation function and adds all the elements to a single AISequence.
         */
        public AISequence(float difficulty, GenerateSequences genFunction) : this(difficulty, genFunction()) {}

        /*
         * Executes the generation function and instantiates this AISequence as the result.
         */
        public AISequence(float difficulty, GenerateSequence genFunction) : this(difficulty, genFunction()) {}

        /*
         * Merges the given array of AISequences, and executes all of them concurrently.
         * This is useful for chaining individual "Shoot1" methods together.
         */
        public static AISequence Merge(params AISequence[] sequences) {
            return new AISequence(0f, () => {
                for (int i = 0; i < sequences.Length; i++) {
                    for (int j = 0; j < sequences[i].events.Length; j++) {
                        sequences[i].events[j].action();
                    }
                }
            });
        }

        public static AISequence Merge(List<AISequence> sequences) {
            return Merge(sequences.ToArray());
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
         * Returns a new AISequence that just consists of waiting for the
         * duration.
         */
        public static AISequence Pause(float duration) {
            return new AISequence(new AIEvent[] { new AIEvent(duration, () => { }) });
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