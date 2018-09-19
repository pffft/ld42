using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public Entity entity;

        public Speed speed;
        public Size size;
        public Type type;

        public float currentTime;
        public float maxTime;

        public float damage;
        public float velocity;

        public static Material blueMaterial;
        public static Material orangeMaterial;
        public static Material greenMaterial;
        public static Material purpleMaterial;

        void Update()
        {
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                OnDestroyTimeout();
                Destroy(this.gameObject);
            }

            if (transform.position.magnitude > 100f)
            {
                OnDestroyOutOfBounds();
                Destroy(this.gameObject);
            }

            CustomUpdate();
        }

        /*
         * Called after object is destroyed due to time limit.
         */
        public virtual void OnDestroyTimeout() { }

        /*
         * Called after object is destroyed due to hitting the arena.
         */
        public virtual void OnDestroyOutOfBounds() { }

        /*
         * Called when the object hits the player
         */
        public virtual void OnDestroyCollision() { }

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
            Entity otherEntity = otherObject.GetComponent<Entity>();
            if (otherEntity != null)
            {
                if (otherEntity.GetFaction() != this.entity.GetFaction())
                {
                    Debug.Log("Projectile collided, should apply damage");
                    Entity.DamageEntity(otherEntity, this.entity, damage);
                    OnDestroyCollision();
                    Destroy(this.gameObject);
                }
            }
        }

        /*
         * Called upon creation with any additional parameters this object needs.
         */
        public virtual void Initialize(params object[] args) { }

        /*
         * Get the preferred material for this projectile.
         * The standard only sets material based on size; if you want your projectile
         * to have its own material, return it here.
         *
         * If you want to use the standard projectile logic, simply return null here.
         */
        public virtual Material GetCustomMaterial() { return null; }

        /*
         * Creates a new basic Projectile with the provided parameters. The default
         * will be a small, medium speed projectile that starts at the entity's position
         * and moves to the right; it will die after 10 seconds.
         */
        public static Projectile Create(Entity entity, Vector3? start=null, Vector3? target=null, float angleOffset=0f, float maxTime=10f, Speed speed=Speed.MEDIUM, Size size=Size.SMALL)
        {
            GameObject newObj;

            // Assign a default material based on the size; normally, small = blue, med = orange, large = red.
            switch (size)
            {
                case Size.TINY:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileTiny"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/BlueTransparent");
                    break;
                case Size.SMALL:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileSmall"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/BlueTransparent");
                    break;
                case Size.MEDIUM:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileMedium"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeTransparent");
                    break;
                case Size.LARGE:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileLarge"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeRedTransparent");
                    break;
                case Size.HUGE:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileHuge"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeRedTransparent");
                    break;
                default:
                    newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/ProjectileMedium"));
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeTransparent");
                    break;
            }

            // Resolve the start position. If the start is null, then we default to the entity's position.
            Vector3 startPosition = start ?? entity.transform.position;

            // Resolve the target position. If it's null, and this is a boss projectile, we check if
            // we're locked onto a position. If so, we take that position; else we take the current player position.
            // Otherwise, we take the target as-is; or a default forward if it's null.
            Vector3 targetPosition;
            if (entity.name.Equals(BossController.BOSS_NAME) && !target.HasValue) {
                if (BossController.isPlayerLocked) {
                    targetPosition = BossController.playerLockPosition;
                } else {
                    // TODO cache me
                    targetPosition = GameObject.Find("Player").transform.position;
                }
            } else if (target.HasValue) {
                targetPosition = target.Value;
            } else {
                targetPosition = Vector3.forward;
            }

            // Figure out the final rotation value based on the target and offset.
            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            // We need to ignore the y coordiante to get an accurate target vector.
            Vector3 topDownSpawn = new Vector3(startPosition.x, 0, startPosition.z);
            Vector3 topDownTarget = new Vector3(targetPosition.x, 0, targetPosition.z);

            // The final rotation is the offset added to the spawn-target rotation.
            Quaternion rotation = offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn);

            // Set the position and rotation.
            newObj.transform.position = startPosition;
            newObj.transform.rotation = rotation;


            Projectile projectile;

            // Look up the subclass we should add based on the type
            projectile = newObj.AddComponent<Projectile>();

            // Assign and init the RigidBody (or create one if it doesn't exist)
            Rigidbody body = newObj.GetComponent<Rigidbody>();
            if (body == null)
            {
                body = newObj.AddComponent<Rigidbody>();
            }
            body.velocity = rotation * (Vector3.forward * (float)speed);
            body.useGravity = false;

            projectile.speed = speed;
            projectile.size = size;

            projectile.entity = entity;
            projectile.currentTime = 0;
            projectile.maxTime = maxTime;
            projectile.velocity = (float)speed;
            projectile.damage = 5f;

            return projectile;
        }

        /*
         * Copies the data of this Projectile into a new one.
         * This then deletes the original Projectile component.
         * 
         * This method is used by extension methods to help cast to a given type.
        */
        public T CastTo<T>() where T : Projectile {
            T other = gameObject.AddComponent<T>();

            // Copy the data over
            other.entity = entity;

            other.speed = speed;
            other.size = size;
            other.currentTime = 0;
            other.maxTime = maxTime;

            other.damage = damage;
            other.velocity = velocity;

            // Assign a different, custom material (if applicable)
            Material customMaterial = other.GetCustomMaterial();
            if (customMaterial != null)
            {
                gameObject.GetComponent<MeshRenderer>().material = customMaterial;
            }

            // Destroy this component so the other one takes priority
            GameObject.Destroy(this);

            // Return the new component
            return other;
        }
    }
}
