using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;
using UnityEditor;

namespace Combat.AbilityBehaviors 
{

    public class PlayerShootBehavior : AbilityBehavior
    {
        private System.Func<bool> ShootTriggerPressed;

        private static readonly float MAX_ANGLE = 90f;
        private static readonly float MIN_ANGLE = 2f;

        PlayerGizmos gizmos;

        private float baseMaxCooldown;

        private static readonly float CHARGE_TIME = 0.6f;

        // Called on trigger, to check any other conditions before activating.
        public override bool Start(GameObject blackboard, Ability ability)
        {
            ShootTriggerPressed = ability.Trigger;
            if (gizmos == null) 
            {
                gizmos = blackboard.GetComponent<PlayerGizmos>();
            }
            return true;
        }

        // Called each update while the ability is active.
        public override IEnumerator Update()
        {
            float duration = CHARGE_TIME;
            float percent;
            float actualAngle;

            // While the trigger is held, decrement duration and yield return
            while (ShootTriggerPressed())
            {
                duration -= Time.deltaTime;
                
                percent = (CHARGE_TIME - duration) / CHARGE_TIME;
                actualAngle = Mathf.Lerp(MAX_ANGLE, MIN_ANGLE, percent);
                gizmos.DrawShotgun(true, actualAngle, GetMousePos() - GameManager.Player.transform.position);


                yield return null;
            }

            // Trigger was released. Check duration to see what we should shoot.
            if (duration <= 0f) 
            {
                // Full power
                // Debug.Log("Fully charged");
            }
            else
            {
                
            }

            // Partial power. Convert to a float in [0, 1] and do something.
            percent = (CHARGE_TIME - duration) / CHARGE_TIME;
            // Debug.Log("Partially charged, percent: " + percent);

            actualAngle = Mathf.Lerp(MAX_ANGLE, MIN_ANGLE, percent);
            
            gizmos.DrawShotgun(true, actualAngle, GetMousePos() - GameManager.Player.transform.position);
            // Vector3 towardsBoss = GameManager.Boss.transform.position - GameManager.Player.transform.position;
            // Handles.DrawWireArc(
            //     GameManager.Player.transform.position, 
            //     Vector3.up,
            //     Quaternion.AngleAxis(-actualAngle / 2.0f, Vector3.up) * towardsBoss, 
            //     actualAngle, 
            //     10
            // );
            

            for (int i = 0; i < 7; i++)
            {
                float bulletAngle = Random.Range(-actualAngle / 2.0f, actualAngle / 2.0f);

                // Quaternion.AngleAxis(-actualAngle / 2.0f, Vector3.up) * target
                Vector3 idealDir = GetMousePos() - GameManager.Player.transform.position + (Vector3.up * Constants.Positions.BOSS_HEIGHT);
                Vector3 actualDir = Quaternion.AngleAxis(bulletAngle, Vector3.up) * idealDir;

                RaycastHit hit;
                if (Physics.SphereCast(
                    GameManager.Player.transform.position,
                    Mathf.Lerp(0.1f, 5f, percent),
                    actualDir,
                    out hit,
                    GameManager.Arena.RadiusInWorldUnits * 2,
                    LayerMask.GetMask("Boss")
                )) 
                {
                    Debug.Log("Did damage to boss!");
                    CombatCore.Entity.DamageEntity(
                        GameManager.Boss.GetComponent<CombatCore.Entity>(),
                        GameManager.Player.GetComponent<CombatCore.Entity>(),
                        1
                    );
                } 
                else 
                {
                    Debug.Log("Spherecast failed");
                }
            }

        }

        public Vector3 GetMousePos() 
        {
            Vector3 facePos = Vector3.zero;

            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            float dist;
            if (plane.Raycast(camRay, out dist))
            {
                facePos = camRay.origin + (dist * camRay.direction);
                Vector3 dir;
                if ((dir = (facePos - GameManager.Player.transform.position)).magnitude > 15f)
                {
                    facePos = GameManager.Player.transform.position + (dir.normalized * 15f);
                }
            }

            return facePos;
        }

        // Called when the ability ends.
        public override void Finish(Result result)
        {
            gizmos.DrawShotgun(false, 45, Vector3.zero);
            Debug.Log("ah");
        }
    }
}
