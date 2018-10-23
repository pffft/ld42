using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCull : MonoBehaviour {

    private SphereCollider col;
    private Transform trans;
    private Transform playerTrans;

	// Use this for initialization
	void Start () {
        col = GetComponent<SphereCollider>();
        trans = transform;
        playerTrans = GameManager.Player.transform;
	}
	
	// Update is called once per frame
	public void Update () {
        col.enabled = (playerTrans.position - trans.position).sqrMagnitude < 10 * 10;
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
