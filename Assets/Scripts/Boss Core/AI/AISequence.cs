using System.Collections;
using System.Collections.Generic;
using BossCore;
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

        // TODO remove this later by making all constructors that set it protected.
        public bool _isDirty = false;

        // TODO put these in a publically accessable location. Possibly in world or game manager.
        public static ProxyVector3 PLAYER_POSITION = new ProxyVector3(() => { return GameManager.Player.transform.position + World.Arena.CENTER; });

        /// <summary>
        /// Grabs the delayed player position. If the "PlayerLock" move is locked on, then
        /// this will return the player position at the time the move was run. Otherwise, this
        /// returns the same value as PLAYER_POSITION.
        /// <see cref="Moves.Basic.PlayerLock"/>
        /// </summary>
        public static ProxyVector3 DELAYED_PLAYER_POSITION = Moves.Basic.PlayerLock._delayed_player_position;

        // Experimental. Leads ahead of the player based on their current velocity and distance from boss.
        // This is quite realtime, but can be jittery as a result.
        public static ProxyVector3 LEADING_PLAYER_POSITION = new ProxyVector3(() => {
            float distance = (GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude;
            //Vector3 offset = (distance / 2f * GameManager.Player.GetComponent<Rigidbody>().velocity.normalized);
            Vector3 offset = 1f * GameManager.Player.GetComponent<Rigidbody>().velocity.normalized;

            return PLAYER_POSITION.GetValue() + offset;
        });

        // Smooths the value of LEADING_PLAYER_POSITION using two samples over time.
        // This is less "realtime", but provides a smoother tracking.
        private static Vector3 last_lead = Vector3.zero;
        private static Vector3 curr_lead = Vector3.zero;
        public static ProxyVector3 SMOOTHED_LEADING_PLAYER_POSITION = new ProxyVector3(() =>
        {
            Vector3 raw_value = LEADING_PLAYER_POSITION.GetValue();

            last_lead = curr_lead;
            curr_lead = raw_value;

            return (last_lead + curr_lead) / 2.0f;

        });

        public static ProxyVector3 BOSS_POSITION = new ProxyVector3(() => { return GameManager.Boss.transform.position; });
        public static ProxyVector3 RANDOM_IN_ARENA = new ProxyVector3(() =>
        {
            float angle = Random.value * 360;
            float distance = Random.Range(0, GameManager.Arena.RadiusInWorldUnits);
            return distance * (Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward) + World.Arena.CENTER;
        });

        // A list of events to execute.
        public AIEvent[] events;
        private AISequence[] children;

        public delegate AISequence[] AISequenceGenerator();
        public AISequenceGenerator GetChildren;

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
        public float Difficulty
        {
            get; protected set;
        }

        /*
         * What's the name of this Move?
         * 
         * The default value is the name of the class.
         */
        public string Name
        {
            get
            {
                return GetType().Name.Replace('_', ' ');
            }

            protected set
            {
                Name = value;
            }
        }

        /*
         * What does this Move do?
         * 
         * You should override this method with a more descriptive bit of text;
         * a warning is generated if this value isn't set.
         */
        public string Description
        {
            get; protected set;
        }

        public override string ToString() {
            if (Description != null) {
                return Description;
            }

            string fullDesc = null;
            AISequence[] sequences = GetChildren();
            if (sequences != null)
            {
                if (sequences.Length > 0) {
                    fullDesc += sequences[0];
                }

                for (int i = 1; i < sequences.Length; i++)
                {
                    fullDesc += " Then, " + sequences[i];
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
        // TODO make private; currently Moves.Basic.Pause() uses this
        [System.Obsolete]
        protected AISequence(AIEvent[] events) {
            _isDirty = true;
            this.events = events; 
            this.children = null;

            this.GetChildren = () => { return children; };
        }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         * TODO make protected
         */
        [System.Obsolete]
        public AISequence(AIEvent.Action a)
        {
            _isDirty = true;
            this.events = new AIEvent[] { new AIEvent(0f, a) };
            this.children = null;

            this.GetChildren = () => { return children; };
        }

        /*
         * Keeps track of a function that can "explode" into a list of AISequences.
         * When this is added to the event queue, this function is called.
         * TODO make protected
         */
        [System.Obsolete]
        public AISequence(GenerateSequences genFunction)
        {
            _isDirty = true;
            this.events = null;
            this.children = null;

            this.GetChildren = () => genFunction();
            this.Description = ShouldTryExpandFunctions ? null : "Some sequences were generated from a function.";
        }

        /*
         * Keeps track of a function that can "explode" into a single AISequence.
         * When this is added to the event queue, this function is called.
         * TODO make protected
         */
        [System.Obsolete]
        public AISequence(GenerateSequence genFunction)
        {
            _isDirty = true;
            this.events = null;
            this.children = null;

            this.GetChildren = () => new AISequence[] { genFunction() };
            this.Description = ShouldTryExpandFunctions ? null : "A sequence was generated from a function.";
        }

        /*
         * Takes an arbitrary length list of AISequences and combines them into an AISequence.
         */
        public AISequence(params AISequence[] sequences)
        {
            this.events = null;
            this.children = sequences;

            this.GetChildren = () => { return children; };
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
         * it is called, and so the exact list returned may vary between method calls.
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

            if (sequence.events == null) {
                Debug.LogError("Failed to flatten AISequence: \"" + sequence + "\". Children and Events are both null.");
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
            for (int i = 0; i < sequences.Length; i++)
            {
                indicies[i] = 0;
                startTimes[i] = 0;
                events[i] = referenceEvents[i][indicies[i]];
            }

            // Continuously go through the next events; merge any that start at the same time.
            // Then remove them from processing. The merged event duration is equal to the minimum
            // duration of all the events combined; this ensures all events can run as before.
            List<AIEvent> finalEventsList = new List<AIEvent>();
            while (true)
            {
                //AIEvent nextEvent = events[0];
                float nextStartTime = float.PositiveInfinity;
                for (int i = 0; i < events.Length; i++)
                {
                    if (indicies[i] >= referenceEvents[i].Length)
                    {
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

                if (eventsToMerge.Count == 0)
                {
                    break;
                }
                else
                {
                    finalEventsList.Add(BasicMerge(eventsToMerge.ToArray()));
                }
            }

            string mergedDesc = "Merged( ";
            for (int i = 0; i < sequences.Length - 1; i++)
            {
                mergedDesc += sequences[i] + " And ";
            }
            mergedDesc += sequences[sequences.Length - 1];
            mergedDesc += ").";

            return new AISequence(finalEventsList.ToArray()) { 
                Description = mergedDesc 
            };
        }

        public static AISequence Merge(List<AISequence> sequences)
        {
            return Merge(sequences.ToArray());
        }

        /*
         * Returns this AISequence repeated "times" number of times.
         * TODO make a ProxyInt
         */
        public AISequence Times(int times)
        {
            if (times <= 0)
            {
                Debug.LogError("Cannot repeat sequence 0 or fewer times");
                times = 1;
            }
            if (times == 1)
            {
                return this;
            }

            AISequence[] newSequences = new AISequence[times];
            for (int i = 0; i < times; i++)
            {
                newSequences[i] = this;
            }
            return new AISequence(newSequences)
            {
                Description = times + " times: "
            };
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
            return new AISequence(new AIEvent[] { new AIEvent(duration, () => { }) }) { 
                Description = "Wait for " + duration + " seconds." 
            };
        }

        /*
         * Returns this AISequence, followed by the events in "seq", in order.
         */
        public AISequence Then(AISequence seq)
        {
            return new AISequence(this, seq);
        }

        /*
         * Picks a random sequence from the provided list.
         */
        public static AISequence Either(params AISequence[] sequences)
        {
            return new AISequence(() =>
            {
                return sequences[(int)(Random.value * sequences.Length)];
            });
        }

        // A delegate that captures an iterator in a for loop
        public delegate AISequence ForBody(float iterator);

        public static AISequence For(float count, ForBody body)
        {
            if (count <= 0)
            {
                Debug.LogError("Found a for loop with negative count.");
                return body(0);
            }
            return For(0, count, 1, body);
        }

        public static AISequence For(float start, float end, ForBody body)
        {
            if (end < start)
            {
                Debug.LogError("Found a for loop with end before start.");
                return body(start);
            }
            return For(start, end, 1, body);
        }

        /*
         * Iterates over the given boundaries, and passes the step value to the ForBody
         * provided in the last parameter. Useful for replacing delegates with basic
         * for loops inside of them. The events returned by this function happen as
         * separate events; if the ForBody's AISequence has a delay, this will appear
         * between all the events produced.
         */
        public static AISequence For(float start, float end, float step, ForBody body)
        {
            Debug.Log("For called!");
            if (Mathf.Approximately(step, 0))
            {
                Debug.LogError("Found for loop with step size 0.");
                return body(start);
            }

            if (Mathf.Abs(Mathf.Sign(end - start) - Mathf.Sign(step)) > 0.01f)
            {
                Debug.LogError("Found for loop that will never terminate.");
                return body(start);
            }

            AISequence[] sequences = new AISequence[(int)Mathf.Abs((end - start) / step)];
            int count = 0;
            if (start > end)
            {
                for (float i = start; i > end; i += step)
                {
                    sequences[count++] = body(i);
                }
            }
            else
            {
                for (float i = start; i < end; i += step)
                {
                    sequences[count++] = body(i);
                }
            }
            return new AISequence(sequences);
        }

        public static AISequence ForConcurrent(float count, ForBody body)
        {
            return ForConcurrent(0, count, 1, body);
        }

        public static AISequence ForConcurrent(float start, float end, ForBody body)
        {
            return ForConcurrent(start, end, 1, body);
        }

        /*
         * Does the same as "For", but all the events generated happen in one frame.
         * Useful for generating sequences with multiple projectiles appearing at once.
         * 
         * This means a wait returned by ForBody will happen at the end, rather than
         * between each sequence.
         */
        public static AISequence ForConcurrent(float start, float end, float step, ForBody body)
        {
            return Merge(For(start, end, step, body).GetChildren());
        }




        #endregion
    }
}