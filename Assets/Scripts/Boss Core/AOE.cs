using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using UnityEngine.Profiling;

public class AOE : MonoBehaviour {

    //private static float oldInnerRadius = 5f;
    //private static float oldRadius = 10f;

    //public static float innerRadius = 5f;
    //public static float radius = 10f;

    // This value might be tweaked based on how smooth the AOE attacks need to look.
    // How many sections are in the AOE attack mesh
    public const int NUM_SECTIONS = 360 / 5;

    // The number of degrees subtended by an AOE region.
    private const float THETA_STEP = 360f / NUM_SECTIONS;

    // The height at which we render the AOE, so it doesn't clip the ground.
    private const float HEIGHT = 0.5f;

    // internal tracker of what triangles are on or off
    private bool[] regions;

    private float innerScale = 1f;

    // internal tracker of current scale
    private float scale = 1f;

    private float innerExpansionSpeed = 5f;
    private float expansionSpeed = 20f;

    private float fixedWidth = 0;

    private Entity entity;
    private float damage = 5f;

    private float currentTime = 0f;
    private float maxTime = 100f;

    private float angleOffset = 0f;

    private static readonly Material AOE_MATERIAL;
    static AOE() {
        AOE_MATERIAL = new Material(Resources.Load<Material>("Art/Materials/AOE"));
    }

    public void Update() {
        // Timeout
        currentTime += Time.deltaTime;
        if (currentTime > maxTime) {
            Destroy(this.gameObject);
        }

        // Hit the arena walls
        // should be "innerscale"- what about AOE attacks without hole in center?
        if (scale > GameObject.Find("Arena").transform.localScale.x * 50f)
        {
            //Debug.Log("Ring hit arena. Returning.");
            //Destroy(this.gameObject);
        }

        // Update the size of the AOE per its expansion rate.
        scale += expansionSpeed * Time.deltaTime;
        gameObject.transform.localScale = scale * Vector3.one;

        // If the inner expansion speed is set, we must recompute the mesh- except if it's equal 
        // to the outer expansion speed, which is the same as just scaling. Then we don't recompute.
        if (Mathf.Abs(innerExpansionSpeed) > 0.01f && !Mathf.Approximately(expansionSpeed, innerExpansionSpeed))
        {
            //Debug.Log("Separate inner update");
            float ideal = (innerExpansionSpeed / expansionSpeed);
            innerScale = innerScale - ((innerScale - ideal) * Time.deltaTime);
            //Debug.Log(innerScale);

            RecomputeMeshHole();
            return;
        }

        // If we have a fixed width to maintain, we must recompute.
        if (Mathf.Abs(fixedWidth) > 0.01f)
        {
            //Debug.Log("Fixed width update");
            innerScale = (scale < fixedWidth) ? 0f : (scale - fixedWidth) / scale;

            RecomputeMeshHole();
            return;
        }
    }

    public static AOE Create(Entity self)
    {
        GameObject obj = new GameObject();
        obj.layer = LayerMask.NameToLayer("AOE");
        obj.name = "AOE";

        MeshFilter meshFilter = obj.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        MeshRenderer meshRenderer = obj.AddComponent<MeshRenderer>();
        meshRenderer.material = AOE_MATERIAL;

        CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
        collider.center = obj.transform.position;
        collider.radius = 1f;
        collider.isTrigger = true;

        AOE aoe = obj.AddComponent<AOE>();
        aoe.entity = self;
        aoe.regions = new bool[NUM_SECTIONS];
        for (int i = 0; i < aoe.regions.Length; i++) {
            aoe.regions[i] = true;
        }
        //aoe.Off(0, 120).Off(140, 150).Off(180, 270).On(200, 220);
        aoe.Off(0, 60).Off(120, 180).Off(240, 300);

        aoe.RecomputeMeshHole();


        // We set the scale to 0 so that timing based attacks work properly.
        // Otherwise the scale is 1 by default.
        aoe.scale = 0f;
        obj.transform.localScale = aoe.scale * Vector3.one;
        return aoe;

        // Set origin pos
        // Set on(from, to) and off(from, to)
        // Set angleOffset 
        // Set max size/ max time
        // Set expansion speed
        // Thickness- as % or as fixed distance
        // rotation?
    }

