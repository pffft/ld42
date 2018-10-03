using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using BossCore;

using UnityEngine.Profiling;

namespace AOEs
{
    public class AOE : MonoBehaviour
    {

        public delegate void AOECallbackDelegate(AOE self);

        public struct AOEStructure
        {

            // Mostly so we know what side we're on.
            internal Entity entity;

            // internal. Tracks what triangles are on or off in the mesh
            internal bool[] regions;

            // Origin of the attack
            internal Vector3? preStart; // A value assigned at start. Will be resolved later.
            internal Vector3 start;

            // Where the attack is facing (the 0 line is defined by start-target)
            internal Vector3? preTarget; // A value assigned at start. Will be resolved later.
            internal Vector3 target;

            // internal. How much this attack is rotated from the north line.
            internal float internalRotation;

            // How much this attack is rotated from the center line.
            internal float angleOffset;

            // The scale of the inside ring, from 0-1 relative to the outside ring.
            // This value has no effect if "fixedWidth" is set; it will impact the
            // profile of the attack if "innerExpansionSpeed" is set and different
            // from "expansionSpeed". 
            internal float innerScale;

            // Current scale. This is exactly equal to the world unit radius of the attack.
            internal float scale;

            // How fast the inner ring expands
            internal Speed innerExpansionSpeed;

            // How fast the outer ring expands
            internal Speed expansionSpeed;

            // Does nothing if 0. Else, represents how many units there are between
            // the inner and outer ring at all times.
            internal float fixedWidth;

            // internal. Time since the move started
            internal float currentTime;

            // The maximum lifetime of this attack
            internal float maxTime;

            // How much damage this attack does.
            internal float damage;

            // How fast this guy rotates.
            internal float rotationSpeed;

            // Whether or not this AOE is destroyed when it goes out of bounds.
            internal bool shouldDestroyOnOutOfBounds;

            #region callbacks
            internal AOECallbackDelegate OnDestroyOutOfBoundsImpl;
            public AOEStructure OnDestroyOutOfBounds(AOECallbackDelegate deleg)
            {
                this.OnDestroyOutOfBoundsImpl = deleg;
                return this;
            }
            #endregion

            public AOEStructure(Entity self)
            {
                this.entity = self;
                this.regions = new bool[NUM_SECTIONS];
                for (int i = 0; i < regions.Length; i++)
                {
                    regions[i] = false;
                }
                this.preStart = null;
                this.start = Vector3.zero;
                this.preTarget = null;
                this.target = Vector3.zero;
                this.internalRotation = 0f;
                this.angleOffset = 0f;
                this.innerScale = 0.95f;
                this.scale = 1f;
                this.innerExpansionSpeed = BossCore.Speed.MEDIUM;
                this.expansionSpeed = BossCore.Speed.MEDIUM;
                this.fixedWidth = 0f;
                this.currentTime = 0f;
                this.maxTime = 100f;
                this.damage = 5;
                this.rotationSpeed = 0f;

                this.shouldDestroyOnOutOfBounds = true;

                this.OnDestroyOutOfBoundsImpl = AOECallbackDictionary.NOTHING;
            }

            public AOEStructure On(float from, float to)
            {
                if (to < from)
                {
                    return On(to, from);
                }

                if (from < 0 && to > 0)
                {
                    return On(from + 360, 360).On(0, to);
                }

                from = from < 0 ? from + 360 : from;
                to = to < 0 ? to + 360 : to;

                for (int i = 0; i < NUM_SECTIONS; i++)
                {
                    float angle = (i + 0.5f) * THETA_STEP;
                    if (angle >= from && angle <= to)
                    {
                        regions[i] = true;
                    }
                }
                return this;
            }

            public AOEStructure Off(float from, float to)
            {
                if (to < from)
                {
                    return Off(to, from);
                }

                if (from < 0 && to > 0)
                {
                    return Off(from + 360, 360).Off(0, to);
                }

                from = from < 0 ? from + 360 : from;
                to = to < 0 ? to + 360 : to;

                for (int i = 0; i < NUM_SECTIONS; i++)
                {
                    float angle = (i + 0.5f) * THETA_STEP;
                    if (angle >= from && angle <= to)
                    {
                        regions[i] = false;
                    }
                }
                return this;
            }

            public AOEStructure Start(Vector3? start)
            {
                this.preStart = start;
                return this;
            }

            public AOEStructure Target(Vector3? target)
            {
                this.preTarget = target;
                return this;
            }

            public AOEStructure AngleOffset(float degrees)
            {
                this.angleOffset = degrees;
                return this;
            }

            public AOEStructure MaxTime(float time)
            {
                this.maxTime = time;
                return this;
            }

            public AOEStructure Speed(Speed speed)
            {
                this.expansionSpeed = speed;
                return this;
            }

            public AOEStructure Damage(float damage)
            {
                this.damage = damage;
                return this;
            }

