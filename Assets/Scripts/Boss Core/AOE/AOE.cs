using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;

namespace AOEs
{
    public class AOE : MonoBehaviour
    {
        // A reference containing the data we'll be using.
        public AOEData data;

        private Vector3 Start;
        private Vector3 Target;

        // Initialize values to the latest ones- start and target, if null, should
        // be set to the live boss/player positions.
        public void Awake()
        {
            Start = data.Start.GetValue();
            Target = data.Target.GetValue();

            // Remove any height from the start and target vectors
            Vector3 topDownSpawn = new Vector3(Start.x, 0, Start.z);
            Vector3 topDownTarget = new Vector3(Target.x, 0, Target.z);

            float degrees = Vector3.Angle(Vector3.forward, topDownTarget - topDownSpawn);
            if (topDownTarget.x < topDownSpawn.x)
            {
                degrees = 360 - degrees;
            }
            data.InternalRotation = degrees + data.AngleOffset;

            // Compute the final rotation
            Quaternion rotation = Quaternion.AngleAxis(degrees + data.AngleOffset, Vector3.up);
            gameObject.transform.rotation = rotation;
            RecomputeMeshHole();

            // We set the scale to 0 so that timing based attacks work properly.
            // Otherwise the scale is 1, because computing the mesh required it.
            // If the scale is not 1, then it's most likely a clone- so we keep the scale as-is.
            if (Mathf.Approximately(data.Scale, 1f))
            {
                data.Scale = 0;
                transform.localScale = data.Scale * Vector3.one;
            }
        }


        public void Update()
        {
            // Timeout
            data.CurrentTime += Time.deltaTime;
            if (data.CurrentTime > data.MaxTime)
            {
                data.OnDestroyTimeout(this);
                Destroy(this.gameObject);
            }

            // Hit the arena walls
            // should be "innerscale"- what about AOE attacks without hole in center?
            if (data.Scale > GameManager.Arena.RadiusInWorldUnits + Start.magnitude)
            {
                data.OnDestroyOutOfBounds(this);
                if (data.shouldDestroyOnOutOfBounds)
                {
                    Destroy(this.gameObject);
                }
            }

            data.Scale += (float)data.OuterSpeed * Time.deltaTime;
            gameObject.transform.localScale = data.Scale * Vector3.one;

            // Rotate it, if needed.
            data.InternalRotation += data.RotationSpeed * Time.deltaTime;
            gameObject.transform.rotation = Quaternion.AngleAxis(data.InternalRotation, Vector3.up);
            //gameObject.transform.Rotate(Vector3.up, data.rotationSpeed * Time.deltaTime);

            // If the inner expansion speed is set, we must recompute the mesh- except if it's equal 
            // to the outer expansion speed, which is the same as just scaling. Then we don't recompute.
            if (Mathf.Abs((float)data.InnerSpeed) > 0.01f &&
                Mathf.Abs((float)data.OuterSpeed) > 0.01f &&
                !Mathf.Approximately((float)data.OuterSpeed, (float)data.InnerSpeed))
            {
                float ideal = ((float)data.InnerSpeed / (float)data.OuterSpeed);
                data.InnerScale = data.InnerScale - ((data.InnerScale - ideal) * Time.deltaTime);

                RecomputeMeshHole();
                return;
            }

            // If we have a fixed width to maintain, we must recompute.
            if (Mathf.Abs(data.FixedWidth) > 0.01f && Mathf.Abs(data.Scale) > 0.01f)
            {
                data.InnerScale = (data.Scale < data.FixedWidth) ? 0f : (data.Scale - data.FixedWidth) / data.Scale;

                RecomputeMeshHole();
                return;
            }
        }

        /*
         * Called on collision with player. Triggers damage.
         */
        public virtual void OnTriggerStay(Collider other)
        {
            GameObject otherObject = other.gameObject;
            Entity otherEntity = otherObject.GetComponent<Entity>();
            if (otherEntity != null && !otherEntity.IsInvincible() && otherEntity.GetFaction() != Entity.Faction.enemy)
            {
                // Position relative to us; not absolute
                Vector3 playerPositionFlat = new Vector3(other.transform.position.x - transform.position.x, 0f, other.transform.position.z - transform.position.z);

                // Inside of the safe circle
                if (playerPositionFlat.magnitude < data.InnerScale * data.Scale)
                {
                    return;
                }

                // Outside the AOE attack (should be impossible)
                if (playerPositionFlat.magnitude > data.Scale)
                {
                    return;
                }

                // Get the section the player is colliding with
                float degrees = Vector3.Angle(Vector3.forward, playerPositionFlat);
                if (playerPositionFlat.x < 0)
                {
                    degrees = 360 - degrees;
                }
                degrees -= data.InternalRotation;
                degrees = Mathf.Repeat(degrees, 360f);

                int section = (int)(degrees / AOEData.THETA_STEP);

                if (data.Regions[section])
                {
                    // Note: we pass in a null entity; callbacks might break
                    Entity.DamageEntity(otherEntity, null, data.Damage);
                }
            }
        }

        // Makes a mesh, possibly with a hole if variable size in the middle.
        private void RecomputeMeshHole()
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();

            List<Vector3> verticesList = new List<Vector3>();
            for (int i = 0; i < AOEData.NUM_SECTIONS; i++)
            {
                if (!data.Regions[i])
                {
                    continue;
                }

                float theta1 = Mathf.Deg2Rad * (90f + (i * AOEData.THETA_STEP));
                float theta2 = Mathf.Deg2Rad * (90f + ((i + 1) * AOEData.THETA_STEP));

                verticesList.Add(new Vector3(data.InnerScale * Mathf.Cos(theta1), 0f, data.InnerScale * Mathf.Sin(theta1)));
                verticesList.Add(new Vector3(Mathf.Cos(theta1), 0f, Mathf.Sin(theta1)));

                verticesList.Add(new Vector3(data.InnerScale * Mathf.Cos(theta2), 0f, data.InnerScale * Mathf.Sin(theta2)));
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
            transform.position = new Vector3(Start.x, AOEData.HEIGHT, Start.z);
        }
    }
}