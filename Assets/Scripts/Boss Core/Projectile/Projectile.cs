﻿using System.Collections;
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

        public float currentTime;
        public float maxTime;

        public float damage;

        public int cacheIndex;

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

        /*
         * Can be overridden to provide custom behavior for components
         */
        public virtual void CustomUpdate(ProjectileComponent component) { }

        /*
         * Can be overridden to provide a custom material.
         */
        public virtual Material CustomMaterial() { return null; }

        #endregion

        /*
        public static Projectile New(Entity entity) {
            return new Projectile(entity);
        }
        */

        public Projectile() : this(BossController.self) { }

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

            this.currentTime = 0f;
            this.maxTime = 10f;

            this.damage = ((float)size + 0.5f) * 2f;

            OnDestroyTimeoutImpl = CallbackDictionary.NOTHING;
            OnDestroyOutOfBoundsImpl = CallbackDictionary.NOTHING;
            OnDestroyCollisionImpl = CallbackDictionary.NOTHING;
        }

        // Builder method
        public virtual Projectile Start(Vector3? start)
        {
            this.preStart = start;
            return this;
        }

        // Builder method
        public virtual Projectile Target(Vector3? target)
        {
            this.preTarget = target;
            return this;
        }

        // Builder method
        public virtual Projectile AngleOffset(float offsetDegrees)
        {
            this.angleOffset = offsetDegrees;
            return this;
        }

        // Builder method
        public virtual Projectile MaxTime(float seconds)
        {
            this.maxTime = seconds;
            return this;
        }

        // Builder method
        public virtual Projectile Damage(float damage)
        {
            this.damage = damage;
            return this;
        }

        // Builder method
        public virtual Projectile Speed(BossCore.Speed speed)
        {
            this.speed = speed;
            return this;
        }

        // Builder method
        public virtual Projectile Size(Size size)
        {
            this.size = size;
            this.damage = ((float)size + 0.5f) * 2f;
            return this;
        }

        // Clone method
        public Projectile Clone()
        {
            return MemberwiseClone() as Projectile;
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
            Profiler.BeginSample("Projectile.Create");

            Profiler.BeginSample("Projectile.Create GameObject Instantiate");
            // Create new GameObject
            //GameObject newObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            cacheIndex = ProjectileManager.NextSlot();
            GameObject newObj = ProjectileManager.Checkout(cacheIndex);
            Profiler.EndSample();

            Profiler.BeginSample("Projectile.Create Component Instantiate");
            // Create a new Projectile component
            ProjectileComponent projectile = newObj.GetComponent<ProjectileComponent>();
            Profiler.EndSample();

            Profiler.BeginSample("Projectile.Create data Clone()");
            // Make a memberwise clone of the most derived type
            projectile.data = Clone();
            Profiler.EndSample();

            Profiler.BeginSample("Projectile.Create data Initialize()");
            // Do the initialization (resolve null variables -> live variables)
            projectile.Initialize();
            Profiler.EndSample();

            Profiler.BeginSample("Projectile.Create data CustomCreate()");
            // Do any custom derived initialization logic (you can access the component now)
            projectile.data.CustomCreate(projectile);
            Profiler.EndSample();

            // Assign and init the RigidBody (or create one if it doesn't exist)
            Rigidbody body = newObj.GetComponent<Rigidbody>();
            if (body == null)
            {
                body = newObj.AddComponent<Rigidbody>();
            }
            body.useGravity = false;


            Profiler.EndSample();
            return projectile;
        }

        /*
         * Allows for custom instantiation once the component is created and can
         * be referenced. Things like accessing the RigidBody are done here, as well
         * as being able to reference live variables, like the updated target value
         * (via component.data.target).
         */
        public virtual void CustomCreate(ProjectileComponent component) { }

        public override string ToString()
        {
            // TODO can make this more descriptive; i.e. if entity is boss and preTarget is null, then add "aimed at the player".
            return "Projectile"
                + " with speed " + speed
                + ", size " + size;
                //+ ((type != Type.BASIC) ? ", and type " + type.ToString() : "");
        }
    }
}
