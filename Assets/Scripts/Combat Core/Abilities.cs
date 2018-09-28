using System.Collections.Generic;
using UnityEngine;
using Projectiles;

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
				0.3f,
				0,
				DebugAbil)
			);

			Put (new Ability (
				"Dash",
				"Dashes to a place",
				null,
				0.3f,
				0,
				PlayerDash)
			);

			Put (new Ability (
				"Throw",
				"Throws a shield",
				null,
				0.5f,
				0,
                PlayerThrow)
			);

			Put (new Ability (
				"Block",
				"Drops a protective shield",
				null,
                cooldownMax: 3f,
				chargesMax: 0,
                effect: PlayerBlock)
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

            if (subject.HasStatus("ShieldRegen")) {
                subject.RemoveStatus(Status.Get("ShieldRegen"));
            }

			return true;
		}

        // TODO: this is a basic stub for testing
        // this needs to throw the shield model (+animation)
        private static bool PlayerThrow(Entity subject, Vector3 targetPosition, params object[] args) 
        {
            Debug.Log("Player shield throw");

            // Can't throw the shield if it's already out!
            if (subject.HasStatus("Shield Placed"))
            {
                // If we're tied to the shield, then reclaim it
                if (subject.HasStatus("ShieldRegen"))
                {
                    Debug.Log("Tied to shield, reclaiming it");
                    subject.RemoveStatus("Shield Placed");
                    subject.RemoveStatus("ShieldRegen");
                    return true;
                }
                else if ((GameObject.Find("Shield Down").transform.position - subject.transform.position).magnitude < 5f)
                {
                    Debug.Log("Close to shield, reclaiming it");
                    // If we're close enough to the shield, then reclaim it
                    subject.RemoveStatus("Shield Placed");
                    return true;
                }
                else
                {
                    Debug.Log("Too far to reclaim shield");
                    return false;
                }
            }

            if (subject.HasStatus("yaintgotshield")) {
                if ((GameObject.Find("Shield Down").transform.position - subject.transform.position).magnitude < 5f) {
                    Debug.Log("Close to shield, reclaiming it");
                    subject.RemoveStatus("yaintgotshield");
                    return true;
                }
                Debug.Log("Shield is already thrown. Go pick it up!");
                return false;
            }

            Debug.Log("YEET");
            Projectile.New(subject)
                      .Target(targetPosition)
                      .MaxTime(2f)
                      .Speed(BossCore.Speed.VERY_FAST)
                      .Size(Size.MEDIUM)
                      .Homing()
                      .Create();
            subject.AddStatus(Status.Get("yaintgotshield"));

            return true;

        }

        // TODO: this is a basic stub for testing
        // this needs a better model for the blocking shield
        private static bool PlayerBlock(Entity subject, Vector3 targetPosition, params object[] args) 
        {
            Debug.Log("PlayerBlock");
            Debug.Log("Aiming at " + targetPosition);
            Debug.Log("Degrees: " + Vector3.Angle(Vector3.forward, targetPosition - subject.transform.position));
            subject.AddStatus(Status.Get("ShieldRegen"));

            Status shieldPlacedStatus = Status.Get("Shield Placed");
            shieldPlacedStatus.GetComponent<StatusComponents.ShieldPlaced>().SetTarget(targetPosition);
            subject.AddStatus(shieldPlacedStatus);
            return true;
        }
		#endregion
	}
}