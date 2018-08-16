using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make a list of weight/AISequence pairs that can be constructed
// make a list of scripted events that happen once in X times
// randomly choose the next sequence to do, or check if player interrupts
public class AIPhase {

    private List<AISequence> phaseSequences;

    public AIPhase()
    {
        phaseSequences = new List<AISequence>();
    }
}
