using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // A list of events to execute.
        public AIEvent[] events;
        private AISequence[] children;

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

        private AISequence reference;

        public delegate AISequence[] AISequenceGenerator();
        public AISequenceGenerator GetChildren;

        #region Constructors

        // Used internally as a shortcut.
        private AISequence(AIEvent[] events) { 
            this.events = events; 
            this.children = null;
            this.difficulty = -1;

            this.GetChildren = () => { return children; };
        }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         */
        public AISequence(AIEvent.Action a) : this(-1, a) { }

        public AISequence(float difficulty, AIEvent.Action a)
        {
            this.difficulty = difficulty;
            this.events = new AIEvent[] { new AIEvent(0f, a) };
            this.children = null;

            this.GetChildren = () => { return children; };
        }

        /*
         * Takes an arbitrary length list of AISequences and combines them into an AISequence.
         */
        public AISequence(params AISequence[] sequences) : this(-1, sequences) { }

        public AISequence(float difficulty, params AISequence[] sequences)
        {
            this.difficulty = difficulty;
            this.events = null;
            this.children = sequences;

            this.GetChildren = () => { return children; };
        }

        /*
         * "Explodes" the generation function and adds all the elements to a single AISequence.
         * TODO: this is in a delicate state- the "events" should be accessed using the new GetEvents
         * delegate. Anything not using that will crash!
         */
        public AISequence(GenerateSequences genFunction) : this(-1, genFunction) { }

        public AISequence(float difficulty, GenerateSequences genFunction) {
            this.difficulty = difficulty;
            this.events = null;
            this.children = null;

            this.GetChildren = () => genFunction();
        }

        /*
         * Executes the generation function and instantiates this AISequence as the result.
         */
        public AISequence(GenerateSequence genFunction) : this(-1, genFunction) { }

        public AISequence(float difficulty, GenerateSequence genFunction)
        {
            this.difficulty = difficulty;
            this.events = null;
            this.children = null;

            this.GetChildren = () => new AISequence[] { genFunction() };
        }

        #endregion

        #region Somewhat internal tools for AISequences

        private static AIEvent BasicMerge(params AIEvent[] events)
        {
            float minDuration = float.PositiveInfinity;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i].duration < minDuration)
                {
                    minDuration = events[i].duration;
                }
            }

            return new AIEvent(minDuration, () =>
            {
                for (int i = 0; i < events.Length; i++)
                {
                    events[i].action();
                }
            });
        }

        /*
         * Returns the provided sequence array, but with every element merged in order
         * respecting wait times. Used to collapse a list of exploded sequences from
         * a Generator function into a single sequence.
         */
        private static AISequence SequentialMerge(AISequence[] sequences)
        {
            if (sequences.Length == 0)
            {
                return new AISequence(new AIEvent[0]);
            }
            if (sequences.Length == 1)
            {
                return sequences[0];
            }

            AISequence sequential = sequences[0];
            for (int i = 1; i < sequences.Length; i++)
            {
                sequential = sequential.Then(sequences[i]);
            }
            return sequential;
        }

        /*
         * Returns this AISequence, but flattened from a tree structure to a simple
         * array of AIEvents. This will execute any generation functions every time 
         * it is called, and so the exact AIEvents generated may vary every time
         * this method is called.
         */
        public AIEvent[] Flatten() {
            return FlattenRecur(this);
        }

        private AIEvent[] FlattenRecur(AISequence sequence) {

            AISequence[] seqChildren = sequence.GetChildren();
            if (seqChildren != null) {
                List<AIEvent> childrenEvents = new List<AIEvent>();
                for (int i = 0; i < seqChildren.Length; i++) {
                    childrenEvents.AddRange(FlattenRecur(seqChildren[i]));
                }
                return childrenEvents.ToArray();
            }
            return sequence.events; // If null, will crash AddRange above
        }

        #endregion

        #region Tools to construct AISequences

        /*
         * Merges the given array of AISequences, and executes all of them concurrently.
         * 
         * At a basic level, this is useful for chaining individual "Shoot1" methods together,
         * or stitching various "ShootArc" methods.
         * 
         * On a more complex scale, two separate sets of tasks can be executed in parallel.
         */
        public static AISequence Merge(params AISequence[] sequences)
        {
            int[] indicies = new int[sequences.Length];
            float[] startTimes = new float[sequences.Length];

            // Force the sequence into a list so we can more easily merge it.
            AIEvent[][] referenceEvents = new AIEvent[sequences.Length][];
            for (int i = 0; i < sequences.Length; i++)
            {
                referenceEvents[i] = sequences[i].Flatten();
            }

            AIEvent[] events = new AIEvent[sequences.Length];
            for (int i = 0; i < sequences.Length; i++) {
                indicies[i] = 0;
                startTimes[i] = 0;
                events[i] = referenceEvents[i][indicies[i]];
            }

            // Continuously go through the next events; merge any that start at the same time.
            // Then remove them from processing. The merged event duration is equal to the minimum
            // duration of all the events combined; this ensures all events can run as before.
            List<AIEvent> finalEventsList = new List<AIEvent>();
            while(true) {
                //AIEvent nextEvent = events[0];
                float nextStartTime = float.PositiveInfinity;
                for (int i = 0; i < events.Length; i++) {
                    if (indicies[i] >= referenceEvents[i].Length) {
                        continue;
                    }

                    if (startTimes[i] < nextStartTime)
                    {
                        nextStartTime = startTimes[i];
                    }
                }

                List<AIEvent> eventsToMerge = new List<AIEvent>();
                for (int i = 0; i < sequences.Length; i++)
                {
                    if (indicies[i] >= referenceEvents[i].Length)
                    {
                        continue;
                    }

                    if (Mathf.Approximately(startTimes[i], nextStartTime))
                    {
                        // Add duration of this event to the start time
                        startTimes[i] += referenceEvents[i][indicies[i]].duration;

                        eventsToMerge.Add(referenceEvents[i][indicies[i]]);
                        indicies[i]++;
                    }
                }

                if (eventsToMerge.Count == 0) {
                    break;
                } else {
                    finalEventsList.Add(BasicMerge(eventsToMerge.ToArray()));
                }
            }

            return new AISequence(finalEventsList.ToArray());
        }

        public static AISequence Merge(List<AISequence> sequences)
        {
            return Merge(sequences.ToArray());
        }

        /*
         * Returns this AISequence repeated "times" number of times.
         */
        public AISequence Times(int times)
        {
            if (times <= 0)
            {
                Debug.LogError("Cannot repeat sequence 0 or fewer times");
                times = 1;
            }
            if (times == 1) {
                return this;
            }

            AISequence[] newSequences = new AISequence[times];
            for (int i = 0; i < times; i++) {
                newSequences[i] = this;
            }
            return new AISequence(newSequences);
        }

        /*
         * Returns this AISequence with an additional delay of length
         * "duration" seconds afterwards.
         */
        public AISequence Wait(float duration)
        {
            return new AISequence(this, Pause(duration));
        }

        /*
         * Returns a new AISequence that just consists of waiting for the
         * duration.
         */
        public static AISequence Pause(float duration)
        {
            return new AISequence(new AIEvent[] { new AIEvent(duration, () => { }) });
        }

        /*
         * Returns this AISequence, followed by the events in "seq", in order.
         */
        public AISequence Then(AISequence seq)
        {
            return new AISequence(this, seq);
        }

        public static AISequence Either(params AISequence[] sequences)
        {
            return new AISequence(() =>
            {
                return sequences[(int)(Random.value * sequences.Length)];
            });
        }

        #endregion
    }
}