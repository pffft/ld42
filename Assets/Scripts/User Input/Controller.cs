using CombatCore;
using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    private const string ABILITY_1 = "Dash", ABILITY_2 = "Throw", ABILITY_3 = "Block";

    [SerializeField]
    private float dashRange = 15f;

    private Rigidbody physbody;
    private Entity self;

    private Vector3 facePos;
    private bool dashing;

    public void Awake()
    {
        physbody = GetComponent<Rigidbody>();
        self = GetComponent<Entity>();
        dashing = false;
    }

    public void Start()
    {
        self.AddAbility(Ability.Get(ABILITY_1));
        self.AddAbility(Ability.Get(ABILITY_2));
        self.AddAbility(Ability.Get(ABILITY_3));

        self.tookDamage += Self_tookDamage;
        self.died += Self_died;
    }

    private void Self_died()
    {

    }

    private void Self_tookDamage(Entity victim, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
    {
        if (damageApplied)
        {
            CameraController.GetInstance().Shake(1f, new Vector3(rawDamage * 10f, rawDamage * 10f, rawDamage * 10f), 0.75f);
            self.AddStatus(new Status("Invincible", "", null, Status.DecayType.communal, 1, 0.25f, new CombatCore.StatusComponents.Invincible()));
        }
    }

    public void Update()
    {
        if (dashing || self.IsRooted())
            return;

        if (Input.GetKey(KeyCode.Space))
        {
            if (!self.GetAbility(ABILITY_1).Use(self, facePos, dashRange))
            {
                //TODO play failed to use ability sound
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if (!self.GetAbility(ABILITY_2).Use(self, facePos))
            {
                //TODO play failed to use ability sound
            }
        }
        if (Input.GetButtonDown("Fire2"))
        {
            if (!self.GetAbility(ABILITY_3).Use(self, facePos))
            {
                //TODO play failed to use ability sound
            }
        }
    }

    public IEnumerator Dashing(Vector3 targetPosition)
    {
        dashing = true;
        physbody.velocity = Vector3.zero;
        self.GetAbility(0).active = false;
        self.SetInvincible(true);

        GameObject dashEffectPref = Resources.Load<GameObject>("Prefabs/PlayerDashEffect");
        GameObject effect = Instantiate(dashEffectPref, gameObject.transform, false);
        effect.transform.localPosition = new Vector3(0f, 1f);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        Vector3 dashDir = (targetPosition - transform.position).normalized;
        float accDist = 0f, maxDist = Vector3.Distance(targetPosition, transform.position);
        while (accDist < maxDist)
        {
            float dashDistance = Mathf.Min(self.movespeed.Value * 4 * Time.deltaTime, maxDist - accDist);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dashDir, out hit, dashDistance, 1 << LayerMask.NameToLayer("Default")))
            {
                transform.position = hit.point;
                break;
            }

            transform.position += dashDir * (dashDistance);
            accDist += dashDistance;
            yield return null;
        }

        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        ParticleSystem.EmissionModule em = effect.GetComponentInChildren<ParticleSystem>().emission;
        em.enabled = false;
        Destroy(effect, self.GetAbility(0).cooldownMax);

        self.SetInvincible(false);
        self.GetAbility(0).active = true;
        physbody.velocity = Vector3.zero;
        dashing = false;
    }

    public void FixedUpdate()
    {
        if (dashing || self.IsRooted())
            return;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float dist;
        if (plane.Raycast(camRay, out dist))
        {
            facePos = camRay.origin + (dist * camRay.direction);
            Vector3 dir;
            if ((dir = (facePos - transform.position)).magnitude > dashRange)
            {
                facePos = transform.position + (dir.normalized * dashRange);
            }
        }

        //movement
        Vector3 movementVector = new Vector3(0f, physbody.velocity.y, 0f);

        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        movementVector += (Vector3.forward * y) + (Vector3.right * x);
        GetComponent<Animator>().SetFloat("SpeedPerc", Mathf.Abs(x) > 0f || Mathf.Abs(y) > 0f ? 1f : 0f);

        if (Mathf.Abs(x) > 0f || Mathf.Abs(y) > 0f)
            transform.rotation = Quaternion.LookRotation(movementVector, Vector3.up);

        physbody.velocity = movementVector.normalized * self.movespeed.Value;
    }

    public Vector3 GetDashTargetPoint()
    {
        return facePos;
    }

    private void facePoint(Vector3 point)
    {
        Quaternion rot = Quaternion.LookRotation(point - transform.position, Vector3.up);
        transform.rotation = rot;
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    public void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireArc(facePos, Vector3.up, Vector3.forward, 360f, 1f);

        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireArc(new Vector3(transform.position.x, 0f, transform.position.z), Vector3.up, Vector3.forward, 360f, dashRange);

        UnityEditor.Handles.DrawWireArc(AI.AISequence.SMOOTHED_LEADING_PLAYER_POSITION.GetValue(), Vector3.up, Vector3.forward, 360f, 1f);

        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }
        GameObject shield = GameManager.PlacedShield;
        if (shield != null)
        {
            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireArc(shield.transform.position, Vector3.up, Vector3.forward, 360f, 5f);
        }
    }
}
