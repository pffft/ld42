using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;

using UnityEngine.Profiling;

namespace Projectiles {
    public class ProjectileComponent : MonoBehaviour
    {
        // The data representing this component's specific appearance and behavior.
        public Projectile data;

        private Vector3 Start;
        private Vector3 Target;

        // Some cached GameObject values for increased performance.
        private Transform trans;
        private MeshRenderer rend;
        public bool shouldUpdate = true;

        // Time is updated in the component rather than the Projectile for increased performance
        public float currentTime;
        private float maxTime;

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

            trans = transform;
            rend = GetComponent<MeshRenderer>();
        }

        public void Initialize()
        {
            // Resolve the proxy variables for start and target
            // This also "locks" them so they don't keep updating.
            Start = data.Start.GetValue();
            Target = data.Target.GetValue();

            // Sets size (and assigns default material, if none set)
            gameObject.transform.localScale = SizeToScale(data.Size) * Vector3.one;

            Material material = data.CustomMaterial();
            if (material != null)
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
            }
            else
            {
                switch (data.Size)
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
            }

            // Computes the starting position, rotation, and velocity.

            // Remove any height from the start and target vectors
            Vector3 topDownSpawn = new Vector3(Start.x, 0, Start.z);
            Vector3 topDownTarget = new Vector3(Target.x, 0, Target.z);

            // Add in rotation offset from the angleOffset parameter
            Quaternion offset = Quaternion.AngleAxis(data.AngleOffset, Vector3.up);

            // Compute the final rotation
            // TODO update this to be rotation around the up axis to fix 180 degree bug
            Quaternion rotation = offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn);

            this.gameObject.transform.position = (Vector3)Start;
            this.gameObject.transform.rotation = rotation;
            //this.gameObject.GetComponent<Rigidbody>().velocity = rotation * (Vector3.forward * (float)data.speed);
            this.data.Velocity = rotation * (Vector3.forward * (float)data.Speed);

            shouldUpdate = true;
            rend.enabled = true;
            currentTime = 0;
            maxTime = data.MaxTime;
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
            if (!shouldUpdate)
            {
                return;
            }
            Profiler.BeginSample("Projectile update loop");
            Profiler.BeginSample("Time check");
            //data.currentTime += Time.deltaTime;
            currentTime += Time.deltaTime;

            if (currentTime >= maxTime)
            {
                GameManager.Boss.ExecuteAsync(data.OnDestroyTimeout(this));
                //Destroy(this.gameObject);
                Cleanup();
            }
            Profiler.EndSample();

            Profiler.BeginSample("Movement");
            if (data.Speed != BossCore.Speed.FROZEN)
            {
                trans.position += (Time.deltaTime * data.Velocity);
                Profiler.EndSample();

                Profiler.BeginSample("Bounds check");
                if (trans.position.sqrMagnitude > 5625f)
                {
                    GameManager.Boss.ExecuteAsync(data.OnDestroyOutOfBounds(this));
                    Cleanup();

                }
            }
            Profiler.EndSample();

            Profiler.BeginSample("Custom update");
            data.CustomUpdate(this);
            Profiler.EndSample();
            Profiler.EndSample();
        }

        private void Cleanup() {
            Profiler.BeginSample("Cleanup");
            shouldUpdate = false;
            rend.enabled = false;
            ProjectileManager.Return(gameObject);
            Profiler.EndSample();
        }

        /*
         * Called on collision with player. Triggers collison death.
         */
        public virtual void OnTriggerEnter(Collider other)
        {
            Profiler.BeginSample("Projectile Collision");
            GameObject otherObject = other.gameObject;
            Entity otherEntity = otherObject.GetComponentInParent<Entity>();
            if (otherEntity != null)
            {
                // All projectiles break if they do damage
                if (!otherEntity.IsInvincible() && otherEntity.GetFaction() != Entity.Faction.enemy)
                {
                    //Debug.Log("Projectile collided, should apply damage");
                    // Note that the entity causing the damage is null; callbacks may fail.
                    Entity.DamageEntity(otherEntity, null, data.Damage);
                    GameManager.Boss.ExecuteAsync(data.OnDestroyCollision(this));
                    //Destroy(this.gameObject);
                    Cleanup();
                }
            }
            Profiler.EndSample();
        }

        /*
         * Get the preferred material for this projectile.
         * The standard only sets material based on size; if you want your projectile
         * to have its own material, override this method and return it here.
         *
         * If you want to use the standard projectile logic, simply return null here.
         */
        public virtual Material GetCustomMaterial() { return null; }
    }

}
