using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : MonoBehaviour
    {
        public Entity entity;

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
                    Destroy(this.gameObject);
                    OnDestroyCollision();
                }
            }
        }

        /*
         * Called upon creation with any additional parameters this object needs.
         */
        public virtual void Initialize(params object[] args) { }

        /*
         * Creates a new Projectile of the specified speed/size/type.
         */
        private static Projectile CreateGeneric(Entity entity, Vector3 start,
            Quaternion startRotation, float maxTime, Speed speed, Size size, Type type, params object[] args)
        {
            
            GameObject newObj;
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

            newObj.transform.position = start;
            newObj.transform.rotation = startRotation;

            Projectile projectile;
            switch (type)
            {
                case Type.BASIC:
                    goto default;
                case Type.INDESTRUCTIBLE:
                    goto default;
                case Type.HOMING:
                    projectile = newObj.AddComponent<ProjectileHoming>();
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/PurpleTransparent");
                    break;
                case Type.CURVING:
                    projectile = newObj.AddComponent<ProjectileCurving>();
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/GreenTransparent");
                    break;
                case Type.DEATHHEX:
                    projectile = newObj.AddComponent<ProjectileDeathHex>();
                    newObj.GetComponent<MeshRenderer>().material = Resources.Load<Material>("Art/Materials/GreenTransparent");
                    break;
                default:
                    projectile = newObj.AddComponent<Projectile>();
                    break;
            }

            Rigidbody body = newObj.GetComponent<Rigidbody>();
            if (body == null)
            {
                body = newObj.AddComponent<Rigidbody>();
            }
            body.velocity = startRotation * (Vector3.forward * (float)speed);
            body.useGravity = false;

            projectile.entity = entity;
            projectile.currentTime = 0;
            projectile.maxTime = maxTime;
            projectile.velocity = (float)speed;
            projectile.damage = 5f;
            projectile.Initialize(args); // Subclasses' init goes here

            return projectile;
        }

        public static void spawnBasic(Entity entity, Vector3 spawn, Vector3 target, float maxTime = 10f, float angleOffset = 0,
                                Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM)
        {

            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            Vector3 topDownSpawn = new Vector3(spawn.x, 0, spawn.z);
            Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

            Projectile.CreateGeneric(entity, spawn,
                                     offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn),
                                     maxTime, speed, size, Type.BASIC);
        }

        public static void spawnHoming(Entity entity, Vector3 spawn, Vector3 target, float maxTime = 10f, float angleOffset = 0,
                                Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM)
        {

            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            Vector3 topDownSpawn = new Vector3(spawn.x, 0, spawn.z);
            Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

            Projectile.CreateGeneric(entity, spawn,
                                     offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn),
                                     maxTime, speed, size, Type.HOMING);
        }

        public static void spawnCurving(Entity entity, Vector3 spawn, Vector3 target, float curveSpeed, float maxTime = 10f, float angleOffset = 0,
                                Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM, bool leavesTrail = true)
        {
            
            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            Vector3 topDownSpawn = new Vector3(spawn.x, 0, spawn.z);
            Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

            Projectile.CreateGeneric(entity, spawn,
                                     offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn),
                                     maxTime, speed, size, Type.CURVING, curveSpeed, leavesTrail);
        }

        public static void spawnDeathHex(Entity entity, Vector3 spawn, Vector3 target, float maxTime = 1f, float angleOffset = 0,
                                         Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM)
        {
            Quaternion offset = Quaternion.AngleAxis(angleOffset, Vector3.up);

            Vector3 topDownSpawn = new Vector3(spawn.x, 0, spawn.z);
            Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

            Projectile.CreateGeneric(entity, spawn,
                                     offset * Quaternion.FromToRotation(Vector3.forward, topDownTarget - topDownSpawn),
                                     maxTime, speed, size, Type.DEATHHEX);
        }
    }
}
