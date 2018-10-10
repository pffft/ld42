using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    /*
* Used for handling death events for Projectiles.
* In the future, other callbacks might be added.
*/
    public delegate void ProjectileCallbackDelegate(Projectile.ProjectileComponent self);

    public class Projectile
    {
        public class ProjectileComponent : MonoBehaviour
        {
            public Projectile data;

            public static Material blueMaterial;
            public static Material orangeMaterial;
            public static Material orangeRedMaterial;

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
                    case Projectiles.Size.TINY:
                    case Projectiles.Size.SMALL:
                        gameObject.GetComponent<MeshRenderer>().material = blueMaterial;
                        break;
                    case Projectiles.Size.MEDIUM:
                        gameObject.GetComponent<MeshRenderer>().material = orangeMaterial;
                        break;
                    case Projectiles.Size.LARGE:
                    case Projectiles.Size.HUGE:
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
                    if (data.entity.GetFaction() == Entity.Faction.player)
                    {
                        if (otherEntity.IsInvincible())
                        {
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
            public T CastTo<T>() where T : ProjectileComponent
            {
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

        public Projectile OnDestroyTimeout(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyTimeoutImpl = deleg;
            return this;
        }

        /*
         * Called after object is destroyed due to hitting the arena.
         */
        public ProjectileCallbackDelegate OnDestroyOutOfBoundsImpl;

        public Projectile OnDestroyOutOfBounds(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyOutOfBoundsImpl = deleg;
            return this;
        }

        /*
         * Called when the object hits the player
         */
        public ProjectileCallbackDelegate OnDestroyCollisionImpl;

        public Projectile OnDestroyCollision(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyCollisionImpl = deleg;
            return this;
        }
        #endregion

        public static Projectile New(Entity entity) {
            return new Projectile(entity);
        }

        public Projectile(Entity entity)
        {
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

            this.damage = ((float)size + 0.5f) * 2f;

            OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;
            OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;
            OnDestroyCollisionImpl = CallbackDictionary.NOTHING;
        }

        // Builder method
        public Projectile Start(Vector3? start)
        {
            this.preStart = start;
            return this;
        }

        // Builder method
        public Projectile Target(Vector3? target)
        {
            this.preTarget = target;
            return this;
        }

        // Builder method
        public Projectile AngleOffset(float offsetDegrees)
        {
            this.angleOffset = offsetDegrees;
            return this;
        }

        // Builder method
        public Projectile MaxTime(float seconds)
        {
            this.maxTime = seconds;
            return this;
        }

        // Builder method
        public Projectile Damage(float damage)
        {
            this.damage = damage;
            return this;
        }

        // Builder method
        public Projectile Speed(BossCore.Speed speed)
        {
            this.speed = speed;
            return this;
        }

        // Builder method
        public Projectile Size(Size size)
        {
            this.size = size;
            this.damage = ((float)size + 0.5f) * 2f;
            return this;
        }

        // Clone method
        public Projectile Clone()
        {
            Projectile clone = new Projectile(this.entity);

            clone.preStart = preStart;
            clone.start = start;
            clone.preTarget = preTarget;
            clone.target = target;
            clone.angleOffset = angleOffset;

            clone.speed = speed;
            clone.size = size;
            clone.type = type;
            clone._typeParameters = _typeParameters;

            clone.currentTime = 0f;
            clone.maxTime = maxTime;

            clone.damage = damage;

            clone.OnDestroyTimeoutImpl = OnDestroyTimeoutImpl;
            clone.OnDestroyOutOfBoundsImpl = OnDestroyOutOfBoundsImpl;
            clone.OnDestroyCollisionImpl = OnDestroyCollisionImpl;
            return clone;
        }

        // Clone method - sets the callbacks to do nothing. This prevents recursive behavior.
        public Projectile CloneWithoutCallbacks()
        {
            Projectile clone = Clone();
            clone.OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;
            clone.OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;
            clone.OnDestroyCollisionImpl = CallbackDictionary.NOTHING;
            return clone;
        }

        // Generates a new GameObject based on this structure.
        public ProjectileComponent Create()
        {
            // Create new GameObject
            GameObject newObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            newObj.SetActive(false); // hack- set inactive so we can assign data for use on awake

            // Create a new Projectile component
            ProjectileComponent projectile = newObj.AddComponent<ProjectileComponent>();
            projectile.data = Clone();

            // Assign and init the RigidBody (or create one if it doesn't exist)
            Rigidbody body = newObj.GetComponent<Rigidbody>();
            if (body == null)
            {
                body = newObj.AddComponent<Rigidbody>();
            }
            body.useGravity = false;

            // Assign the type with any parameters that were forced in.
            switch (type)
            {
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
}
