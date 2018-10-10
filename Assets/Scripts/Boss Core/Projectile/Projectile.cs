using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        public ProjectileData data;

        public static Material blueMaterial;
        public static Material orangeMaterial;
        public static Material orangeRedMaterial;

        /*
         * Creates a default Projectile component for the builder notation.
         * By default, this is a small, medium speed projectile with a max life
         * of 10. It will start at the entity's position and aim at the player
         * (if the entity is the boss), or else aim forward (for the player).
         */
        public static ProjectileData New(Entity entity) {
            return new ProjectileData(entity);
        }

        public void Awake()
        {
            if (blueMaterial == null)
            {
                blueMaterial = Resources.Load<Material>("Art/Materials/BlueTransparent");
                orangeMaterial = Resources.Load<Material>("Art/Materials/OrangeTransparent");
                orangeRedMaterial = Resources.Load<Material>("Art/Materials/OrangeRedTransparent");
            }

            // Sets start
            data.start = data.preStart ?? data.entity.transform.position;

            // Sets target
            Vector3 targetPosition;
            if (data.preTarget == null)
            {
                if (data.entity.name.Equals(BossController.BOSS_NAME))
                {
                    // TODO cache me
                    targetPosition = BossController.isPlayerLocked ?
                       BossController.playerLockPosition :
                       GameObject.Find("Player").transform.position;
                }
                else
                {
                    targetPosition = Vector3.forward;
                }
            }
            else
            {
                targetPosition = data.preTarget.Value;
            }
            data.target = targetPosition;

            // Sets size (and assigns default material)
            gameObject.transform.localScale = SizeToScale(data.size) * Vector3.one;
            switch (data.size)
            {
                case Size.TINY:
                case Size.SMALL:
                    gameObject.GetComponent<MeshRenderer>().material = blueMaterial;
                    break;
                case Size.MEDIUM:
                    gameObject.GetComponent<MeshRenderer>().material = orangeMaterial;
                    break;
                case Size.LARGE:
                case Size.HUGE:
                    gameObject.GetComponent<MeshRenderer>().material = orangeRedMaterial;
                    break;
                default:
                    gameObject.GetComponent<MeshRenderer>().material = orangeMaterial;
                    break;
            }

            UpdateOrientationAndVelocity();
        }


        /* Updates this Projectile's orientation and velocity. Called when the start,
         * target, angleOffset, or speed are changed using the builder methods.
         */
        private void UpdateOrientationAndVelocity()
        {
            // Remove any height from the start and target vectors
            Vector3 topDownSpawn = new Vector3(data.start.x, 0, data.start.z);
            Vector3 topDownTarget = new Vector3(data.target.x, 0, data.target.z);

            // Add in rotation offset from the angleOffset parameter
            Quaternion offset = Quaternion.AngleAxis(data.angleOffset, Vector3.up);

            // Compute the final rotation
            // TODO update this to be rotation around the up axis to fix 180 degree bug
            Quaternion rotation = offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn);

            this.gameObject.transform.position = data.start;
            this.gameObject.transform.rotation = rotation;
            this.gameObject.GetComponent<Rigidbody>().velocity = rotation * (Vector3.forward * (float)data.speed);
        }

        /*
         * A helper function that retuns the local scale of a projectile, given
         * its size. This corresponds to its diameter.
         */
        public static float SizeToScale(Size size)
        {
            return 1.0f + ((float)size / 2.0f);
        }

        void Update()
        {
            Profiler.BeginSample("Projectile update loop");
            data.currentTime += Time.deltaTime;

            if (data.currentTime >= data.maxTime)
            {
                data.OnDestroyTimeoutImpl(this);
                Destroy(this.gameObject);
            }

            if (transform.position.magnitude > 100f)
            {
                data.OnDestroyOutOfBoundsImpl(this);
                Destroy(this.gameObject);
            }

            Profiler.BeginSample("Projectile custom update");
            CustomUpdate();
            Profiler.EndSample();
            Profiler.EndSample();
        }

        /*
         * Called at the end of every frame on update.
         * Will stop as soon as object dies.
         */
        public virtual void CustomUpdate() { }

        /*
         * Called on collision with player. Triggers collison death.
         */
        public virtual void OnTriggerEnter(Collider other)
        {
            GameObject otherObject = other.gameObject;
            Entity otherEntity = otherObject.GetComponentInParent<Entity>();
            if (otherEntity != null)
            {
                // All projectiles break if they do damage
                if (!otherEntity.IsInvincible() && otherEntity.GetFaction() != data.entity.GetFaction())
                {
                    Debug.Log("Projectile collided, should apply damage");
                    Entity.DamageEntity(otherEntity, data.entity, data.damage);
                    data.OnDestroyCollisionImpl(this);
                    Destroy(this.gameObject);
                }

                // Player's projectiles always break on the boss, even if he's invincible
                if (data.entity.GetFaction() == Entity.Faction.player) {
                    if (otherEntity.IsInvincible()) {
                        data.OnDestroyCollisionImpl(this);
                        Destroy(this.gameObject);
                    }
                }
            }
        }

        /*
         * Get the preferred material for this projectile.
         * The standard only sets material based on size; if you want your projectile
         * to have its own material, override this method and return it here.
         *
         * If you want to use the standard projectile logic, simply return null here.
         */
        public virtual Material GetCustomMaterial() { return null; }

        /*
         * Copies the data of this Projectile into a new one.
         * This then deletes the original Projectile component.
         * 
         * This method is used by extension methods to help cast to a given type.
        */
        public T CastTo<T>() where T : Projectile {
            gameObject.SetActive(false);
            T other = gameObject.AddComponent<T>();

            // Copy the data over
            //other.data = new ProjectileStructure();
            other.data = data.Clone();
            gameObject.SetActive(true);

            // Assign a different, custom material (if applicable)
            Material customMaterial = other.GetCustomMaterial();
            if (customMaterial != null)
            {
                gameObject.GetComponent<MeshRenderer>().material = customMaterial;
            }

            // Destroy this component so the other one takes priority
            Destroy(this);

            // Return the new component
            return other;
        }
    }
}
