using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class ProjectileHoming : Projectile {

    private GameObject player;
    private Rigidbody body;

    private float curDivergence;
    private float maxDivergence = 110f;

    // How many degrees per update can the projectile turn?
    // Proportional to the speed; more curve at higher speeds.
    private float homingScale;

    // Was this projectile once close to the player?
    private bool wasClose;

	// Update is called once per frame
	void Update () {
        currentTime += Time.deltaTime;
        //Debug.Log(transform.position.magnitude);
        if (currentTime >= maxTime || transform.position.magnitude > 50f)
        {
            Destroy(this.gameObject);
        }

        Vector3 idealVelocity = velocity * (player.transform.position - transform.position).normalized;
        float idealRotation = Vector3.SignedAngle(idealVelocity, body.velocity, Vector3.up);

        float distance = Vector3.Distance(player.transform.position, transform.position);

        if (!wasClose && distance < 10f) {
            wasClose = true;
        }

        if ((wasClose && distance > 10f) || curDivergence >= maxDivergence) {
            return;
        }

        float feathering = 1f;
        if (distance > 10f && distance < 25f) {
            feathering = (25f - distance) / 15f;
        }

        if (Mathf.Abs(idealRotation) >= 10f && Mathf.Abs(idealRotation) < 120f) {
            Quaternion rot = Quaternion.AngleAxis(-Mathf.Sign(idealRotation) * homingScale * feathering, Vector3.up);
            body.velocity = rot * body.velocity;
            curDivergence += homingScale;
        }


	}

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        Entity otherEntity = otherObject.GetComponent<Entity>();
        if (otherEntity != null)
        {
            if (otherEntity.GetFaction() != this.entity.GetFaction())
            {
                Entity.DamageEntity(otherEntity, this.entity, 5f);
            }
        }
    }

    public static new ProjectileHoming Create(Entity entity, Vector3 startPosition, Quaternion startRotation, float velocity, float maxTime)
    {

        // TODO: if things get laggy, then make a static reference to the projectile prefab.
        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileHoming")) as GameObject;
        ProjectileHoming projectile = newObj.GetComponent<ProjectileHoming>();

        Rigidbody body = newObj.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = newObj.AddComponent<Rigidbody>();
        }
        body.velocity = startRotation * (Vector3.forward * velocity);
        body.useGravity = false;

        newObj.transform.position = startPosition;
        newObj.transform.rotation = startRotation;

        projectile.player = GameObject.Find("Player");
        projectile.body = body;
        projectile.wasClose = false;
        projectile.curDivergence = 0f;
        //projectile.maxDivergence = Random.Range(60f, 120f);
        projectile.homingScale = velocity / 7f;

        projectile.entity = entity;
        projectile.currentTime = 0;
        projectile.maxTime = maxTime;
        projectile.velocity = velocity;

        return projectile;
    }
}
