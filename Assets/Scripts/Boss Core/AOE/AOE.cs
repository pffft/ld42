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
        // How many sections are in the AOE attack mesh
        public const int NUM_SECTIONS = 360 / 5;

        // The number of degrees subtended by an AOE region.
        public const float THETA_STEP = 360f / NUM_SECTIONS;

        // The height at which we render the AOE, so it doesn't clip the ground.
        private const float HEIGHT = 0.5f;

        // Every AOE has the same material, for now. We cache it here.
        public static Material AOE_MATERIAL;

        // A reference containing the data we'll be using.
        public AOEData data;

        public static AOEData New(Entity self)
        {
            // Create a new structure, and initialize it with default values.
            AOEData structure = new AOEData(self);
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

        // Initialize values to the latest ones- start and target, if null, should
        // be set to the live boss/player positions.
        public void Awake()
        {
            if (AOE_MATERIAL == null) {
                AOE_MATERIAL = new Material(Resources.Load<Material>("Art/Materials/AOE"));
            }

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
            // If the scale is not 1, then it's most likely a clone- so we keep the scale as-is.
            if (Mathf.Approximately(data.scale, 1f))
            {
                data.scale = 0;
                transform.localScale = data.scale * Vector3.one;
            }
        }

        public void Update()
        {
            // Timeout
            data.currentTime += Time.deltaTime;
            if (data.currentTime > data.maxTime)
            {
                Debug.Log("Time is over! " + data.currentTime + " max: " + data.maxTime);
                data.OnDestroyTimeoutImpl(this);
                Destroy(this.gameObject);
            }

            // Hit the arena walls
            // should be "innerscale"- what about AOE attacks without hole in center?
            if (data.scale > (GameObject.Find("Arena").transform.localScale.x * 50f) + (data.start.magnitude))
            {
                Debug.Log("Ring hit arena. Returning.");
                data.OnDestroyOutOfBoundsImpl(this);
                if (data.shouldDestroyOnOutOfBounds)
                {
                    Destroy(this.gameObject);
                }
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