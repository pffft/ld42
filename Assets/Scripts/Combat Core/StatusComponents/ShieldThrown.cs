﻿using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace CombatCore.StatusComponents
{
    public class ShieldThrown : StatusComponent
    {

        private Vector3 target;

        public void SetTarget(Vector3 target) {
            this.target = target;
        }

        public override void OnApply(Entity subject)
        {
            GameManager.HeldShield.SetActive(false);

            // TODO: fix this to work properly. Projectile system will be private to Boss
            // in some future release.

            // Spawns a homing projectile
            ProjectileComponent homingProj = new ProjectileHoming(subject)
            {
                Target = target,
                MaxTime = 2f,
                Size = Size.MEDIUM,
                OnDestroyTimeout = StopMoving,
                OnDestroyOutOfBounds = StopMoving,
                OnDestroyCollision = Bounce
            }.Create();


            //.. and then makes a shield instance as a child of the projectile
            GameObject.Destroy(homingProj.GetComponent<MeshRenderer>());

            // Try to spawn a shield. If one exists (it shouldn't), fail.
            if (GameManager.ThrownShield == null)
            {
                GameManager.ThrownShield = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Shield"));
                GameManager.ThrownShield.name = "Thrown Shield";
                GameManager.ThrownShield.transform.position = subject.transform.position;
                GameManager.ThrownShield.transform.parent = homingProj.transform;
                GameManager.ThrownShield.GetComponent<KeepOnArena>().shouldReset = false;
            }
        }

        // Just make the shield freeze
        private static readonly ProjectileCallback StopMoving = (self) =>
        {
            Transform child = self.transform.GetChild(0);
            Rigidbody body = child.GetComponent<Rigidbody>();
            self.gameObject.AddComponent<MeshRenderer>();

            body.useGravity = true;
            body.isKinematic = false;
            body.velocity = Vector3.zero;

            self.transform.GetChild(0).GetComponent<KeepOnArena>().shouldReset = true;
            self.transform.GetChild(0).parent = null;
            return AI.AISequence.Pause(0f);
        };

        // Makes the shield "bounce" in a randomized direction
        private static readonly ProjectileCallback Bounce = (self) =>
        {
            self.gameObject.AddComponent<MeshRenderer>();
            if (self.transform.childCount < 1) return AI.AISequence.Pause(0f);
            Rigidbody body = self.transform.GetChild(0).GetComponent<Rigidbody>();
            body.useGravity = true;
            body.isKinematic = false;

            // Randomly choose (relative) left or right as a base direction
            float degrees = Random.Range(0, 2) == 0 ? -90 : 90;

            // Add +/- 45 degrees to that direction
            float offset = Random.Range(-1f, 1f);
            degrees += offset * 45f;

            // Then make the shield go that way
            Quaternion rotation = Quaternion.AngleAxis(degrees, Vector3.up);
            body.AddForce(250f * (rotation * self.GetComponent<Rigidbody>().velocity.normalized), ForceMode.Impulse);
            self.transform.GetChild(0).GetComponent<KeepOnArena>().shouldReset = true;
            self.transform.GetChild(0).parent = null;
            return AI.AISequence.Pause(0f);
        };

        public override void OnRevert(Entity subject)
        {
            GameObject.Destroy(GameManager.ThrownShield);
            GameManager.ThrownShield = null;

            GameManager.HeldShield.SetActive(true);
            GameManager.HUD.shieldAvailable = true;
        }
    }
}