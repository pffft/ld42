using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

public class Move : AISequence {

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

    /*
     * What's the name of this Move?
     * This is normally the same as the static name in the dictionary file.
     */
    public string name;

    /*
     * What does this Move do?
     * This is usually whatever was commented above the AISequence definition.
     */
    public string description;

    /*
     * The actual AISequence we're talking about.
     */
    public AISequence sequence;

    public Move(float difficulty, string name, string description, AISequence sequence) {
        this.difficulty = difficulty;
        this.name = name;
        this.description = description;
        this.sequence = sequence;

        this.GetChildren = sequence.GetChildren;
    }

    public Move(float difficulty, AISequence sequence) {
        Debug.LogWarning("Move created without name or description.");
        this.name = null;
        this.description = null;
        this.difficulty = difficulty;
        this.sequence = sequence;
    }
}
