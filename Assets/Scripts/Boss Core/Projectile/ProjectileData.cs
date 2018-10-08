using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatCore;

namespace Projectiles
{
    /*
     * Used for handling death events for Projectiles.
     * In the future, other callbacks might be added.
     */
    public delegate void ProjectileCallbackDelegate(Projectile self);

    public class ProjectileData
    {
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

        public ProjectileData OnDestroyTimeout(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyTimeoutImpl = deleg;
            return this;
        }

        /*
         * Called after object is destroyed due to hitting the arena.
         */
        public ProjectileCallbackDelegate OnDestroyOutOfBoundsImpl;

        public ProjectileData OnDestroyOutOfBounds(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyOutOfBoundsImpl = deleg;
            return this;
        }

        /*
         * Called when the object hits the player
         */
        public ProjectileCallbackDelegate OnDestroyCollisionImpl;

        public ProjectileData OnDestroyCollision(ProjectileCallbackDelegate deleg)
        {
            this.OnDestroyCollisionImpl = deleg;
            return this;
        }
        #endregion

        public ProjectileData(Entity entity)
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
        public ProjectileData Start(Vector3? start)
        {
            this.preStart = start;
            return this;
        }

        // Builder method
        public ProjectileData Target(Vector3? target)
        {
            this.preTarget = target;
            return this;
        }

        // Builder method
        public ProjectileData AngleOffset(float offsetDegrees)
        {
            this.angleOffset = offsetDegrees;
            return this;
        }

        // Builder method
        public ProjectileData MaxTime(float seconds)
        {
            this.maxTime = seconds;
            return this;
        }

        // Builder method
        public ProjectileData Damage(float damage)
        {
            this.damage = damage;
            return this;
        }

        // Builder method
        public ProjectileData Speed(BossCore.Speed speed)
        {
            this.speed = speed;
            return this;
        }

        // Builder method
        public ProjectileData Size(Size size)
        {
            this.size = size;
            this.damage = ((float)size + 0.5f) * 2f;
            return this;
        }

        // Clone method
        public ProjectileData CloneWithCallbacks() {
            ProjectileData clone = new ProjectileData(this.entity);

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
        public ProjectileData Clone() {
            ProjectileData clone = CloneWithCallbacks();
            clone.OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;
            clone.OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;
            clone.OnDestroyCollisionImpl = CallbackDictionary.NOTHING;
            return clone;
        }

        // Generates a new GameObject based on this structure.
        public Projectile Create()
        {
            // Create new GameObject
            GameObject newObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            newObj.SetActive(false); // hack- set inactive so we can assign data for use on awake

            // Create a new Projectile component
            Projectile projectile = newObj.AddComponent<Projectile>();
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
