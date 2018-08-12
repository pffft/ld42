using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager {

    public static void spawn1(Vector3 spawn, Vector3 target, float angleOffset = 0, Speed speed = Speed.MEDIUM)
    {
        Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

        Vector3 topDownTarget = new Vector3(target.x, 0, target.z);
        Projectile.Create(spawn, offset * Quaternion.FromToRotation(Vector3.right, topDownTarget), (float)speed, 10f);
    }

    public enum Speed
    {
        SNAIL = 5,
        SLOW = 10,
        MEDIUM = 15,
        FAST = 20,
        VERY_FAST = 25
    };
}
