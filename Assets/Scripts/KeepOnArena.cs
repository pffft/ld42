using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Keeps objects within the constraints of the arena.
public class KeepOnArena : MonoBehaviour {

    public bool shouldReset = false;
    private Rigidbody body;

    // Use this for initialization
    void Start () {
        this.body = GetComponent<Rigidbody>();
    }
    
    // Update is called once per frame
    void Update () {
        if (shouldReset)
        {
            //if (transform.position.magnitude > Constants.Positions.RadiusInWorldUnits)
            if (transform.position.y < -1f)
            {
                float randomDegrees = Random.Range(0f, 359f);
                float randomWidth = Random.Range(5f, GameManager.Arena.RadiusInWorldUnits);
                transform.position = (Quaternion.AngleAxis(randomDegrees, Vector3.up) * (randomWidth * Vector3.forward)) + Vector3.up * 10f;

                if (body != null)
                {
                    body.velocity = Vector3.zero;
                    body.drag = 0f;
                    //body.AddForce(100f * new Vector3(0f, -1f, 0f), ForceMode.Impulse);
                    body.AddForceAtPosition(100f * Vector3.down, new Vector3(Random.value, 0f, Random.value));
                }
            }
        }
    }
}
