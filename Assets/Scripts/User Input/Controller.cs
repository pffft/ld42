using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	[SerializeField]
	private float dashRange;

	private Rigidbody physbody;
	private CombatCore.Entity self;

	private Vector3 facePos;
	private bool dashing;

    private GameObject arena;

	public void Awake()
	{
		physbody = GetComponent<Rigidbody> ();
		self = GetComponent<CombatCore.Entity> ();
        arena = GameObject.Find("Arena");
	}

	public void Start()
	{
		self.AddAbility (CombatCore.Ability.Get ("Dash"));
		self.AddAbility (CombatCore.Ability.Get ("Shoot"));
		self.AddAbility (CombatCore.Ability.Get ("Reflect"));

		self.tookDamage += Self_tookDamage;

		dashing = false;
	}

	private void Self_tookDamage(CombatCore.Entity victim, CombatCore.Entity attacker, float rawDamage, float calcDamage, bool hitShields)
	{
		CameraController.GetInstance ().Shake (1f, new Vector3 (rawDamage, rawDamage, rawDamage), 0.75f);
        arena.transform.localScale = new Vector3(self.HealthPerc, self.HealthPerc, self.HealthPerc);
		self.AddStatus (new CombatCore.Status ("Invincible", "", null, CombatCore.Status.DecayType.communal, 1, 0.25f, new CombatCore.StatusComponents.Invincible ()));
	}

	public void Update()
	{
		if (dashing)
			return;

		if (Input.GetKey (KeyCode.Space))
		{
			if (self.GetAbility (0).Use (self, facePos, dashRange))
			{
				
			}
		}
		if (Input.GetKey (KeyCode.Mouse0))
		{
			if (self.GetAbility (1).Use (self, facePos))
			{
                
			}
		}
		if (Input.GetKey (KeyCode.Mouse1))
		{
			if (self.GetAbility (2).Use (self, facePos))
			{

			}
		}
	}

	public IEnumerator Dashing(Vector3 targetPosition)
	{
		dashing = true;
		self.GetAbility (0).active = false;
		Vector3 startVelocity = physbody.velocity;
		physbody.velocity = Vector3.zero;
		self.SetInvincible (true);

		Vector3 dashDir = (targetPosition - transform.position).normalized;
		float dist;
		while ((dist = Vector3.Distance (targetPosition, transform.position)) > 0f)
		{
			float dashDistance = self.movespeed.Value * 4 * Time.deltaTime;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, dashDir, out hit, dashDistance, 1 << LayerMask.NameToLayer("Default")))
			{
				transform.position = hit.point;
				break;
			}

			if (dist < dashDistance)
			{
				transform.position = targetPosition;
				break;
			}

			transform.position += dashDir * (dashDistance);
			yield return null;
		}

		self.SetInvincible (false);
		self.GetAbility (0).active = true;
		dashing = false;
	}

	public void FixedUpdate()
	{
		if (dashing)
			return;

		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);
		Plane plane = new Plane (Vector3.up, Vector3.zero);
		float dist;
		if (plane.Raycast (camRay, out dist))
		{
			facePos = camRay.origin + (dist * camRay.direction);
			facePos += new Vector3 (0f, 0.5f, 0f);
			Vector3 dir;
			if ((dir = (facePos - transform.position)).magnitude > dashRange)
			{
				facePos = transform.position + (dir.normalized * dashRange);
			}
		}

		//movement
		Vector3 movementVector = Vector3.zero;

		float x = Input.GetAxisRaw ("Horizontal");
		float y = Input.GetAxisRaw ("Vertical");

		movementVector += (Vector3.forward * y) + (Vector3.right * x);
		GetComponent<Animator> ().SetFloat ("SpeedPerc", movementVector != Vector3.zero ? 1f : 0f);

		if(movementVector != Vector3.zero)
			transform.rotation = Quaternion.LookRotation (movementVector, Vector3.up);

		physbody.velocity = movementVector.normalized * self.movespeed.Value;
	}

	private void facePoint(Vector3 point)
	{
		Quaternion rot = Quaternion.LookRotation (point - transform.position, Vector3.up);
		transform.rotation = rot;
		transform.rotation = Quaternion.Euler (0f, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}

//#if UNITY_EDITOR
	public void OnDrawGizmos()
	{
		UnityEditor.Handles.color = Color.green;
		UnityEditor.Handles.DrawWireArc (facePos, Vector3.up, Vector3.forward, 360f, 1f);

		UnityEditor.Handles.color = Color.red;
		UnityEditor.Handles.DrawWireArc (transform.position, Vector3.up, Vector3.forward, 360f, dashRange);
	}
//#endif
}
