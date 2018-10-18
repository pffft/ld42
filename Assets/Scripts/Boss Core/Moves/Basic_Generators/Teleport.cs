using AI;
using Projectiles;
using static BossController;
using UnityEngine;

namespace Moves.Basic
{
    public class Teleport : AISequence
    {
        public Teleport(Vector3? target = null, int speed = 25) : base
        (
            () =>
            {
                self.movespeed.LockTo(speed);
                if (target.HasValue)
                {
                    GameManager.Boss.StartCoroutine(Dash(target.Value));
                    Glare();
                    return;
                }

                float minDistance = 35f;
                float minAngle = 50f;
                float maxAngle = 100f;

                Vector3 oldPosVector = GameManager.Boss.transform.position - GameManager.Player.transform.position;

                int count = 0;
                Vector3 rawPosition;
                do
                {
                    count++;
                    float degreeRotation = Random.Range(minAngle, maxAngle) * (Random.Range(0, 2) == 0 ? -1 : 1);
                    float distance = Random.Range(minDistance * GameManager.Arena.transform.localScale.x, 50f * GameManager.Arena.transform.localScale.x);

                    if (count == 15)
                    {
                        degreeRotation = Random.Range(0, 359f);
                        distance = Random.Range(15, 49f);
                    }

                    Quaternion rot = Quaternion.AngleAxis(degreeRotation, Vector3.up);

                    if (count == 15)
                    {
                        rawPosition = rot * (distance * Vector3.forward);
                    }
                    else
                    {
                        rawPosition = (rot * (oldPosVector * (distance / oldPosVector.magnitude))) + GameManager.Player.transform.position;
                    }
                    rawPosition.y = 0f;

                } while (rawPosition.magnitude > 50f);

                rawPosition.y = 1.31f;
                GameManager.Boss.StartCoroutine(Dash(rawPosition));

                Glare();
            }
        )
        {
            Description = "Teleports to " + (target == null ? "some random position" : target.Value.ToString()) + ".";
        }
    }
}
