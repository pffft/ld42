using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Entity entity;

    public float currentTime;
    public float maxTime;

    public float velocity;

	void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime || transform.position.magnitude > 50f) {
            Destroy(this.gameObject);
        }
	}

	private void OnTriggerEnter(Collider other)
	{
        GameObject otherObject = other.gameObject;
        Entity otherEntity = otherObject.GetComponent<Entity>();
        if (otherEntity != null) {
            if (otherEntity.GetFaction() != this.entity.GetFaction()) {
                Entity.DamageEntity(otherEntity, this.entity, 5f);
            }
        }
	}

	public static Projectile Create(Entity entity, Vector3 startPosition, Quaternion startRotation, float velocity, float maxTime) {

        // TODO: if things get laggy, then make a static reference to the projectile prefab.
        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileSmall")) as GameObject;
        Projectile projectile = newObj.GetComponent<Projectile>();

        Rigidbody body = newObj.GetComponent<Rigidbody>();
        if (body == null) {
            body = newObj.AddComponent<Rigidbody>();
        }
        body.velocity = startRotation * (Vector3.forward * velocity);
        body.useGravity = false;

        newObj.transform.position = startPosition;
        newObj.transform.rotation = startRotation;

        projectile.entity = entity;
        projectile.currentTime = 0;
        projectile.maxTime = maxTime;
        projectile.velocity = velocity;

        return projectile;
    }

    public static Projectile CreateMedium(Entity entity, Vector3 startPosition, Quaternion startRotation, float velocity, float maxTime)
    {

        // TODO: if things get laggy, then make a static reference to the projectile prefab.
        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileMedium")) as GameObject;
        Projectile projectile = newObj.GetComponent<Projectile>();

        Rigidbody body = newObj.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = newObj.AddComponent<Rigidbody>();
        }
        body.velocity = startRotation * (Vector3.forward * velocity);
        body.useGravity = false;

        newObj.transform.position = startPosition;
        newObj.transform.rotation = startRotation;

        projectile.entity = entity;
        projectile.currentTime = 0;
        projectile.maxTime = maxTime;
        projectile.velocity = velocity;

        return projectile;
    }
}
