using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;

public class AISequence
{
    public AIEvent[] events;

    private static Dictionary<string, AISequence> sequenceDictionary = new Dictionary<string, AISequence>();

    public AISequence(params AIEvent[] events)
    {
        this.events = events;
    }

    public static void AddSequence(string name, AISequence sequence) {
        sequenceDictionary.Add(name, sequence);
    }

    public static AISequence GetSequence(string name) {
        return sequenceDictionary[name];
    }
}
