using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class AIEvent {
    public float startTime;
    public float duration;
    public Ability ability;
    public object[] parameters;

    public AIEvent(float start, float duration, Ability ability, params object[] parameters)
    {
        this.startTime = start;
        this.duration = duration;
        this.ability = ability;
        this.parameters = parameters;
    }

    // For use when making AISequences. The start time is set when adding.
    public AIEvent(float duration, Ability ability, params object[] parameters) {
        this.startTime = -1;
        this.duration = duration;
        this.ability = ability;
        this.parameters = parameters;
    }
}