            public AOEStructure FixedWidth(float width)
            {
                //this.innerScale = 0f;
                this.innerExpansionSpeed = BossCore.Speed.FROZEN;
                this.fixedWidth = width;
                return this;
            }

            public AOEStructure InnerScale(float scale)
            {
                this.innerScale = scale;
                //this.innerExpansionSpeed = 0f;
                this.fixedWidth = 0f;
                return this;
            }

            public AOEStructure InnerSpeed(Speed speed)
            {
                //this.innerScale = 0f; // initial inner scale gives slightly different effects
                this.innerExpansionSpeed = speed;
                this.fixedWidth = 0f;
                return this;
            }

            public AOEStructure RotationSpeed(float speed)
            {
                this.rotationSpeed = speed;
                return this;
            }

            public AOE Create()
            {
                // Set up the gameobject
                GameObject obj = new GameObject();
                obj.transform.position = entity.transform.position;
                obj.layer = LayerMask.NameToLayer("AOE");
                obj.name = "AOE";
                obj.SetActive(false); // hack so we can assign variables on init

                MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
                meshFilter.mesh = new Mesh();

                MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
                meshRenderer.material = AOE_MATERIAL;

                CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
                collider.center = Vector3.zero;
                collider.radius = 1f;
                collider.isTrigger = true;

                // Add the component with this as its data reference
                // We specifically make a copy, so that we can use this as a template.
                AOE aoe = obj.AddComponent<AOE>();
                aoe.data = new AOEStructure();
                aoe.data = this;

                obj.SetActive(true);
                return aoe;
            }
        }

        // How many sections are in the AOE attack mesh
        public const int NUM_SECTIONS = 360 / 5;

        // The number of degrees subtended by an AOE region.
        private const float THETA_STEP = 360f / NUM_SECTIONS;

        // The height at which we render the AOE, so it doesn't clip the ground.
        private const float HEIGHT = 0.5f;

        // Every AOE has the same material, for now. We cache it here.
        private static readonly Material AOE_MATERIAL;
        static AOE()
        {
            AOE_MATERIAL = new Material(Resources.Load<Material>("Art/Materials/AOE"));
        }

        // A reference containing the data we'll be using.
        public AOEStructure data;

        // Initialize values to the latest ones- start and target, if null, should
        // be set to the live boss/player positions.
        public void Awake()
        {
            data.start = data.preStart ?? data.entity.transform.position;

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
            UpdateOrientation();

            RecomputeMeshHole();

            // We set the scale to 0 so that timing based attacks work properly.
            // Otherwise the scale is 1, because computing the mesh required it.
            data.scale = 0;
            transform.localScale = data.scale * Vector3.one;
        }

        public void Update()
        {
            // Timeout
            data.currentTime += Time.deltaTime;
            if (data.currentTime > data.maxTime)
            {
                //Debug.Log("Time is over!");
                Destroy(this.gameObject);
            }

            // Hit the arena walls
            // should be "innerscale"- what about AOE attacks without hole in center?
            if (data.scale > (GameObject.Find("Arena").transform.localScale.x * 50f) + (data.start.magnitude))
            {
                data.OnDestroyOutOfBoundsImpl(this);
                if (data.shouldDestroyOnOutOfBounds)
                {
                    Destroy(this.gameObject);
                }
                //Debug.Log("Ring hit arena. Returning.");
            }

            // Update the size of the AOE per its expansion rate.
            // We divide by two because AOEs move based on radius, not diameter;
            // this makes the speeds faster than for projectiles without this correction.
            //Debug.Log("Speed: " + data.expansionSpeed);
            data.scale += (float)data.expansionSpeed * Time.deltaTime;
            gameObject.transform.localScale = data.scale * Vector3.one;

            // Rotate it, if needed.
            data.internalRotation += data.rotationSpeed * Time.deltaTime;
            gameObject.transform.rotation = Quaternion.AngleAxis(data.internalRotation, Vector3.up);
            //gameObject.transform.Rotate(Vector3.up, data.rotationSpeed * Time.deltaTime);

            // If the inner expansion speed is set, we must recompute the mesh- except if it's equal 
            // to the outer expansion speed, which is the same as just scaling. Then we don't recompute.
            if (Mathf.Abs((float)data.innerExpansionSpeed) > 0.01f &&
                Mathf.Abs((float)data.expansionSpeed) > 0.01f &&
                !Mathf.Approximately((float)data.expansionSpeed, (float)data.innerExpansionSpeed))
            {
                //Debug.Log("Separate inner AOE update");
                //Debug.Log("Separate inner update");
                float ideal = ((float)data.innerExpansionSpeed / (float)data.expansionSpeed);
                data.innerScale = data.innerScale - ((data.innerScale - ideal) * Time.deltaTime);

                RecomputeMeshHole();
                return;
            }

            // If we have a fixed width to maintain, we must recompute.
            if (Mathf.Abs(data.fixedWidth) > 0.01f && Mathf.Abs(data.scale) > 0.01f)
            {
                //Debug.Log("Fixed width AOE update");
                //Debug.Log("Fixed width update");
                data.innerScale = (data.scale < data.fixedWidth) ? 0f : (data.scale - data.fixedWidth) / data.scale;

                RecomputeMeshHole();
                return;
            }

            //Debug.Log("Normal AOE update");
        }

