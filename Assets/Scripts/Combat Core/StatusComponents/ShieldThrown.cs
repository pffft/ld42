using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace CombatCore.StatusComponents
{
    public class ShieldThrown : StatusComponent
    {

        private Vector3 target;
        private GameObject shieldThrown = null;

        public void SetTarget(Vector3 target) {
            this.target = target;
        }

        public override void OnApply(Entity subject)
        {
            Controller.playerShield.SetActive(false);

            // Spawns a homing projectile
            Projectile homingProj = Projectile.New(subject)
                                              .Target(target)
                                              .MaxTime(2f)
                                              .Speed(BossCore.Speed.VERY_FAST)
                                              .Size(Size.MEDIUM)
                                              .Homing()
                                              .OnDestroyTimeout(StopMoving)
                                              .OnDestroyOutOfBounds(StopMoving)
                                              .OnDestroyCollision(Bounce)
                                              .Create();

            //.. and then makes a shield instance as a child of the projectile
            GameObject.Destroy(homingProj.GetComponent<MeshRenderer>());

            // Try to spawn a shield. If one exists (it shouldn't), fail.
            if (shieldThrown == null)
            {
                shieldThrown = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Shield"));
                shieldThrown.name = "Thrown Shield";
                shieldThrown.transform.position = subject.transform.position;
                shieldThrown.transform.parent = homingProj.transform;
                shieldThrown.GetComponent<KeepOnArena>().shouldReset = false;
            }
        }

        // Just make the shield freeze
        private static readonly ProjectileCallbackDelegate StopMoving = (self) =>
        {
            Transform child = self.transform.GetChild(0);
            Rigidbody body = child.GetComponent<Rigidbody>();

            body.useGravity = true;
            body.isKinematic = false;
            body.velocity = Vector3.zero;

            self.transform.GetChild(0).GetComponent<KeepOnArena>().shouldReset = true;
            self.transform.GetChild(0).parent = null;

        };

        // Makes the shield "bounce" in a randomized direction
        private static readonly ProjectileCallbackDelegate Bounce = (self) =>
        {
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
            self.transform.GetChild(0).parent = null;
        };

        public override void OnRevert(Entity subject)
        {
            GameObject.Destroy(shieldThrown);
            shieldThrown = null;

            Controller.playerShield.SetActive(true);
            GameObject.Find("HUD").GetComponent<HUD>().shieldAvailable = true;
        }
    }
}