﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using AOEs;

namespace Moves
{
    //public abstract class Move : AISequence
    //{
    //    protected Move()
    //    {
    //        Debug.Log("Move constructor called");
    //        Initialize();
    //    }

    //    /*
    //     * A relative difficulty parameter. 
    //     * 
    //     * This is from a scale of 0 - 10, where a "10" is the point where any person
    //     * would call the move "actual bullshit". That means that the move may guarantee
    //     * damage, might not have safespots, might be too fast, or all of the above.
    //     * 
    //     * Most moves that make it to the game should be at most an 8.
    //     * 
    //     * This can go above 10, but that's for testing purposes (or masochism).
    //     */
    //    public virtual float GetDifficulty()
    //    {
    //        return 0f;
    //    }

    //    /*
    //     * What's the name of this Move?
    //     * 
    //     * The default value is the name of the class.
    //     */
    //    public virtual string GetName() {
    //        return GetType().Name;
    //    }

    //    /*
    //     * What does this Move do?
    //     * 
    //     * The default value is autogenerated from the provided AISequence.
    //     * You should override this method with a more descriptive bit of text.
    //     */
    //    public virtual string GetDescription() {
    //        return GetSequence().description;
    //    }

    //    /*
    //     * The actual AISequence that we're using.
    //     * 
    //     * This should be a "pure" sequence- without any delays before or after
    //     * the actual body, and without any teleporting (except as part of the
    //     * move itself). Those should be added in the respective methods.
    //     */
    //    public abstract AISequence GetSequence();

    //    public void Initialize() {
    //        this.description = GetDescription();
    //        this.GetChildren = () => GetSequence().GetChildren();
    //    }
    //}

}
