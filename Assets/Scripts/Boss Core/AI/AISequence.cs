﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;

using System.Reflection;

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

        public int? payloadID = null;

        private static int payloadCount;
        private static Dictionary<int, object> payloads = new Dictionary<int, object>();

        public static int AddPayload()
        {
            Debug.Log("Adding payload with ID: " + payloadCount);
            return payloadCount++;
        }

        public static void SetPayload(int id, object payload)
        {
            payloads[id] = payload;
        }

        private AISequence SetPayloadID(int? to)
        {
            this.payloadID = to;
            return this;
        }

        #region Constructors

        // Used internally as a shortcut.
        private AISequence(AIEvent[] events) { this.events = events; }

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         */
        public AISequence(AIEvent.Action a) : this(-1, a) { }

        public AISequence(float difficulty, AIEvent.Action a)
        {
            this.difficulty = difficulty;
            this.events = new AIEvent[] { new AIEvent(0f, a) };
        }

        /*
         * Takes an arbitrary length list of AISequences and combines them into an AISequence.
         */
        public AISequence(params AISequence[] sequences) : this(-1, sequences) { }

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
        public AISequence(GenerateSequences genFunction) : this(-1, genFunction()) { }

        public AISequence(float difficulty, GenerateSequences genFunction) : this(difficulty, genFunction()) { }

        /*
         * Executes the generation function and instantiates this AISequence as the result.
         */
        public AISequence(GenerateSequence genFunction) : this(-1, genFunction()) { }

        public AISequence(float difficulty, GenerateSequence genFunction) : this(difficulty, genFunction()) { }

        #endregion

        #region Tools to build up AISequences
        /*
         * Merges the given array of AISequences, and executes all of them concurrently.
         * 
         * This is useful for chaining individual "Shoot1" methods together,
         * or stitching various "ShootArc" methods.
         * 
         * TODO: make this stitch more complex AISequences using their timings. do a zipper merge!
         * this will allow things like having concurrent "Sweep" attacks in opposite directions.
         */
        public static AISequence Merge(params AISequence[] sequences)
        {
            return new AISequence(() =>
            {
                for (int i = 0; i < sequences.Length; i++)
                {
                    for (int j = 0; j < sequences[i].events.Length; j++)
                    {
                        sequences[i].events[j].action();
                    }
                }
            });
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
            if (times == 0)
            {
                times = 1;
                Debug.LogError("Cannot repeat sequence 0 times");
            }
            AIEvent[] newEvents = new AIEvent[this.events.Length * times];
            for (int i = 0; i < times; i++)
            {
                for (int j = 0; j < this.events.Length; j++)
                {
                    newEvents[(i * this.events.Length) + j] = this.events[j];
                }
            }
            return new AISequence(newEvents).SetPayloadID(payloadID);
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

            if (payloadID != null)
            {
                Debug.Log("(wait) Payload: " + payloadID);
            }

            return new AISequence(newEvents).SetPayloadID(payloadID);

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
            AIEvent[] newEvents = new AIEvent[this.events.Length + seq.events.Length];
            for (int i = 0; i < this.events.Length; i++)
            {
                newEvents[i] = this.events[i];
            }
            for (int i = 0; i < seq.events.Length; i++)
            {
                newEvents[i + this.events.Length] = seq.events[i];
            }

            return new AISequence(newEvents).SetPayloadID(payloadID);
        }

        #endregion

        public AISequence SetSpeed(BossCore.Speed speed)
        {
            return Then(new AISequence(() =>
            {
                if (payloadID == null)
                {
                    return;
                }
                else
                {
                    object payload = payloads[payloadID ?? -1];
                    Debug.Log("Got payload: " + payload);
                    /*
                    if (payload is AOE || payload is Projectile)
                    {
                        try
                        {
                            FieldInfo info = payload.GetType().GetField("data");
                            Debug.Log(info);
                            System.Type type = info.FieldType;
                            Debug.Log(type);
                            MethodInfo method = type.GetMethod("Speed");
                            Debug.Log(method);
                            method.Invoke((AOE.AOEStructure)info.GetValue(payload), new object[] { speed });
                        }
                        catch (System.Exception e)
                        {
                            Debug.Log("hecced up" + e);
                        }
                    }
                    */

                    if (payload is AOE)
                    {
                        AOE aoe = payload as AOE;
                        aoe.data = aoe.data.Speed(speed);
                        aoe.data = aoe.data.InnerSpeed(speed);
                        Debug.Log(aoe.data.expansionSpeed);
                    }
                }
                /*
                if (
                {
                }
                else
                {
                    Debug.Log("Failed because type is: " + payload.GetType());
                    return;
                }
                */
            })).SetPayloadID(payloadID);

            /*
            if (payload is AOE)
            {
                Debug.Log("SetSpeed called with AOE");
                return Then(new AISequence(() =>
                {
                    AOE aoe = payload as AOE;
                    aoe.data = aoe.data.Speed(speed);
                    Debug.Log(aoe.data.expansionSpeed);
                }));
            }
            else if (payload is Projectile)
            {
                return Then(new AISequence(() =>
                {
                    Projectile projectile = payload as Projectile;
                }));
            }
            else
            {
                return this;
            }
            */
        }
    }
}