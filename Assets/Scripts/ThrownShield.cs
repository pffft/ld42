using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;
using Constants;

public class ThrownShield : MonoBehaviour {

    private Entity entity;
    private Transform bossTransform;

    private Rigidbody body;
    private KeepOnArena keepOnArenaScript;

    // How fast do we move?
    private const Speed speed = Speed.LIGHTNING;

    private float currentTime;
    private const float maxTime = 2f;

    private float initialIdealRotation;
    private float distance;

    private bool shouldDoDamage = true;

    // Use this for initialization
    void Start () {
        entity = GetComponent<Entity>();
        bossTransform = GameManager.Boss.transform;

        body = GetComponent<Rigidbody>();
        keepOnArenaScript = GetComponent<KeepOnArena>();

        // Compute the angle between shield throw and the boss. Save that value
        body.velocity = (float)speed * (GameManager.Player.GetDashTargetPoint() - transform.position).normalized;

        Vector3 idealPosition = bossTransform.position - transform.position;
        idealPosition = new Vector3(idealPosition.x, 0, idealPosition.z);

        Vector3 idealVelocity = ((float)speed) * idealPosition.normalized;
        initialIdealRotation = Vector3.SignedAngle(idealVelocity, body.velocity, Vector3.up);

        // Grab the initial distance between the player and boss; used for computing curvature
        distance = idealPosition.magnitude;
    }
    
    // Update is called once per frame
    void Update () {
        // Out of time; fall down and don't home anymore (disable this script)
        currentTime += Time.deltaTime;
        if (currentTime > maxTime) {
            enabled = false;
            Unfreeze();
        }

        // Out of bounds; continue to home, but also fall down
        if (transform.position.magnitude > GameManager.Arena.RadiusInWorldUnits)
        {
            Unfreeze();
        }

        // If the initial rotation is "close enough", then curve the shield throw
        // so that it'll hit the boss.
        if (Mathf.Abs(initialIdealRotation) < 12.5f)
        {
            Quaternion rotation = Quaternion.AngleAxis(-initialIdealRotation * Time.deltaTime * (130f / distance), Vector3.up);
            body.velocity = rotation * body.velocity;
        }
    }

    public void OnShieldTriggerEntered(Collider other) 
    {
        // If we've already done damage, we can't do damage again until the player
        // picks up the shield and rethrows it.
        if (!shouldDoDamage) 
        {
            return;
        }

        GameObject otherObject = other.gameObject;
        Entity otherEntity = otherObject.GetComponentInParent<Entity>();
        if (otherEntity != null)
        {
            // Do damage if the target isn't invincible and not on our team
            if (otherEntity.GetFaction() != Entity.Faction.player)
            {
                if (!otherEntity.IsInvincible())
                {
                    Entity.DamageEntity(otherEntity, entity, 5f);
                }

                // Bounce off the target and prevent duplicate damage.
                Bounce();
                shouldDoDamage = false;
            }
        }
    }

    // Makes the shield fall down due to gravity, and interact with objects in the world
    public void Unfreeze()
    {
        // Unfreeze y position constraint; make it spin for fun
        body.constraints = RigidbodyConstraints.None;
        body.useGravity = true;
        body.isKinematic = false;

        body.drag = 1f;

        // Make sure it'll stay on the arena
        keepOnArenaScript.shouldReset = true;
    }

    // Bounce off the boss
    private void Bounce()
    {
        enabled = false;
        Unfreeze();

        // Randomly choose straight left or right as a base direction
        float degrees = Random.Range(0, 2) == 0 ? -90 : 90;

        // Add a +/- 45 degree offset from the base
        float offset = Random.Range(-1f, 1f);
        degrees += offset * 45f;

        // Make the shield go that way
        Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);
        //Debug.Log("Bounce degree offset: " + degrees);
        Vector3 force = rotation * body.velocity.normalized;
        //Debug.Log("Bounce force: " + force);
        body.velocity = Vector3.zero;
        body.AddForce(250f * force, ForceMode.Impulse);
    }
}
