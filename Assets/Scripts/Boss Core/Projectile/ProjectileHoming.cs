using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileHoming : Projectile
    {

        public GameObject player;
        public Rigidbody body;

        public float curDivergence;
        public float maxDivergence = 110f;

        // How many degrees per update can the projectile turn?
        // Proportional to the speed; more curve at higher speeds.
        public float homingScale;

        // Was this projectile once close to the player?
        public bool wasClose;

        public override void CustomUpdate() {
            Vector3 idealVelocity = velocity * (player.transform.position - transform.position).normalized;
            float idealRotation = Vector3.SignedAngle(idealVelocity, body.velocity, Vector3.up);

            float distance = Vector3.Distance(player.transform.position, transform.position);

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

            if (Mathf.Abs(idealRotation) >= 10f && Mathf.Abs(idealRotation) < 120f)
            {
                Quaternion rot = Quaternion.AngleAxis(-Mathf.Sign(idealRotation) * homingScale * feathering, Vector3.up);
                body.velocity = rot * body.velocity;
                curDivergence += homingScale;
            }
        }

		public override void Initialize(params object[] args) {
            this.player = GameObject.Find("Player");
            this.body = GetComponent<Rigidbody>();
            this.wasClose = false;
            this.curDivergence = 0f;
            this.homingScale = velocity / 7f;
		}
    }
}