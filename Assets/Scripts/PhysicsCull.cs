using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCull : MonoBehaviour {

    private SphereCollider col;

	// Use this for initialization
	void Start () {
        col = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () {
        col.enabled = (GameManager.Player.transform.position - transform.position).sqrMagnitude < 10 * 10;
        /*
        if ((GameManager.Player.transform.position - transform.position).sqrMagnitude < 10 * 10)
        {
            if (col == null)
            {
                col = this.gameObject.AddComponent<SphereCollider>();
                col.isTrigger = true;
            }
        }
        else
        {
            GameObject.Destroy(col);
            col = null;
        }
        */
    }
}
