using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used for moving the dummy player in the Boss Observer scene.
 */
public class MoveRandomly : MonoBehaviour {

    private bool moving = false;
    private Vector3 newPosition;

    // Use this for initialization
    void Start () {
        
    }
    
    // Update is called once per frame
    void Update () {
        if (!moving) {
            newPosition = Constants.Positions.RANDOM_IN_ARENA.GetValue();
            moving = true;
        }

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime);
        if ((transform.position - newPosition).sqrMagnitude < 1f) {
            moving = false;
        }
    }
}
