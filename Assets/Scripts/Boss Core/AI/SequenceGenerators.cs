using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Projectiles;
using AOEs;

using BossCore;
using static BossController;

namespace AI
{
    public static class SequenceGenerators
    {
        //public static AISequence Shoot1(Projectile skeleton=null)
        //{
        //    return new AISequence(() =>
        //    {
        //        Glare();
        //        Projectile struc = skeleton ?? Projectile.New(self);
        //        struc.Create();
        //    }).Description("Shot a single " + (skeleton == null ? "default projectile at the player." : skeleton + "."));
        //}

        //public static AISequence Shoot3(Projectile skeleton=null)
        //{
        //    return new AISequence(() =>
        //    {
        //        Glare();

        //        for (int i = 0; i < 3; i++)
        //        {
        //            Projectile newStruc = skeleton ?? Projectile.New(self);
        //            newStruc.angleOffset = -30 + (30 * i) + newStruc.angleOffset;
        //            newStruc.Create();
        //        }
        //    }).Description("Shot three " + (skeleton == null ? "default projectiles at the player." : skeleton + ".")); ;
        //}

        /*
        public static AISequence ShootArc(int density = 50, float from = 0, float to = 360, Projectile skeleton=null)
        {
            return new AISequence(() =>
            {
                Glare();

                // Ensure that "from" is always less than "to".
                if (to < from)
                {
                    float temp = from;
                    from = to;
                    to = temp;
                }

                float step = 360f / density;
                Projectile clone = skeleton ?? Projectile.New(self).Size(Size.MEDIUM);
                for (float i = from; i <= to; i += step)
                {
                    Projectile newStruc = clone.Clone();
                    newStruc.AngleOffset(newStruc.angleOffset + i).Create();
                }
            })
            {
                Description = "Shot an arc (density=" + density + ", from=" + from + ", to=" + to + ") of " +
                    (skeleton == null ? "default projectiles at the player." : skeleton + ".")
            };
        }
        */

        // Deprecated; can be created from two merged ShootArc calls.
        /*
        public static AISequence ShootWall(float angleOffset)
        {
            return new AISequence(AISequence.Merge(
                ShootArc(100, angleOffset + -60, angleOffset + -60 + 28, Projectile.New(self).Speed(Speed.SLOW)),
                ShootArc(100, angleOffset + 20, angleOffset + 60, Projectile.New(self).Speed(Speed.SLOW))
            ))
            { Description = "Shot a wall with offset " + angleOffset + " at the player." };
        }
        */

