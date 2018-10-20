using UnityEngine;
using CombatCore;
using System.Collections;

namespace World
{
	public class Arena : MonoBehaviour
	{
        #region Arena Location constants
        public static readonly float BOSS_HEIGHT = 1.31f;
        private static readonly float FAR = 45f;
        private static readonly float MED = 30f;
        private static readonly float CLOSE = 15f;

        public static readonly Vector3 CENTER = new Vector3(0, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_FAR = new Vector3(0f, BOSS_HEIGHT, FAR);
        public static readonly Vector3 SOUTH_FAR = new Vector3(0f, BOSS_HEIGHT, -FAR);
        public static readonly Vector3 EAST_FAR = new Vector3(45f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_FAR = new Vector3(-45f, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_MED = new Vector3(0f, BOSS_HEIGHT, MED);
        public static readonly Vector3 SOUTH_MED = new Vector3(0f, BOSS_HEIGHT, -MED);
        public static readonly Vector3 EAST_MED = new Vector3(30f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_MED = new Vector3(-30f, BOSS_HEIGHT, 0);

        public static readonly Vector3 NORTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, CLOSE);
        public static readonly Vector3 SOUTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, -CLOSE);
        public static readonly Vector3 EAST_CLOSE = new Vector3(15f, BOSS_HEIGHT, 0);
        public static readonly Vector3 WEST_CLOSE = new Vector3(-15f, BOSS_HEIGHT, 0);
        #endregion

		[SerializeField]
		private float adjustSpeed = 3f;

		[SerializeField]
		private float threshold = 0.01f;

		private float maxArea;

        private static float ARENA_SCALE = 50f;

		private float CurrentArea
		{
			get { return Mathf.PI * transform.localScale.x * transform.localScale.x; }
		}

        public float RadiusInWorldUnits 
        {
            get { return transform.localScale.x * ARENA_SCALE; }
            set
			{
                ARENA_SCALE = value;
                maxArea = ARENA_SCALE * ARENA_SCALE / 50f / 50f * Mathf.PI;
                StartCoroutine(ChangeArenaSize(GameManager.Player.GetComponent<Entity>().HealthPerc * maxArea));
			}
        }

		public void Start()
		{
            float maxRadius = Mathf.Max (transform.localScale.x, transform.localScale.y);
			transform.localScale = Vector3.one * maxRadius;
			maxArea = Mathf.PI * maxRadius * maxRadius;

            if(GameManager.Player != null)
                GameManager.Player.GetComponent<Entity>().tookDamage += OnPlayerDamageTaken;
		}

		public void OnDestroy()
		{
			if (GameManager.Player != null)
                GameManager.Player.GetComponent<Entity>().tookDamage -= OnPlayerDamageTaken;
		}

		public void Update()
		{
			if (GameManager.Player == null)
				return;

			//drop the player if they're outside the arena
			if (Vector3.Distance (transform.position, GameManager.Player.transform.position) > 50f * transform.localScale.x)
			{
				Rigidbody playerRB = GameManager.Player.GetComponent<Rigidbody> ();

				playerRB.constraints = playerRB.constraints & ~RigidbodyConstraints.FreezeAll;
				playerRB.useGravity = true;
				playerRB.AddForce (Vector3.down * 20f, ForceMode.Impulse);
				playerRB.AddRelativeTorque (Vector3.right * 2f, ForceMode.Impulse);

                GameManager.Player.GetComponent<Controller> ().enabled = false;
                GameManager.Player.GetComponent<Animator> ().enabled = false;

                Rigidbody[] parts = GameManager.Player.GetComponentsInChildren<Rigidbody> ();
				Vector3 center = Vector3.up + GameManager.Player.transform.position;
				foreach (Rigidbody p in parts)
				{
					p.transform.parent = null;

					p.isKinematic = false;
					p.useGravity = true;
					p.AddForceAtPosition ((center - p.transform.position).normalized * 15f, center, ForceMode.Impulse);

					if(p.gameObject != GameManager.Player.gameObject)
						Destroy (p.gameObject, 5f);
				}
				enabled = false;

				GameManager.TimeScale = 0.3f;

                GameManager.Player.GetComponent<Entity>().OnDeath ();
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
                float newRadius = Mathf.Lerp (transform.localScale.x, (ARENA_SCALE / 50f) * Mathf.Sqrt (targetArea / Mathf.PI), Time.deltaTime * adjustSpeed);
				transform.localScale = Vector3.one * newRadius;

				yield return null;
			}
		}
	}
}
