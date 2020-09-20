using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Very basic pass-through for the thrown shield. 
 * We want the shield on a layer separate from the boss (so that it doesn't clip
 * into him), but it still needs to have a trigger for detecting collisions.
 */
public class ThrownShieldTrigger : MonoBehaviour {

    private ThrownShield shield;

    // Use this for initialization
    void Start () {
        shield = GetComponentInParent<ThrownShield>();
    }

    private void OnTriggerEnter(Collider other)
    {
        shield.OnShieldTriggerEntered(other);
    }
}