        public static AISequence ShootHexCurve(bool clockwise = true, Projectile skeleton=null)
        {
            return new AISequence(() =>
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile struc = skeleton ?? Projectile.New(self);

                    float multiplier = clockwise ? 1f : -1f;
                    float curveSpeed = (float)struc.speed * multiplier * 2f;

                    struc.Clone()
                         .AngleOffset(struc.angleOffset + (i * multiplier * 60))
                         .MaxTime(3f)
                         .Create()
                         .Curving(curveSpeed, true);
                }
            })
            {
                Description = "Shot a " + (clockwise ? "clockwise" : "counterclockwise") + " hex curve " +
                    (skeleton == null ? "with a default projectile." : skeleton + ".")
            };
        }

        public static AISequence ShootLine(int amount = 50, float width = 75f, Vector3? target = null, Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM)
        {
            return new AISequence(() =>
            {

                Vector3 targetPos = target.HasValue ? target.Value - GameManager.Boss.transform.position : GameManager.Player.transform.position - GameManager.Boss.transform.position;
                Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * targetPos).normalized;

                for (int i = 0; i < amount; i++)
                {
                    Vector3 spawn = GameManager.Boss.transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
                    Projectile.New(self)
                              .Start(spawn)
                              .Target(spawn + targetPos)
                              .Speed(speed)
                              .Size(size)
                              .Create();
                }
            })
            {
                Description = "Shot a line (amount=" + amount + ", width=" + width + ") at " + (target == null ? " the player" : target.ToString()) +
                    " with speed " + speed + " and size " + size + "."
            };
        }

        public static AISequence ShootDeathHex(float maxTime = 1f)
        {
            return new AISequence(() =>
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.New(self)
                              .Target(GameManager.Player.transform.position)
                              .AngleOffset(i * 60f)
                              .MaxTime(maxTime)
                              .Create()
                              .DeathHex();
                }
            })
            { Description = "Shot a death hex with a lifetime of " + maxTime + " seconds." };
        }

        public static AISequence ShootHomingStrafe(bool clockwise=true, int strafeAmount=5, int speed=25) {
            return new AISequence(
                Strafe(clockwise, strafeAmount, speed),
                new Moves.Basic.Shoot1(Projectile.New(self).Size(Size.MEDIUM).Homing())
            )
            { Description = "Strafed " + strafeAmount + " degrees " + (clockwise ? "clockwise" : "counterclockwise") + " and shot a homing projectile." };
        }

        /*
        public static AISequence ShootAOE(AOE structure)
        {
            return new AISequence(() => { Glare(); structure.Clone().Create(); }) { Description = "Shot " + structure };
        }
        */

        public static AISequence Teleport(Vector3? target = null, int speed = 25)
        {
            return new AISequence(() =>
            {
                self.movespeed.LockTo(speed);
                if (target.HasValue)
                {
                    GameManager.Boss.StartCoroutine(Dashing(target.Value));
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
                GameManager.Boss.StartCoroutine(Dashing(rawPosition));

                Glare();
            })
            { Description = "Teleports to " + (target == null ? "some random position" : target.Value.ToString()) + "." };
        }

        public static IEnumerator Dashing(Vector3 targetPosition)
        {

            eventQueue.Pause();

            Vector3 dashDir = (targetPosition - GameManager.Boss.transform.position).normalized;

            float accDist = 0f, maxDist = Vector3.Distance(targetPosition, GameManager.Boss.transform.position);
            while(accDist < maxDist) {
                float dashDistance = Mathf.Min((insaneMode ? 1.2f : 1f) * self.movespeed.Value * 4 * Time.deltaTime, maxDist - accDist);

                RaycastHit hit;
                if (Physics.Raycast(GameManager.Boss.transform.position, dashDir, out hit, dashDistance, 1 << LayerMask.NameToLayer("Default")))
                {
                    GameManager.Boss.transform.position = hit.point;
                    break;
                }

                GameManager.Boss.transform.position += dashDir * dashDistance;
                accDist += dashDistance;
                yield return null;
            }

            self.movespeed.LockTo(25);
            eventQueue.Unpause();
        }

        public static AISequence Strafe(bool clockwise = true, float degrees = 10f, int speed = 25, Vector3 center = default(Vector3))
        {
            return new AISequence(() =>
            {
                self.movespeed.LockTo(speed);

                Vector3 oldPosVector = GameManager.Boss.transform.position - center;
                Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

                GameManager.Boss.StartCoroutine(Dashing(rot * oldPosVector));
            });
        }

        public static AISequence CameraMove(bool isFollow = false, Vector3? targetPosition = null)
        {
            return new AISequence(() =>
            {
                CameraController.GetInstance().IsFollowing = isFollow;

                if (targetPosition.HasValue)
                {
                    CameraController.GetInstance().Goto(targetPosition.Value, 1);
                }
            })
            { Description = "Moved the camera to " + targetPosition + ". Camera is now " + (isFollow ? "" : "not") + " following." };
        }

        public static AISequence PlayerLock(bool enableLock = true)
        {
            return new AISequence(() =>
            {
                if (enableLock)
                {
                    playerLockPosition = GameManager.Player.transform.position;
                }
                isPlayerLocked = enableLock;
            })
            { Description = (enableLock ? "Locked onto" : "Unlocked from") + " the player." };
        }
    }
}
