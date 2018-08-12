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

	public void Awake()
	{
		physbody = GetComponent<Rigidbody> ();
		self = GetComponent<CombatCore.Entity> ();
	}

	public void Start()
	{
		self.AddAbility (CombatCore.Ability.Get ("Dash"));
		self.AddAbility (CombatCore.Ability.Get ("Shoot"));
		self.AddAbility (CombatCore.Ability.Get ("Reflect"));

		dashing = false;
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

		Vector3 dashDir = (targetPosition - transform.position).normalized;
		float dist;
		while ((dist = Vector3.Distance (targetPosition, transform.position)) > 1f)
		{
			if (dist < self.movespeed.Value * Time.deltaTime)
			{
				transform.position = targetPosition;
				break;
			}
			transform.position += dashDir * (self.movespeed.Value * 2 * Time.deltaTime);
			yield return null;
		}

		physbody.velocity = startVelocity;
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
		}
		facePoint (facePos);

		//movement
		Vector3 movementVector = Vector3.zero;

		bool forward = Input.GetKey(KeyCode.W);
		bool left = Input.GetKey (KeyCode.A);
		bool backward = Input.GetKey (KeyCode.S);
		bool right = Input.GetKey (KeyCode.D);

		if (forward)
			movementVector += Vector3.forward;
		if (left)
			movementVector += Vector3.left;
		if (backward)
			movementVector += Vector3.back;
		if (right)
			movementVector += Vector3.right;

		physbody.AddForce (movementVector * self.movespeed.Value);
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
