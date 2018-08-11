using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
	private Rigidbody2D physbody;
	private CombatCore.Entity self;

	public void Awake()
	{
		physbody = GetComponent<Rigidbody2D> ();
		self = GetComponent<CombatCore.Entity> ();
	}

	public void Start()
	{
		self.AddAbility (CombatCore.Ability.Get ("DEBUG"));
	}

	public void Update()
	{
		if (Input.GetKey (KeyCode.Space))
		{
			if (self.GetAbility (0).Use (self, Input.mousePosition))
			{
				Debug.Log ("y");
			}
		}
	}

	public void FixedUpdate()
	{
		if (!physbody.simulated)
			return;

		//face the mouse
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 0));
		facePoint (mousePos);

		//movement
		Vector2 movementVector = Vector2.zero;

		bool up = Input.GetKey(KeyCode.W);
		bool left = Input.GetKey (KeyCode.A);
		bool down = Input.GetKey (KeyCode.S);
		bool right = Input.GetKey (KeyCode.D);

		if (up)
			movementVector += Vector2.up;
		if (left)
			movementVector += Vector2.left;
		if (down)
			movementVector += Vector2.down;
		if (right)
			movementVector += Vector2.right;

		physbody.AddForce (movementVector * self.movespeed.Value);
	}

	private void facePoint(Vector2 point)
	{
		Quaternion rot = Quaternion.LookRotation (transform.position - new Vector3 (point.x, point.y, -100f), Vector3.forward);
		transform.rotation = rot;
		transform.eulerAngles = new Vector3 (0f, 0f, transform.eulerAngles.z);
	}

}
