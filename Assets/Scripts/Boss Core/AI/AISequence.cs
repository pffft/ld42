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
        /// <summary>
        /// When calling ToString on AISequences, should we try to expand sequence generator
        /// functions into their base AISequences, or simply say there was a function there?
        /// </summary>
        public static bool ShouldTryExpandFunctions = false;

        /// <summary>
        /// Should we allow new AISequences to be generated? This is false by default; when
        /// we are ready to load in the sequence dictionaries, we set this to true. This will
        /// catch floating AISequence declarations at runtime.
        /// </summary>
        public static bool ShouldAllowInstantiation = false;

        [System.Diagnostics.DebuggerHidden]
        private static void CheckAllowInstantiation()
        {
            if (!ShouldAllowInstantiation)
            {
                throw new System.Exception("Free-floating AISequence generated outside of Load().");
            }
        }

        // A list of events to execute.
        public AIEvent[] events;
        private AISequence[] children;

        public delegate AISequence[] AISequenceGenerator();
        public AISequenceGenerator GetChildren;

        // A description string. 
        public string description;

        public AISequence Description(string desc) {
            this.description = desc;
            return this;
        }

        public override string ToString() {
            if (description != null) {
                return description;
            }

            string fullDesc = null;
            AISequence[] sequences = GetChildren();
            if (sequences != null)
            {

                if (description == null)
                {
                    fullDesc += sequences[0];
                }
                else
                {
                    fullDesc += " Then, " + sequences[0];
                }

                if (sequences.Length > 1)
                {
                    for (int i = 1; i < sequences.Length; i++)
                    {
                        fullDesc += " Then, " + sequences[i];
                    }
                }

                // TODO: find a way to detect repeated function calls (with different parameters), and
                // combine them. E.g., Shoot1(... AngleOffset: -30) ... Shoot1(... AngleOffset: 90) should
                // collapse into something easier to read. For now, ShouldTryExpandFunctions is false so
                // we don't have a million sequences printing out.
            }
            else if (events != null)
            {
                fullDesc += "Some events executed, and that's all we know.";
            }

            return fullDesc;
        }

        #region Constructors

        // Used internally as a shortcut.
        private AISequence(AIEvent[] events) {
            CheckAllowInstantiation();
            this.events = events; 
            this.children = null;

            this.GetChildren = () => { return children; };
        }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         */
        public AISequence(AIEvent.Action a)
        {
            CheckAllowInstantiation();
            this.events = new AIEvent[] { new AIEvent(0f, a) };
            this.children = null;

            this.GetChildren = () => { return children; };
        }

        /*
         * Takes an arbitrary length list of AISequences and combines them into an AISequence.
         */
        public AISequence(params AISequence[] sequences)
        {
            CheckAllowInstantiation();
            this.events = null;
            this.children = sequences;

            this.GetChildren = () => { return children; };
        }

        /*
         * Keeps track of a function that can "explode" into a list of AISequences.
         * When this is added to the event queue, this function is called.
         */
        public AISequence(GenerateSequences genFunction)
        {
            CheckAllowInstantiation();
            this.events = null;
            this.children = null;

            this.GetChildren = () => genFunction();
            this.description = ShouldTryExpandFunctions ? null : "Some sequences were generated from a function.";
        }

        /*
         * Keeps track of a function that can "explode" into a single AISequence.
         * When this is added to the event queue, this function is called.
         */
        public AISequence(GenerateSequence genFunction)
        {
            CheckAllowInstantiation();
            this.events = null;
            this.children = null;

            this.GetChildren = () => new AISequence[] { genFunction() };
            this.description = ShouldTryExpandFunctions ? null : "A sequence was generated from a function.";
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

            string mergedDesc = "Merged( ";
            for (int i = 0; i < sequences.Length - 1; i++) {
                mergedDesc += sequences[i] + " And ";
            }
            mergedDesc += sequences[sequences.Length - 1];
            mergedDesc += ").";

            return new AISequence(finalEventsList.ToArray()).Description(mergedDesc);
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
            return new AISequence(newSequences).Description(times + " times: ");
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
            return new AISequence(new AIEvent[] { new AIEvent(duration, () => { }) }).Description("Wait for " + duration + " seconds.");
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