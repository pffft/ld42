using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class ProjectileCurve : Projectile
{

    private Rigidbody body;
    private float curveAmount;

    private int count = 0;
    private float numSpawners = 30f;

    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime || transform.position.magnitude > 50f)
        {
            Destroy(this.gameObject);
        }

        Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * curveAmount, Vector3.up);
        body.velocity = rot * body.velocity;

        if (currentTime > count / numSpawners) {
            count++;
            Projectile.Create(entity, transform.position, Quaternion.identity, 0, maxTime - currentTime);
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

    public static ProjectileCurve Create(Entity entity, Vector3 startPosition, Quaternion startRotation, float velocity, float curveAmount, float maxTime)
    {

        // TODO: if things get laggy, then make a static reference to the projectile prefab.
        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileCurving")) as GameObject;
        ProjectileCurve projectile = newObj.GetComponent<ProjectileCurve>();

        Rigidbody body = newObj.GetComponent<Rigidbody>();
        if (body == null)
        {
            body = newObj.AddComponent<Rigidbody>();
        }
        body.velocity = startRotation * (Vector3.forward * velocity);
        body.useGravity = false;

        newObj.transform.position = startPosition;
        newObj.transform.rotation = startRotation;

        projectile.body = body;
        projectile.curveAmount = curveAmount;
        projectile.entity = entity;
        projectile.currentTime = 0;
        projectile.maxTime = maxTime;
        projectile.velocity = velocity;

        return projectile;
    }
}
