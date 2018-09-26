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
        public struct ProjectileStructure {
            public Entity entity;

            // Used internally for the builder notation
            public Vector3? preStart;
            public Vector3 start;
            public Vector3? preTarget;
            public Vector3 target;
            public float angleOffset;

            public BossCore.Speed speed;
            public Size size;
            public Type type;
            public object[] _typeParameters;

            public float currentTime;
            public float maxTime;

            public float damage;

            #region callbacks
            /*
             * Called after object is destroyed due to time limit.
             */
            public ProjectileCallbackDelegate OnDestroyTimeoutImpl;

            public ProjectileStructure OnDestroyTimeout(ProjectileCallbackDelegate deleg)
            {
                this.OnDestroyTimeoutImpl = deleg;
                return this;
            }

            /*
             * Called after object is destroyed due to hitting the arena.
             */
            public ProjectileCallbackDelegate OnDestroyOutOfBoundsImpl;

            public ProjectileStructure OnDestroyOutOfBounds(ProjectileCallbackDelegate deleg)
            {
                this.OnDestroyOutOfBoundsImpl = deleg;
                return this;
            }

            /*
             * Called when the object hits the player
             */
            public ProjectileCallbackDelegate OnDestroyCollisionImpl;

            public ProjectileStructure OnDestroyCollision(ProjectileCallbackDelegate deleg)
            {
                this.OnDestroyCollisionImpl = deleg;
                return this;
            }
            #endregion

            public ProjectileStructure(Entity entity) {
                this.entity = entity;

                this.preStart = null;
                this.start = Vector3.zero;
                this.preTarget = null;
                this.target = Vector3.zero;
                this.angleOffset = 0f;

                this.speed = BossCore.Speed.MEDIUM;
                this.size = Projectiles.Size.SMALL;
                this.type = Type.BASIC;
                this._typeParameters = null;

                this.currentTime = 0f;
                this.maxTime = 10f;

                this.damage = 5f;

                OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;
                OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;
                OnDestroyCollisionImpl = CallbackDictionary.NOTHING;
            }

            // Builder method
            public ProjectileStructure Start(Vector3? start)
            {
                this.preStart = start;
                return this;
            }

            // Builder method
            public ProjectileStructure Target(Vector3? target)
            {
                this.preTarget = target;
                return this;
            }

            // Builder method
            public ProjectileStructure AngleOffset(float offsetDegrees)
            {
                this.angleOffset = offsetDegrees;
                return this;
            }

            // Builder method
            public ProjectileStructure MaxTime(float seconds)
            {
                this.maxTime = seconds;
                return this;
            }

            // Builder method
            public ProjectileStructure Speed(BossCore.Speed speed)
            {
                this.speed = speed;
                return this;
            }

            // Builder method
            public ProjectileStructure Size(Size size)
            {
                this.size = size;
                return this;
            }

            // Generates a new GameObject based on this structure.
            public Projectile Create()
            {
                // Create new GameObject
                GameObject newObj = Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/Projectile"));
                newObj.SetActive(false); // hack- set inactive so we can assign data for use on awake

                // Create a new Projectile component
                Projectile projectile = newObj.AddComponent<Projectile>();
                projectile.data = this;

                // Assign and init the RigidBody (or create one if it doesn't exist)
                Rigidbody body = newObj.GetComponent<Rigidbody>();
                if (body == null)
                {
                    body = newObj.AddComponent<Rigidbody>();
                }
                body.useGravity = false;

                // Assign the type with any parameters that were forced in.
                switch(type) {
                    case Type.BASIC: break;
                    case Type.CURVING: projectile.Curving((float)_typeParameters[0], (bool)_typeParameters[1]); break;
                    case Type.DEATHHEX: projectile.DeathHex(); break;
                    case Type.HOMING: projectile.Homing(); break;
                    case Type.INDESTRUCTIBLE: break;
                }

                newObj.SetActive(true);
                return projectile;
            }

        }

        public ProjectileStructure data;

        /*
         * Creates a default Projectile component for the builder notation.
         * By default, this is a small, medium speed projectile with a max life
         * of 10. It will start at the entity's position and aim at the player
         * (if the entity is the boss), or else aim forward (for the player).
         */
        public static ProjectileStructure New(Entity entity) {
            return new ProjectileStructure(entity);
        }

        public void Awake()
        {
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

            UpdateOrientationAndVelocity();

            CustomAwake();
        }

        public virtual void CustomAwake() { }


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

            CustomUpdate();
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
            Entity otherEntity = otherObject.GetComponent<Entity>();
            if (otherEntity != null)
            {
                if (!otherEntity.IsInvincible() && otherEntity.GetFaction() != data.entity.GetFaction())
                {
                    Debug.Log("Projectile collided, should apply damage");
                    Entity.DamageEntity(otherEntity, data.entity, data.damage);
                    data.OnDestroyCollisionImpl(this);
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
            other.data = this.data;
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