        public static AOEStructure New(Entity self)
        {
            // Create a new structure, and initialize it with default values.
            AOEStructure structure = new AOEStructure(self);
            return structure;
        }

        // Updates the 0 line for the AOE attack. Called when start or target change.
        private void UpdateOrientation()
        {
            // Remove any height from the start and target vectors
            Vector3 topDownSpawn = new Vector3(data.start.x, 0, data.start.z);
            Vector3 topDownTarget = new Vector3(data.target.x, 0, data.target.z);

            float degrees = Vector3.Angle(Vector3.forward, topDownTarget - topDownSpawn);
            if (topDownTarget.x < topDownSpawn.x)
            {
                degrees = 360 - degrees;
            }
            data.internalRotation = degrees + data.angleOffset;

            // Compute the final rotation
            Quaternion rotation = Quaternion.AngleAxis(degrees + data.angleOffset, Vector3.up);
            gameObject.transform.rotation = rotation;
        }

        /*
         * Called on collision with player. Triggers damage.
         */
        public virtual void OnTriggerStay(Collider other)
        {
            GameObject otherObject = other.gameObject;
            Entity otherEntity = otherObject.GetComponent<Entity>();
            if (otherEntity != null && !otherEntity.IsInvincible() && otherEntity.GetFaction() != data.entity.GetFaction())
            {
                // Position relative to us; not absolute
                Vector3 playerPositionFlat = new Vector3(other.transform.position.x - transform.position.x, 0f, other.transform.position.z - transform.position.z);

                // Inside of the safe circle
                if (playerPositionFlat.magnitude < data.innerScale * data.scale)
                {
                    return;
                }

                // Outside the AOE attack (should be impossible)
                if (playerPositionFlat.magnitude > data.scale)
                {
                    return;
                }

                // Get the section the player is colliding with
                float degrees = Vector3.Angle(Vector3.forward, playerPositionFlat);
                if (playerPositionFlat.x < 0)
                {
                    degrees = 360 - degrees;
                }
                degrees -= data.internalRotation;
                degrees = Mathf.Repeat(degrees, 360f);
                //Debug.Log(degrees);


                int section = (int)(degrees / THETA_STEP);
                //Debug.Log("In section " + section);

                if (data.regions[section])
                {
                    Entity.DamageEntity(otherEntity, data.entity, data.damage);
                }
            }
        }

        // Makes a mesh, possibly with a hole if variable size in the middle.
        private void RecomputeMeshHole()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            List<Vector3> verticesList = new List<Vector3>();
            for (int i = 0; i < NUM_SECTIONS; i++)
            {
                if (!data.regions[i])
                {
                    continue;
                }

                float theta1 = Mathf.Deg2Rad * (90f + (i * THETA_STEP));
                float theta2 = Mathf.Deg2Rad * (90f + ((i + 1) * THETA_STEP));

                verticesList.Add(new Vector3(data.innerScale * Mathf.Cos(theta1), 0f, data.innerScale * Mathf.Sin(theta1)));
                verticesList.Add(new Vector3(Mathf.Cos(theta1), 0f, Mathf.Sin(theta1)));

                verticesList.Add(new Vector3(data.innerScale * Mathf.Cos(theta2), 0f, data.innerScale * Mathf.Sin(theta2)));
                verticesList.Add(new Vector3(Mathf.Cos(theta2), 0f, Mathf.Sin(theta2)));
            }
            Vector3[] vertices = verticesList.ToArray();

            int[] triangles = new int[vertices.Length / 4 * 6];
            for (int i = 0; i < vertices.Length / 4; i++)
            {
                triangles[(6 * i) + 0] = (4 * i) + 0;
                triangles[(6 * i) + 1] = (4 * i) + 2;
                triangles[(6 * i) + 2] = (4 * i) + 1;

                triangles[(6 * i) + 3] = (4 * i) + 2;
                triangles[(6 * i) + 4] = (4 * i) + 3;
                triangles[(6 * i) + 5] = (4 * i) + 1;
            }

            // Unity complains about assigning a new vertices array to a mesh
            if (meshFilter.sharedMesh.vertices.Length != vertices.Length)
            {
                meshFilter.sharedMesh.Clear();
            }
            meshFilter.sharedMesh.vertices = vertices;
            meshFilter.sharedMesh.triangles = triangles;
            meshFilter.sharedMesh.RecalculateNormals();
            transform.position = new Vector3(data.start.x, HEIGHT, data.start.z);
        }
    }
}