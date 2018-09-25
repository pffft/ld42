using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using UnityEngine.Profiling;

public class AOE : MonoBehaviour {

    // How many sections are in the AOE attack mesh
    public const int NUM_SECTIONS = 360 / 5;

    // The number of degrees subtended by an AOE region.
    private const float THETA_STEP = 360f / NUM_SECTIONS;

    // The height at which we render the AOE, so it doesn't clip the ground.
    private const float HEIGHT = 0.5f;

    // How much damage this attack does (TODO make this a parameter; same for Projectiles)
    private const float damage = 5f;

    // Mostly so we know what side we're on.
    private Entity entity;

    // internal. Tracks what triangles are on or off in the mesh
    private bool[] regions;

    // Origin of the attack
    private Vector3 start;

    // Where the attack is facing (the 0 line is defined by start-target)
    private Vector3 target;

    // internal. How much this attack is rotated from the north line.
    private float internalRotation;

    // How much this attack is rotated from the center line.
    private float angleOffset;

    // The scale of the inside ring, from 0-1 relative to the outside ring.
    // This value has no effect if "fixedWidth" is set; it will impact the
    // profile of the attack if "innerExpansionSpeed" is set and different
    // from "expansionSpeed". 
    private float innerScale = 1f;

    // Current scale. This relates exactly to the radius of the attack.
    private float scale = 1f;

    // How fast the inner ring expands
    private Speed innerExpansionSpeed = Speed.MEDIUM_SLOW;

    // How fast the outer ring expands
    private Speed expansionSpeed = Speed.MEDIUM_SLOW;

    // Does nothing if 0. Else, represents how many units there are between
    // the inner and outer ring at all times.
    private float fixedWidth;

    // internal. Time since the move started
    private float currentTime;

    // The maximum lifetime of this attack
    private float maxTime = 100f;

    // Every AOE has the same material, for now. We cache it here.
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
            Destroy(this.gameObject);
        }

        // Update the size of the AOE per its expansion rate.
        // We divide by two because AOEs move based on radius, not diameter;
        // this makes the speeds faster than for projectiles without this correction.
        scale += (float)expansionSpeed * Time.deltaTime;
        gameObject.transform.localScale = scale * Vector3.one;

        // If the inner expansion speed is set, we must recompute the mesh- except if it's equal 
        // to the outer expansion speed, which is the same as just scaling. Then we don't recompute.
        if (Mathf.Abs((float)innerExpansionSpeed) > 0.01f && !Mathf.Approximately((float)expansionSpeed, (float)innerExpansionSpeed))
        {
            //Debug.Log("Separate inner update");
            float ideal = ((float)innerExpansionSpeed / (float)expansionSpeed);
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
        obj.transform.position = self.transform.position;
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
            aoe.regions[i] = false;
        }

        // Initial config
        aoe.SetStart(null)
           .SetTarget(null)
           .SetSpeed(Speed.MEDIUM)
           .SetInnerSpeed(Speed.MEDIUM)
           .SetInnerScale(0.9f);
        aoe.RecomputeMeshHole(); // Initial computation

        // We set the scale to 0 so that timing based attacks work properly.
        // Otherwise the scale is 1, because computing the mesh required it.
        aoe.scale = 0f;
        obj.transform.localScale = aoe.scale * Vector3.one;
        return aoe;
    }

    public AOE On(float from, float to)
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
        RecomputeMeshHole();
        return this;
    }

    public AOE Off(float from, float to)
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
        RecomputeMeshHole();
        return this;
    }

    public AOE SetStart(Vector3? start)
    {
        this.start = start ?? entity.transform.position;
        this.transform.position = this.start;
        UpdateOrientation();
        return this;
    }

    public AOE SetTarget(Vector3? target)
    {
        Vector3 targetPosition;
        if (target == null)
        {
            if (entity.name.Equals(BossController.BOSS_NAME))
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
            targetPosition = target.Value;
        }

        this.target = targetPosition;
        UpdateOrientation();
        return this;
    }

    // Updates the 0 line for the AOE attack. Called when start or target change.
    private void UpdateOrientation() {
        // Remove any height from the start and target vectors
        Vector3 topDownSpawn = new Vector3(start.x, 0, start.z);
        Vector3 topDownTarget = new Vector3(target.x, 0, target.z);

        float degrees = Vector3.Angle(Vector3.forward, topDownTarget - topDownSpawn);
        if (topDownTarget.x < 0) {
            degrees = 360 - degrees;
        }
        this.internalRotation = degrees;
        Debug.Log("internal rotation: " + degrees);

        // Compute the final rotation
        Quaternion rotation = Quaternion.AngleAxis(degrees + this.angleOffset, Vector3.up);
        this.gameObject.transform.rotation = rotation;
    }

    public AOE SetAngleOffset(float degrees) {
        this.angleOffset = degrees;
        this.gameObject.transform.rotation = Quaternion.AngleAxis(degrees, Vector3.up);
        UpdateOrientation();
        return this;
    }

    public AOE SetSpeed(Speed speed)
    {
        this.expansionSpeed = speed;
        return this;
    }

    public AOE SetFixedWidth(float width) {
        this.innerScale = 0f;
        this.innerExpansionSpeed = Speed.FROZEN;
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

    public AOE SetInnerSpeed(Speed speed) {
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
            float degrees = Vector3.Angle(Vector3.forward, playerPositionFlat);
            if (playerPositionFlat.x < 0) {
                degrees = 360 - degrees;
            }
            degrees -= internalRotation;
            degrees -= angleOffset;
            degrees = Mathf.Repeat(degrees, 360f);

            Debug.Log(degrees);

            int section = (int)(degrees / THETA_STEP);
            //Debug.Log("In section " + section);

            if (regions[section]) {
                Entity.DamageEntity(otherEntity, this.entity, damage);
            }
        }
    }

    // Makes a mesh, possibly with a hole if variable size in the middle.
    private void RecomputeMeshHole() {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i < NUM_SECTIONS; i++)
        {
            if (!regions[i]) {
                continue;
            }

            float theta1 = Mathf.Deg2Rad * (90f + (i * THETA_STEP));
            float theta2 = Mathf.Deg2Rad * (90f + ((i + 1) * THETA_STEP));

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
