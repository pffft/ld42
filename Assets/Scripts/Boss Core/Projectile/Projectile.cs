using System.Collections;
using System.Collections.Generic;
using BossCore;
using CombatCore;
using UnityEngine;

using UnityEngine.Profiling;

namespace Projectiles
{
    /*
    * Used for handling events for Projectiles. Currently death events are supported.
    */
    public delegate AI.AISequence ProjectileCallback(ProjectileComponent self);

    public class Projectile
    {
        public Entity Entity { get; set; }

        public virtual ProxyVector3 Start { get; set; } = AI.AISequence.BOSS_POSITION;
        public virtual ProxyVector3 Target { get; set; } = AI.AISequence.DELAYED_PLAYER_POSITION;
        public virtual float AngleOffset { get; set; } = 0f;

        public virtual Speed Speed { get; set; } = Speed.MEDIUM;
        public virtual Size Size { get; set; } = Size.SMALL;

        public virtual float MaxTime { get; set; } = 10f;
        public virtual float Damage { get; set; } = ((float)Size.SMALL + 0.5f) * 2f;

        public virtual Vector3 Velocity { get; set; } = Vector3.forward;

        /*
         * Called after object is destroyed due to time limit.
         */
        public ProjectileCallback OnDestroyTimeout { get; set; } = CallbackDictionary.NOTHING;

        /*
         * Called after object is destroyed due to hitting the arena.
         */
        public ProjectileCallback OnDestroyOutOfBounds { get; set; } = CallbackDictionary.NOTHING;

        /*
         * Called when the object hits the player
         */
        public ProjectileCallback OnDestroyCollision { get; set; } = CallbackDictionary.NOTHING;

        #region Constructors

        public Projectile() : this(BossController.self) { }

        public Projectile(Entity entity)
        {
            Entity = entity;
        }

        #endregion

        /// <summary>
        /// This method is called at the end of every Update() call. When overridden,
        /// this can be used to specify custom movement.
        /// </summary>
        /// <param name="component">The ProjectileComponent of this active GameObject.</param>
        public virtual void CustomUpdate(ProjectileComponent component) { }

        /// <summary>
        /// Provides a custom material. By default, material is chosen based on the size
        /// of the Projectile.
        /// </summary>
        /// <returns>The material to render with.</returns>
        public virtual Material CustomMaterial() { return null; }

        /// <summary>
        /// Clones this Projectile data object. 
        /// 
        /// This is mostly used internally by the ShootX() AISequences. If you try to
        /// use Projectiles without first cloning them, then any time you reuse a reference,
        /// you will be modifying the position of the same one every time. 
        /// </summary>
        /// <returns>The clone.</returns>
        public Projectile Clone()
        {
            return MemberwiseClone() as Projectile;
        }

        /// <summary>
        /// Generates a new GameObject with a ProjectileComponent that references this
        /// data object.
        /// </summary>
        /// <returns>The ProjectileComponent added to the new GameObject.</returns>
        public ProjectileComponent Create()
        {
            Profiler.BeginSample("Projectile.Create");

            Profiler.BeginSample("Projectile.Create GameObject Instantiate");
            // Create new GameObject
            //GameObject newObj = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Projectile"));
            GameObject newObj = ProjectileManager.Checkout();
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
                + " with speed " + Speed
                + ", size " + Size;
        }
    }
}
