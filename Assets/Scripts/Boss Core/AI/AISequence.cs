﻿using System.Collections;
using System.Collections.Generic;
using BossCore;
using UnityEngine;

namespace AI
{
    public class AISequence
    {
        /// <summary>
        /// When calling ToString on AISequences, should we try to expand sequence generator
        /// functions into their base AISequences, or simply say there was a function there?
        /// </summary>
        public static bool ShouldTryExpandFunctions = false;

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
        public static ProxyVector3 LEADING_PLAYER_POSITION = new ProxyVector3(() =>
        {
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

        // A list of events to execute. This is the data, or leaf, of the recursive structure.
        public AIEvent[] Events
        {
            get; protected set;
        }

        // The AISequence children. This represents an intermediate node in the recursion.
        public delegate AISequence[] AISequenceGenerator();
        public AISequenceGenerator Children
        {
            get; protected set;
        }

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

        public override string ToString()
        {
            if (Description != null)
            {
                return Description;
            }

            string fullDesc = null;
            AISequence[] sequences = Children();
            if (sequences != null)
            {
                if (sequences.Length > 0)
                {
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
            else if (Events != null)
            {
                fullDesc += "Some events executed, and that's all we know.";
            }

            return fullDesc;
        }

        #region Constructors

        // Used internally within AISequence.
        private AISequence(AIEvent[] events)
        {
            this.Events = events;
            //this.Children = null;

            this.Children = () => { return null; };
        }

        /// <summary>
        /// Takes an arbitrary length list of AISequences and combines them into an AISequence.
        /// </summary>
        /// <param name="sequences">A variable length list of sequences.</param>
        public AISequence(params AISequence[] sequences)
        {
            this.Events = null;
            //this.Children = sequences;

            this.Children = () => { return sequences; };
        }

        #endregion

        #region Internal tools for AISequences

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
        private AIEvent[] Flatten()
        {
            return FlattenRecur(this);
        }

        private AIEvent[] FlattenRecur(AISequence sequence)
        {

            AISequence[] seqChildren = sequence.Children();
            if (seqChildren != null)
            {
                List<AIEvent> childrenEvents = new List<AIEvent>();
                for (int i = 0; i < seqChildren.Length; i++)
                {
                    childrenEvents.AddRange(FlattenRecur(seqChildren[i]));
                }
                return childrenEvents.ToArray();
            }

            if (sequence.Events == null)
            {
                Debug.LogError("Failed to flatten AISequence: \"" + sequence + "\". Children and Events are both null.");
            }
            return sequence.Events; // If null, will crash AddRange above
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
         * 
         * Note: This returns a new AISequence because "MergeCoerce" flattens the sequences passed in.
         * This fixes random values, and so breaks a lot of the sequences. Making the MergeCoerce call
         * take place in a delegate means it'll resample "sequences" every time.
         */
        public static AISequence Merge(params AISequence[] sequences)
        {
            return new AISequence
            {
                Children = () => new AISequence[] { MergeCoerce(sequences) }
            };
        }

        // Merges the given array of AISequences and executes all of them concurrently.
        private static AISequence MergeCoerce(params AISequence[] sequences)
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

            return new AISequence(finalEventsList.ToArray())
            {
                Description = mergedDesc
            };
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
            return new AISequence(new AIEvent[] { new AIEvent(duration, () => { }) })
            {
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
            return new AISequence
            {
                Events = null,
                Children = () => new AISequence[] { sequences[(int)(Random.value * sequences.Length)] },
                Description = "A sequence was chosen randomly from a list."
            };
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
            return Merge(For(start, end, step, body).Children());
        }

        public static AISequence If(bool condition, AISequence then)
        {
            return condition ? then : Pause(0f);
        }

        public static AISequence If(bool condition, AISequence then, AISequence _else)
        {
            return condition ? then : _else;
        }

        // TODO this isn't pretty but it works.
        // I tried using ProxyVariables but then everything needs to use them. Is there a better way
        // to get randomness in the moves?
        public static AISequence GenerateRandom(ForBody body)
        {
            return new AISequence()
            {
                Children = () => new AISequence[] { body(Random.value) }
            };
        }

        #endregion

        /// <summary>
        /// Guards against invalid AISequences. Prints via Debug.Log/Error if there are any
        /// problems with the AISequence. If this returns true, it may still have warnings; if
        /// this returns false, it will print errors.
        /// </summary>
        public static bool IsValid(AISequence sequence) 
        {
            if (sequence == null)
            {
                Debug.LogError("Null AISequence added to queue.");
                return false;
            }

            // Guard against basic attempts to break the game. A user cannot define InternalMoves.
            if (!(sequence is Move))
            {
                if (sequence is InternalMove)
                {
                    if (!InternalMove.IsValid(sequence as InternalMove))
                    {
                        Debug.LogError("Found InternalMove that was not recognized. Refusing to execute. Name: " + sequence.Name);
                        return false;
                    } // else ok
                }
            }

            // "glue" AISequences are special: AISequences followed by "Then" or "Wait" 
            // won't have descriptions, but can be identified by being direct instances
            // of the "AISequence" class (vs. subclasses for every other move).
            //
            // These guys don't need to have a valid difficulty or description.
            //bool isGlueSequence = sequence.Name.Equals("AISequence");
            bool isGlueSequence = sequence.GetType() == typeof(AISequence) || sequence is InternalMove;

            // Warn about unnamed sequences. By default, this shouldn't be called; the standard name is valid.
            if (sequence.Name == null)
            {
                Debug.LogWarning("Found AISequence without a name. Description: " + sequence.Description ?? "not provided.");
            }

            // Warn if there's a named sequence without a description.
            //
            if (sequence.Description == null && !isGlueSequence)
            {
                Debug.LogWarning("Found AISequence with a name, but without a description. Name: " + sequence.Name);
            }

            // Warn about default descriptions.
            if (sequence.Description != null && sequence.Description.Equals("Your description here"))
            {
                Debug.LogWarning("Found AISequence with default description. Name: " + sequence.Name);
            }

            // Warn if there's a sequence with too high a difficulty.
            if (sequence.Difficulty >= 8f)
            {
                Debug.LogWarning("Found AISequence with very high difficulty (" + sequence.Difficulty + "). Name: " + sequence.Name);
            }

            // Warn about default difficulty (-1). Glue sequences can ignore this.
            if (Mathf.Abs(sequence.Difficulty - -1) < 0.01f && !isGlueSequence)
            {
                Debug.LogWarning("Found AISequence with default difficulty (-1). Name: " + sequence.Name);
            }

            // Warn about invalid difficulty (<= 0). Glue sequences can ignore this.
            if (sequence.Difficulty <= 0f && !isGlueSequence)
            {
                Debug.LogWarning("Found AISequence with invalid difficulty (<= 0). Name: " + sequence.Name);
            }

            return true;
        }
    }
}