using System.Collections.Generic;
using UnityEngine;

namespace CombatCore
{
	public partial class Ability
	{
		static Ability()
		{
			latestID = 0;

			repository = new Dictionary<string, Ability> ();

			Put (new Ability (
				"DEBUG",
				"Does nothing",
				null,
				0.1f,
				0,
				DebugAbil)
			);

			Put (new Ability (
				"Dash",
				"Dashes to a place",
				null,
				0.1f,
				0,
				PlayerDash)
			);

			Put (new Ability (
				"Shoot",
				"Shoots a bullet",
				null,
				0f,
				0,
				PlayerShoot)
			);

			Put (new Ability (
				"Reflect",
				"Provides a short period of reflection",
				null,
				1f,
				0,
				PlayerReflect)
			);
		}

		private static bool DebugAbil(Entity subject, Vector3 targetPosition, params object[] args)
		{
			Debug.Log ("BANG");
			return true;
		}

		#region PLAYER_ABILITIES
		private static bool PlayerDash(Entity subject, Vector3 targetPosition, params object[] args)
		{
			float range = (float)args[0];
			Debug.Log ("PlayerDash: " + range);
			Vector3 dir = targetPosition - subject.transform.position;
			Vector3 targetPos = subject.transform.position + dir.normalized * Mathf.Min (range, dir.magnitude);
			Controller c = subject.GetComponent<Controller> ();
			c.StartCoroutine (c.Dashing (targetPos));
			return true;
		}

		private static bool PlayerShoot(Entity subject, Vector3 targetPosition, params object[] args)
		{
			Debug.Log ("PlayerShoot");
			CameraController.GetInstance ().Shake (1f, new Vector3 (10f, 10f, 10f), 0.75f);
			return true;
		}

		private static bool PlayerReflect(Entity subject, Vector3 targetPosition, params object[] args)
		{
			Debug.Log ("PlayerReflect");
			return true;
		}
		#endregion
	}
}