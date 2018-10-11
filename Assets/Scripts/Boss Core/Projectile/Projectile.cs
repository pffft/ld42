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
    public delegate void ProjectileCallbackDelegate(ProjectileComponent self);

    public class Projectile
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
            GameObject newObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));

            // Create a new Projectile component
            ProjectileComponent projectile = newObj.GetComponent<ProjectileComponent>();
            //Debug.Log("Is projectile null?: " + (projectile != null));
            projectile.data = Clone();
            projectile.Initialize();
            //Debug.Log("Is projectile data null?: " + (projectile.data != null));

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

            return projectile;
        }
    }
}
