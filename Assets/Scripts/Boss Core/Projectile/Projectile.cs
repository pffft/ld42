using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    /*
     * Used for handling death events for Projectiles.
     * In the future, other callbacks might be added.
     */
    public delegate void ProjectileCallbackDelegate(Projectile self);

    public class Projectile : MonoBehaviour
    {
        public Entity entity;

        // Used internally for the builder notation
        public Vector3 start;
        public Vector3 target;
        public float angleOffset;

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

        /*
         * Creates a default Projectile component for the builder notation.
         * By default, this is a small, medium speed projectile with a max life
         * of 10. It will start at the entity's position and aim at the player
         * (if the entity is the boss), or else aim forward (for the player).
         */
        public static Projectile Create(Entity entity)
        {
            // Create new GameObject
            GameObject newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Projectile"));

            // Default start and target
            Vector3 startPosition = entity.transform.position;
            Vector3 targetPosition;
            if (entity.name.Equals(BossController.BOSS_NAME)) {
                // TODO cache me
                targetPosition = BossController.isPlayerLocked ? 
                   BossController.playerLockPosition : 
                   GameObject.Find("Player").transform.position;
            } else {
                targetPosition = Vector3.forward;
            }

            // Create a new Projectile component
            Projectile projectile = newObj.AddComponent<Projectile>();

            // Assign and init the RigidBody (or create one if it doesn't exist)
            Rigidbody body = newObj.GetComponent<Rigidbody>();
            if (body == null)
            {
                body = newObj.AddComponent<Rigidbody>();
            }
            body.useGravity = false;

            // Set up basic projectile information
            projectile.entity = entity;
            projectile.currentTime = 0;
            projectile.damage = 5f;

            // Set defaults.
            return projectile
                .SetStart(startPosition)
                .SetTarget(targetPosition)
                .SetAngleOffset(0f)
                .SetMaxTime(10f)
                .SetSpeed(Speed.MEDIUM)
                .SetSize(Size.SMALL);
        }

        #region Builder Methods

        // Builder method
        public Projectile SetStart(Vector3? start) 
        {
            this.start = start ?? entity.transform.position;
            UpdateOrientationAndVelocity();
            return this;
        }

        // Builder method
        public Projectile SetTarget(Vector3? target)
        {
            Vector3 targetPosition;
            if (target == null) {
                if (entity.name.Equals(BossController.BOSS_NAME))
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
            } else {
                targetPosition = target.Value;
            }

            this.target = targetPosition;
            UpdateOrientationAndVelocity();
            return this;
        }

        // Builder method
        public Projectile SetAngleOffset(float offsetDegrees) {
            this.angleOffset = offsetDegrees;
            UpdateOrientationAndVelocity();
            return this;
        }

        // Builder method
        public Projectile SetMaxTime(float seconds) {
            this.maxTime = seconds;
            return this;
        }

        // Builder method
        public Projectile SetSpeed(Speed speed) {
            this.speed = speed;
            UpdateOrientationAndVelocity();
            return this;
        }

        // Builder method
        public Projectile SetSize(Size size) {
            this.size = size;
            gameObject.transform.localScale = SizeToScale(size) * Vector3.one;
            switch (size)
            {
                case Size.TINY:
                case Size.SMALL:
                    gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/BlueTransparent");
                    break;
                case Size.MEDIUM:
                    gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeTransparent");
                    break;
                case Size.LARGE:
                case Size.HUGE:
                    gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeRedTransparent");
                    break;
                default:
                    gameObject.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/OrangeTransparent");
                    break;
            }

            return this;
        }

        /* Updates this Projectile's orientation and velocity. Called when the start,
         * target, angleOffset, or speed are changed using the builder methods.
         */
        private void UpdateOrientationAndVelocity()
        {
            Vector3 start = this.start;

            // Remove any height from the start and target vectors
            Vector3 topDownSpawn = new Vector3(start.x, 0, start.z);
            Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

            // Add in rotation offset from the angleOffset parameter
            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            // Compute the final rotation
            Quaternion rotation = offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn);

            this.gameObject.transform.position = start;
            this.gameObject.transform.rotation = rotation;
            this.gameObject.GetComponent<Rigidbody>().velocity = rotation * (Vector3.forward * (float)speed);
            this.velocity = (float)speed;
        }

        #endregion

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
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                OnDestroyTimeoutImpl(this);
                Destroy(this.gameObject);
            }

            if (transform.position.magnitude > 100f)
            {
                OnDestroyOutOfBoundsImpl(this);
                Destroy(this.gameObject);
            }

            CustomUpdate();
        }

        #region Event Callbacks

        /*
         * Called after object is destroyed due to time limit.
         */
        public ProjectileCallbackDelegate OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;

        public virtual Projectile OnDestroyTimeout(ProjectileCallbackDelegate deleg) {
            this.OnDestroyTimeoutImpl = deleg;
            return this;
        }

        /*
         * Called after object is destroyed due to hitting the arena.
         */
        public ProjectileCallbackDelegate OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;

        public virtual Projectile OnDestroyOutOfBounds(ProjectileCallbackDelegate deleg) {
            this.OnDestroyOutOfBoundsImpl = deleg;
            return this;
        }

        /*
         * Called when the object hits the player
         */
        public ProjectileCallbackDelegate OnDestroyCollisionImpl = CallbackDictionary.NOTHING;

        public virtual Projectile OnDestroyCollision(ProjectileCallbackDelegate deleg) {
            this.OnDestroyCollisionImpl = deleg;
            return this;
        }

        #endregion

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
                    OnDestroyCollisionImpl(this);
                    Destroy(this.gameObject);
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
         * Creates a new basic Projectile with the provided parameters. The default
         * will be a small, medium speed projectile that starts at the entity's position
         * and moves to the right; it will die after 10 seconds.
         */
        public static Projectile Create(Entity entity, Vector3? start=null, Vector3? target=null, float angleOffset=0f, float maxTime=10f, Speed speed=Speed.MEDIUM, Size size=Size.SMALL)
        {
            return Projectile
                .Create(entity)
                .SetStart(start)
                .SetTarget(target)
                .SetAngleOffset(angleOffset)
                .SetMaxTime(maxTime)
                .SetSpeed(speed)
                .SetSize(size);
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
