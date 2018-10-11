using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps objects within the constraints of the arena.
public class KeepOnArena : MonoBehaviour {

    public bool shouldReset = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (shouldReset)
        {
            //if (transform.position.magnitude > World.Arena.RadiusInWorldUnits)
            if (transform.position.y < -1f)
            {
                float randomDegrees = Random.Range(0f, 359f);
                float randomWidth = Random.Range(5f, GameManager.Arena.RadiusInWorldUnits);
                transform.position = (Quaternion.AngleAxis(randomDegrees, Vector3.up) * (randomWidth * Vector3.forward)) + Vector3.up * 10f;
            }
        }
	}
}
