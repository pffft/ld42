using UnityEngine;
using CombatCore;
using System.Collections;

namespace World
{
	public class Arena : MonoBehaviour
	{
		[SerializeField]
		private float adjustSpeed = 3f;

		[SerializeField]
		private float threshold = 0.01f;

		[SerializeField]
		private Entity player;

		private float maxArea;

		private float CurrentArea
		{
			get { return Mathf.PI * transform.localScale.x * transform.localScale.x; }
		}

		public void Start()
		{
			float maxRadius = Mathf.Max (transform.localScale.x, transform.localScale.y);
			transform.localScale = Vector3.one * maxRadius;
			maxArea = Mathf.PI * maxRadius * maxRadius;

			if(player != null)
				player.tookDamage += OnPlayerDamageTaken;
		}

		public void OnDestroy()
		{
			if (player != null)
				player.tookDamage -= OnPlayerDamageTaken;
		}

		public void Update()
		{
			if (player == null)
				return;

			//drop the player if they're outside the arena
			if (Vector3.Distance (transform.position, player.transform.position) > 50 * transform.localScale.x)
			{
				Rigidbody playerRB = player.GetComponent<Rigidbody> ();

				playerRB.constraints = playerRB.constraints & ~RigidbodyConstraints.FreezeAll;
				playerRB.useGravity = true;
				playerRB.AddForce (Vector3.down * 20f, ForceMode.Impulse);
				playerRB.AddRelativeTorque (Vector3.right * 2f, ForceMode.Impulse);

				player.GetComponent<Controller> ().enabled = false;
				player.GetComponent<Animator> ().enabled = false;

				Rigidbody[] parts = player.GetComponentsInChildren<Rigidbody> ();
				Vector3 center = Vector3.up + player.transform.position;
				foreach (Rigidbody p in parts)
				{
					p.transform.parent = null;

					p.isKinematic = false;
					p.useGravity = true;
					p.AddForceAtPosition ((center - p.transform.position).normalized * 15f, center, ForceMode.Impulse);
				}
				enabled = false;

				Time.timeScale = 0.3f;  Time.fixedDeltaTime = 0.02f * Time.timeScale;
			}
		}

		private void OnPlayerDamageTaken(Entity victim, Entity attacker, float rawDamage, float calcDamage, bool damageApplied, bool hitShields)
		{
			StopAllCoroutines ();
			StartCoroutine (ChangeArenaSize (maxArea * victim.HealthPerc));
		}

		private IEnumerator ChangeArenaSize(float targetArea)
		{
			while (Mathf.Abs(CurrentArea - targetArea) > threshold)
			{
				float newRadius = Mathf.Lerp (transform.localScale.x, Mathf.Sqrt (targetArea / Mathf.PI), Time.deltaTime * adjustSpeed);
				transform.localScale = Vector3.one * newRadius;

				yield return null;
			}
		}
	}
}
