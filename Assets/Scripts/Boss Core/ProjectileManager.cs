using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager {

    public static void spawn1(Vector3 spawn, Vector3 target)
    {
        Projectile.Create(spawn, Quaternion.FromToRotation(Vector3.right, target), 1f, 10f);
    }

}
