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

        private float maxArea;

        private const float DEFAULT_ARENA_SCALE = 50f;
        private static float ARENA_SCALE = 50f;

        private float CurrentArea
        {
            get { return Mathf.PI * transform.localScale.x * transform.localScale.x; }
        }

        public float RadiusInWorldUnits 
        {
            get { return transform.localScale.x * DEFAULT_ARENA_SCALE; }
            set
            {
                ARENA_SCALE = value;
                maxArea = ARENA_SCALE * ARENA_SCALE / DEFAULT_ARENA_SCALE / DEFAULT_ARENA_SCALE * Mathf.PI;
                if (GameManager.Player.GetComponent<Entity>())
                {
                    StartCoroutine(ChangeArenaSize(GameManager.Player.GetComponent<Entity>().HealthPerc * maxArea));
                }

            }
        }

        public void Start()
        {
            float maxRadius = Mathf.Max (transform.localScale.x, transform.localScale.y);
            transform.localScale = Vector3.one * maxRadius;
            maxArea = Mathf.PI * maxRadius * maxRadius;

            if (GameManager.Player != null)
            {
                if (GameManager.Player.GetComponent<Entity>() != null)
                {
                    GameManager.Player.GetComponent<Entity>().tookDamage += OnPlayerDamageTaken;
                }
            }
        }

        public void OnDestroy()
        {
            if (GameManager.Player != null)
            {
                if (GameManager.Player.GetComponent<Entity>() != null) {
                    GameManager.Player.GetComponent<Entity>().tookDamage -= OnPlayerDamageTaken;
                }
            }
        }

        public void Update()
        {
            if (GameManager.Player == null)
                return;

            //drop the player if they're outside the arena
            if (Vector3.Distance (transform.position, GameManager.Player.transform.position) > RadiusInWorldUnits)
            {
                //swap out a dummy and blow it up
                GameManager.Player.gameObject.SetActive (false);
                GameObject dummyPlayer = Instantiate(
                    Resources.Load<GameObject> ("Prefabs/Player"),
                    GameManager.Player.transform.position,
                    GameManager.Player.transform.rotation);
                    

                //add force
                Rigidbody playerRB = dummyPlayer.GetComponent<Rigidbody> ();
                playerRB.constraints = playerRB.constraints & ~RigidbodyConstraints.FreezeAll;
                playerRB.useGravity = true;
                playerRB.AddForce (Vector3.down * 20f, ForceMode.Impulse);
                playerRB.AddRelativeTorque (Vector3.right * 2f, ForceMode.Impulse);

                //disable control
                dummyPlayer.GetComponent<Controller> ().enabled = false;
                dummyPlayer.GetComponent<Animator> ().enabled = false;

                //boom
                Rigidbody[] parts = dummyPlayer.GetComponentsInChildren<Rigidbody> ();
                Vector3 center = Vector3.up + dummyPlayer.transform.position;
                foreach (Rigidbody p in parts)
                {
                    p.transform.parent = null;

                    p.isKinematic = false;
                    p.useGravity = true;
                    p.AddForceAtPosition ((center - p.transform.position).normalized * 15f, center, ForceMode.Impulse);

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
            float idealRadius = Mathf.Sqrt((ARENA_SCALE / DEFAULT_ARENA_SCALE) * Mathf.Sqrt(targetArea / Mathf.PI));
            while (Mathf.Abs(CurrentArea - targetArea) > threshold)
            {
                float newRadius = Mathf.Lerp(transform.localScale.x, idealRadius, Time.deltaTime * adjustSpeed);
                transform.localScale = Vector3.one * newRadius;

                yield return null;
            }

            // Set to the ideal value
            transform.localScale = Vector3.one * idealRadius;
        }
    }
}
