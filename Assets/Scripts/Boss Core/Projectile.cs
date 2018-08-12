using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    


    public float currentTime;
    public float maxTime;

    public float velocity;

	void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= maxTime) {
            Destroy(this.gameObject);
        }
	}

    public static Projectile Create(Vector3 startPosition, Quaternion startRotation, float velocity, float maxTime) {

        // TODO: if things get laggy, then make a static reference to the projectile prefab.
        GameObject newObj = Instantiate(Resources.Load<GameObject>("Prefabs/ProjectileSmall")) as GameObject;
        Projectile projectile = newObj.GetComponent<Projectile>();

        Rigidbody body = newObj.GetComponent<Rigidbody>();
        if (body == null) {
            body = newObj.AddComponent<Rigidbody>();
        }
        body.velocity = startRotation * (Vector3.right * velocity);
        body.useGravity = false;

        newObj.transform.position = startPosition;
        newObj.transform.rotation = startRotation;

        projectile.currentTime = 0;
        projectile.maxTime = maxTime;
        projectile.velocity = velocity;

        return projectile;
    }
}