    public AOE On(float from, float to) {
        for (int i = 0; i < NUM_SECTIONS; i++) 
        {
            float angle = (i + 0.5f) * THETA_STEP;
            if (angle >= from && angle <= to) 
            {
                regions[i] = true;
            }
        }
        RecomputeMeshHole();
        return this;
    }

    public AOE Off(float from, float to)
    {
        for (int i = 0; i < NUM_SECTIONS; i++)
        {
            float angle = (i + 0.5f) * THETA_STEP;
            if (angle >= from && angle <= to)
            {
                regions[i] = false;
            }
        }
        RecomputeMeshHole();
        return this;
    }

    public AOE SetAngleOffset(float degrees) {
        this.angleOffset = degrees;
        this.gameObject.transform.rotation = Quaternion.AngleAxis(degrees, Vector3.up);
        return this;
    }

    public AOE SetFixedWidth(float width) {
        this.innerScale = 0f;
        this.innerExpansionSpeed = 0f;
        this.fixedWidth = width;
        RecomputeMeshHole();
        return this;
    }

    public AOE SetInnerScale(float scale) {
        this.innerScale = scale;
        //this.innerExpansionSpeed = 0f;
        this.fixedWidth = 0f;
        RecomputeMeshHole();
        return this;
    }

    public AOE SetInnerSpeed(float speed) {
        //this.innerScale = 0f; // initial inner scale gives slightly different effects
        this.innerExpansionSpeed = speed;
        this.fixedWidth = 0f;
        RecomputeMeshHole();
        return this;
    }

    /*
     * Called on collision with player. Triggers damage.
     */
    public virtual void OnTriggerStay(Collider other)
    {
        GameObject otherObject = other.gameObject;
        Entity otherEntity = otherObject.GetComponent<Entity>();
        if (otherEntity != null && !otherEntity.IsInvincible() && otherEntity.GetFaction() != this.entity.GetFaction())
        {
            Vector3 playerPositionFlat = new Vector3(other.transform.position.x, 0f, other.transform.position.z);

            // Inside of the safe circle
            if (playerPositionFlat.magnitude < innerScale * scale)
            {
                return;
            }

            // Outside the AOE attack (should be impossible)
            if (playerPositionFlat.magnitude > scale)
            {
                return;
            }

            // Get the section the player is colliding with
            float degrees = Vector3.Angle(Vector3.forward, playerPositionFlat) + angleOffset;
            if (playerPositionFlat.x < 0) {
                degrees = 360 - degrees;
            }

            int section = (int)(degrees / THETA_STEP);

            if (regions[section]) {
                Entity.DamageEntity(otherEntity, this.entity, damage);
            }
        }
    }

    // Makes a mesh, possibly with a hole in the middle of variable distance.
    private void RecomputeMeshHole() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i < NUM_SECTIONS; i++)
        {
            if (!regions[i]) {
                continue;
            }

            float theta1 = Mathf.Deg2Rad * i * THETA_STEP;
            float theta2 = Mathf.Deg2Rad * (i + 1) * THETA_STEP;

            verticesList.Add(new Vector3(innerScale * Mathf.Cos(theta1), 0f, innerScale * Mathf.Sin(theta1)));
            verticesList.Add(new Vector3(Mathf.Cos(theta1), 0f, Mathf.Sin(theta1)));

            verticesList.Add(new Vector3(innerScale * Mathf.Cos(theta2), 0f, innerScale * Mathf.Sin(theta2)));
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
        transform.position = new Vector3(0f, HEIGHT, 0f);
    }
}
