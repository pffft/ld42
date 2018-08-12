using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class BossController : MonoBehaviour {

    private GameObject player;

    private Rigidbody physbody;
    private CombatCore.Entity self;

    private void Awake() {
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();
	}

	// Use this for initialization
	void Start () {
        self.AddAbility(new CombatCore.Ability("shoot", "", null, 1, 1, Shoot1), 2);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos = player.transform.position;

        if (Input.GetKey(KeyCode.P)) {
            if (self.GetAbility(2).Use(self, playerPos)) {}
        }
	}

    private bool Shoot1(Entity subject, Vector3 targetPosition, params object[] args) {
        ProjectileManager.spawn1(transform.position, player.transform.position);
        return true;
    }
}
