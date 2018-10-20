using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileHoming : ProjectileComponent
    {

        public GameObject targetObject;
        public Rigidbody body;

        public float curDivergence;
        public float maxDivergence = 110f;

        // How many degrees per update can the projectile turn?
        // Proportional to the speed; more curve at higher speeds.
        public float homingScale;

        // Was this projectile once close to the player?
        public bool wasClose;

        public override Material GetCustomMaterial() {
            return Resources.Load<Material>("Art/Materials/PurpleTransparent");
        }
        
        public override void CustomUpdate() {
            Vector3 idealVelocity = ((float)data.speed) * (targetObject.transform.position - transform.position).normalized;
            float idealRotation = Vector3.SignedAngle(idealVelocity, body.velocity, Vector3.up);

            float distance = Vector3.Distance(targetObject.transform.position, transform.position);

            if (!wasClose && distance < 10f)
            {
                wasClose = true;
            }

            if ((wasClose && distance > 10f) || curDivergence >= maxDivergence)
            {
                return;
            }

            float feathering = 1f;
            if (distance > 10f && distance < 25f)
            {
                feathering = (25f - distance) / 15f;
            }

            float distanceScale = 1f; //distance < 15f ? 1 + ((15 - distance) / 5f) : 1f;
            if (Mathf.Abs(idealRotation) >= 10f && Mathf.Abs(idealRotation) < 120f)
            {
                Quaternion rot = Quaternion.AngleAxis(-Mathf.Sign(idealRotation) * homingScale * feathering * distanceScale, Vector3.up);
                body.velocity = rot * body.velocity;
                body.velocity = (distanceScale * (float)data.speed) * body.velocity.normalized;
                curDivergence += homingScale;
            }
        }
    }

    public static class ProjectileHomingHelper {
        public static ProjectileHoming Homing(this ProjectileComponent projectile)
        {
            ProjectileHoming homing = projectile.CastTo<ProjectileHoming>();

            homing.targetObject = projectile.data.entity.GetFaction() == Entity.Faction.enemy? GameManager.Player.gameObject : GameManager.Boss.gameObject;
            homing.body = projectile.GetComponent<Rigidbody>();
            homing.wasClose = false;
            homing.curDivergence = 0f;
            homing.homingScale = (float)projectile.data.speed / 7f;

            return homing;
        }

        public static Projectile Homing(this Projectile structure) {
            structure.type = Type.HOMING;
            return structure;
        }
    }
}