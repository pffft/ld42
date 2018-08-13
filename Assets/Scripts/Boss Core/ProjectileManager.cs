using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class ProjectileManager {

    private Entity entity;

    public ProjectileManager(Entity source) {
        this.entity = source;
    }

    public void spawn1(Vector3 spawn, Vector3 target, float angleOffset = 0, Speed speed = Speed.MEDIUM) {
        Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

        // We want to ignore any verticality
        Vector3 topDownSpawn = new Vector3(spawn.x, 0, spawn.z);
        Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

        Projectile.Create(this.entity, spawn, offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn), (float)speed, 10f);
    }

    public enum Speed
    {
        SNAIL = 10,
        SLOW = 15,
        MEDIUM = 25,
        FAST = 35,
        VERY_FAST = 45,
        SNIPE = 50
    };
}
