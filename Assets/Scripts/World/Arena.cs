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

		[SerializeField]
		private Entity player;

		private float maxArea;

        private static float ARENA_SCALE = 50f;

        private static Arena instance = null;

		private float CurrentArea
		{
			get { return Mathf.PI * transform.localScale.x * transform.localScale.x; }
		}

        public static float RadiusInWorldUnits 
        {
            get { GetInstance(); return instance.transform.localScale.x * ARENA_SCALE; }
            set
			{
                Debug.Log("Setting arena scale to " + value);
                GetInstance();
                ARENA_SCALE = value;
                instance.transform.localScale = GameObject.Find("Player").GetComponent<Entity>().HealthPerc * (ARENA_SCALE / 50f) * Vector3.one;
				/*Entity.DamageEntity(GameObject.Find("Player").GetComponent<Entity>(), BossController.self, 0f);*/
			}
        }

        public static Arena GetInstance() {
            if (instance == null) {
                instance = GameObject.Find("Arena").GetComponent<Arena>();
            }
            return instance;
        }

		public void Start()
		{
            GetInstance();

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
			if (Vector3.Distance (transform.position, player.transform.position) > 50f * transform.localScale.x)
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

				player.OnDeath ();
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
