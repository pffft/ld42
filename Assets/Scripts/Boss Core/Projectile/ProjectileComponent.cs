using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;

using UnityEngine.Profiling;

namespace Projectiles {
    public class ProjectileComponent : MonoBehaviour
    {
        public Projectile data;

        private Transform trans;
        private MeshRenderer rend;
        private bool shouldUpdate = true;

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
                //orangeMaterial = Resources.Load<Material>("Art/Materials/Orange");
                orangeRedMaterial = Resources.Load<Material>("Art/Materials/OrangeRedTransparent");
            }

            trans = transform;
            rend = GetComponent<MeshRenderer>();
        }

        public void Initialize()
        {
            // Sets start
            //data.start = data.preStart ?? data.entity.transform.position;
            data.start = data.preStart.GetValue();
            Debug.Log("Start: " + data.start);

            // Sets target
            /*
            Vector3 targetPosition;
            if (data.preTarget == null)
            {
                if (data.entity.name.Equals(BossController.BOSS_NAME))
                {
                    targetPosition = BossController.isPlayerLocked ?
                       BossController.playerLockPosition :
                       GameManager.Player.transform.position;
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
            */
            data.target = data.preTarget.GetValue();
            Debug.Log("Target: " + data.target);

            // Sets size (and assigns default material, if none set)
            gameObject.transform.localScale = SizeToScale(data.size) * Vector3.one;

            Material material = data.CustomMaterial();
            if (material != null)
            {
                gameObject.GetComponent<MeshRenderer>().material = material;
            }
            else
            {
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
            }

            // Computes the starting position, rotation, and velocity.

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
            //this.gameObject.GetComponent<Rigidbody>().velocity = rotation * (Vector3.forward * (float)data.speed);
            this.data.velocity = rotation * (Vector3.forward * (float)data.speed);

            shouldUpdate = true;
            rend.enabled = true;
            currentTime = 0;
            maxTime = data.maxTime;
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
                BossController.ExecuteAsync(data.OnDestroyTimeoutImpl(this));
                //Destroy(this.gameObject);
                Cleanup();
            }
            Profiler.EndSample();

            Profiler.BeginSample("Movement");
            trans.position += (Time.deltaTime * data.velocity);
            Profiler.EndSample();

            Profiler.BeginSample("Bounds check");
            if (trans.position.sqrMagnitude > 5625f)
            {
                BossController.ExecuteAsync(data.OnDestroyOutOfBoundsImpl(this));
                Cleanup();
                //gameObject.SetActive(false);
                //shouldUpdate = false;
                //rend.enabled = false;
                //ProjectileManager.cache.Enqueue(gameObject);

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
            //ProjectileManager.cache.Enqueue(gameObject);
            //StartCoroutine(ProjectileManager.ReturnWhenever(gameObject));
            ProjectileManager.Return(gameObject);
            Profiler.EndSample();

            //ProjectileManager.Return(this.gameObject);
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
                if (!otherEntity.IsInvincible() && otherEntity.GetFaction() != data.entity.GetFaction())
                {
                    //Debug.Log("Projectile collided, should apply damage");
                    Entity.DamageEntity(otherEntity, data.entity, data.damage);
                    BossController.ExecuteAsync(data.OnDestroyCollisionImpl(this));
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
