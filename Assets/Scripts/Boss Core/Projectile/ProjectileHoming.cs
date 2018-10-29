using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileHoming : Projectile
    {

        private GameObject targetObject;

        private float curDivergence;
        private float maxDivergence = 110f;

        // How many degrees per update can the projectile turn?
        // Proportional to the speed; more curve at higher speeds.
        private float homingScale;

        // Was this projectile once close to the player?
        private bool wasClose;

        public ProjectileHoming() : this(GameManager.Boss.self) { }

        public ProjectileHoming(Entity self) : base(self) { }

        public override void CustomCreate(ProjectileComponent component)
        {
            targetObject = component.data.Entity.GetFaction() == Entity.Faction.enemy ? GameManager.Player.gameObject : GameManager.Boss.gameObject;
            wasClose = false;
            curDivergence = 0f;
            homingScale = (float)component.data.Speed / 7f;
        }

        public override Material CustomMaterial() {
            return Resources.Load<Material>("Art/Materials/PurpleTransparent");
        }
        
        public override void CustomUpdate(ProjectileComponent component) {
            Vector3 idealVelocity = ((float)Speed) * (targetObject.transform.position - component.transform.position).normalized;
            float idealRotation = Vector3.SignedAngle(idealVelocity, Velocity, Vector3.up);

            float distance = Vector3.Distance(targetObject.transform.position, component.transform.position);

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
                //body.velocity = rot * body.velocity;
                Velocity = rot * Velocity;
                //body.velocity = (distanceScale * (float)speed) * body.velocity.normalized;
                Velocity = (distanceScale * (float)Speed) * Velocity.normalized;
                curDivergence += homingScale;
            }
        }
    }
}