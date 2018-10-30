using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;

public class ThrownShield : MonoBehaviour {

    private Entity entity;
    private Transform bossTransform;

    private Rigidbody body;
    private KeepOnArena keepOnArenaScript;

    // How fast do we move?
    private const BossCore.Speed speed = BossCore.Speed.LIGHTNING;

    // How many degrees per update can the shield rotate?
    // Proportional to the speed; more curve at higher speeds.
    private const float homingScale = (float)speed / 7f;

    // Was this projectile once close to the boss?
    private bool wasClose;

    private float curDivergence;
    private float maxDivergence = 110f;

    private float currentTime;
    private const float maxTime = 2f;

    private bool shouldUpdate = true;

	// Use this for initialization
	void Start () {
        entity = GetComponent<Entity>();
        bossTransform = GameManager.Boss.transform;

        body = GetComponent<Rigidbody>();
        keepOnArenaScript = GetComponent<KeepOnArena>();

        //body.velocity = (float)speed * (bossTransform.position - transform.position).normalized;
        body.velocity = (float)speed * (GameManager.Player.GetDashTargetPoint() - transform.position).normalized;
    }
	
	// Update is called once per frame
	void Update () {
        if (!shouldUpdate) {
            return;
        }

        // Out of time
        currentTime += Time.deltaTime;
        if (currentTime > maxTime) {
            StopUpdating();
        }

        // Out of bounds
        if (transform.position.magnitude > GameManager.Arena.RadiusInWorldUnits)
        {
            StopUpdating();
        }

        // Calculate the ideal values (i.e., where we want to face to hit the boss)
        Vector3 idealVelocity = ((float)speed) * (bossTransform.position - transform.position).normalized;
        float idealRotation = Vector3.SignedAngle(idealVelocity, body.velocity, Vector3.up);

        // Calculate distance between the shield and the boss
        float distance = Vector3.Distance(bossTransform.position, transform.position);

        // If this is our first time close to the boss, set the respective flag
        if (!wasClose && distance < 10f)
        {
            wasClose = true;
        }

        // If we are too far away (and were close to boss), stop tracking
        // If we curved the maximum amount possible in our lifetime, stop tracking
        if ((wasClose && distance > 10f) || curDivergence >= maxDivergence)
        {
            return;
        }

        // Feathering; we want to have a weak, tapered homing effect far away from the boss.
        float feathering = 1f;
        if (distance > 10f && distance < 25f)
        {
            feathering = (25f - distance) / 15f;
        }

        // If we're within a certain amount of degrees offset from the boss, home in on him
        if (Mathf.Abs(idealRotation) < 180f)
        {
            Quaternion rot = Quaternion.AngleAxis(-Mathf.Sign(idealRotation) * homingScale * feathering, Vector3.up);
            body.velocity = rot * body.velocity;
            body.velocity = new Vector3(body.velocity.x, 0, body.velocity.z);
            curDivergence += homingScale;
        }
        //Debug.Log("Current velocity magnitude: " + body.velocity.magnitude);
    }

    public void OnShieldTriggerEntered(Collider other) 
    {
        if (!shouldUpdate)
        {
            return;
        }

        GameObject otherObject = other.gameObject;
        Entity otherEntity = otherObject.GetComponentInParent<Entity>();
        Debug.Log(otherObject.name);
        if (otherEntity != null)
        {
            // Do damage if the target isn't invincible and not on our team
            if (otherEntity.GetFaction() != Entity.Faction.player)
            {
                if (!otherEntity.IsInvincible())
                {
                    Entity.DamageEntity(otherEntity, entity, 5f);
                }
                Bounce();
            }
        }
    }

    // Just stop updating the shield; velocity will decay
    private void StopUpdating()
    {
        shouldUpdate = false;

        body.useGravity = true;
        body.isKinematic = false;

        // Make sure it'll stay on the arena
        keepOnArenaScript.shouldReset = true;
    }

    // Bounce off the boss
    private void Bounce()
    {
        shouldUpdate = false;

        body.useGravity = true;
        body.isKinematic = false;

        // Randomly choose straight left or right as a base direction
        float degrees = Random.Range(0, 2) == 0 ? -90 : 90;

        // Add a +/- 45 degree offset from the base
        float offset = Random.Range(-1f, 1f);
        degrees += offset * 45f;

        // Make the shield go that way
        Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);
        Vector3 force = rotation * body.velocity.normalized;
        body.velocity = Vector3.zero;
        body.AddForce(250f * force, ForceMode.Impulse);

        // Make sure the shield stays on the arena
        keepOnArenaScript.shouldReset = true;
    }
}
