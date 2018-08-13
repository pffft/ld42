using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class EventQueue
{

    private Entity entity;
    private Queue<AIEvent> events;
    private AIEvent lastEvent;
    private float lastTime;

    public EventQueue(Entity reference)
    {
        this.entity = reference;

        events = new Queue<AIEvent>();
        lastEvent = null;
        lastTime = 0;
    }

    /*
     * Add a single event to the queue, as soon as possible.
     */
    public void Add(float duration, Ability ability, params object[] pars)
    {
        float start = 0;
        if (Time.time > lastTime)
        {
            start = Time.time;
        }
        else
        {
            start = lastTime;
            lastTime += duration;
        }

        events.Enqueue(new AIEvent(start, duration, ability, pars));
    }

    /*
     * Adds a list of events in a sequence.
     */
    public void AddSequence(AISequence sequence)
    {
        foreach (AIEvent e in sequence.events)
        {
            Add(e.duration, e.ability, e.parameters);
        }
    }

    /*
     * Adds a sequence that is in the Sequence dictionary.
     */
    public void AddSequence(string sequenceName) {
        AddSequence(AISequence.GetSequence(sequenceName));
    }

    /*
     * Adds a single action "times" times to the queue.
     */
    public void AddRepeat(int times, float duration, Ability ability, params object[] pars)
    {
        for (int i = 0; i < times; i++)
        {
            Add(duration, ability, pars);
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

        // if the player is too aggressive, you can ignore the q here

        if (events.Count == 0) return;
        AIEvent iEvent = events.Peek();
        //Debug.Log("Top event is " + (iEvent.ability == null ? "null" : iEvent.ability.name));

        if (Time.time >= iEvent.startTime)
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
                if (iEvent.ability != null)
                {
                    iEvent.ability.Use(entity, Vector3.zero, iEvent.parameters);
                }
                lastEvent = iEvent;
            }

            // If the event is stale, remove it from the q
            if (Time.time >= iEvent.startTime + iEvent.duration)
            {
                events.Dequeue();
            }
        }
    }
}