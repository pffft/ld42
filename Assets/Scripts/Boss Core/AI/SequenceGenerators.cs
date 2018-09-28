﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Projectiles;
using BossCore;
using static BossController;

namespace AI
{
    public static class SequenceGenerators
    {

        #region Arena Location constants
        public static float BOSS_HEIGHT = 1.31f;
        private static float FAR = 45f;
        private static float MED = 30f;
        private static float CLOSE = 15f;

        public static Vector3 CENTER = new Vector3(0, BOSS_HEIGHT, 0);

        public static Vector3 NORTH_FAR = new Vector3(0f, BOSS_HEIGHT, FAR);
        public static Vector3 SOUTH_FAR = new Vector3(0f, BOSS_HEIGHT, -FAR);
        public static Vector3 EAST_FAR = new Vector3(45f, BOSS_HEIGHT, 0);
        public static Vector3 WEST_FAR = new Vector3(-45f, BOSS_HEIGHT, 0);

        public static Vector3 NORTH_MED = new Vector3(0f, BOSS_HEIGHT, MED);
        public static Vector3 SOUTH_MED = new Vector3(0f, BOSS_HEIGHT, -MED);
        public static Vector3 EAST_MED = new Vector3(30f, BOSS_HEIGHT, 0);
        public static Vector3 WEST_MED = new Vector3(-30f, BOSS_HEIGHT, 0);

        public static Vector3 NORTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, CLOSE);
        public static Vector3 SOUTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, -CLOSE);
        public static Vector3 EAST_CLOSE = new Vector3(15f, BOSS_HEIGHT, 0);
        public static Vector3 WEST_CLOSE = new Vector3(-15f, BOSS_HEIGHT, 0);
        #endregion

        public static AISequence Shoot1(Projectile.ProjectileStructure? skeleton=null)
        {
            return new AISequence(() =>
            {
                Glare();
                Projectile.ProjectileStructure struc = skeleton ?? Projectile.New(self);
                struc.Create();
            });
        }

        public static AISequence Shoot3(Projectile.ProjectileStructure? skeleton=null)
        {
            return new AISequence(() =>
            {
                Glare();

                for (int i = 0; i < 3; i++)
                {
                    Projectile.ProjectileStructure newStruc = skeleton ?? Projectile.New(self);
                    newStruc.angleOffset = -30 + (30 * i) + newStruc.angleOffset;
                    newStruc.Create();
                }
            });
        }

        public static AISequence ShootArc(int density = 50, float from = 0, float to = 360, Projectile.ProjectileStructure? skeleton=null)
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
                for (float i = from; i <= to; i += step) {
                    Projectile.ProjectileStructure newStruc = skeleton ?? Projectile.New(self).Size(Size.MEDIUM);
                    newStruc.AngleOffset(newStruc.angleOffset + i).Create();
                }
            });
        }

        // Deprecated; can be created from two merged ShootArc calls.
        public static AISequence ShootWall(float angleOffset)
        {
            return AISequence.Merge(
                ShootArc(100, angleOffset + -60, angleOffset + -60 + 28, Projectile.New(self).Speed(Speed.SLOW)),
                ShootArc(100, angleOffset + 20, angleOffset + 60, Projectile.New(self).Speed(Speed.SLOW))
            );
        }

        public static AISequence ShootHexCurve(bool clockwise = true, Projectile.ProjectileStructure? skeleton=null)
        {
            return new AISequence(() =>
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.ProjectileStructure struc = skeleton ?? Projectile.New(self);

                    float multiplier = clockwise ? 1f : -1f;
                    float curveSpeed = (float)struc.speed * multiplier * 2f;

                    struc.AngleOffset(struc.angleOffset + (i * multiplier * 60))
                         .MaxTime(3f)
                         .Create()
                         .Curving(curveSpeed, true);
                }
            });
        }

        public static AISequence ShootLine(int amount = 50, float width = 75f, Vector3? target = null, Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM)
        {
            return new AISequence(() =>
            {

                Vector3 targetPos = target.HasValue ? target.Value - instance.transform.position : player.transform.position - instance.transform.position;
                Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * targetPos).normalized;

                for (int i = 0; i < amount; i++)
                {
                    Vector3 spawn = instance.transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
                    Projectile.New(self)
                              .Start(spawn)
                              .Target(spawn + targetPos)
                              .Speed(speed)
                              .Size(size)
                              .Create();
                }
            });
        }

        public static AISequence ShootDeathHex(float maxTime = 1f)
        {
            return new AISequence(() =>
            {
                for (int i = 0; i < 6; i++)
                {
                    Projectile.New(self)
                              .Target(player.transform.position)
                              .AngleOffset(i * 60f)
                              .MaxTime(maxTime)
                              .Create()
                              .DeathHex();
                }
            });
        }

        public static AISequence ShootAOE(AOE.AOEStructure structure)
        {
            return new AISequence(() => { Glare(); structure.Create(); });
        }

        public static AISequence Teleport(Vector3? target = null, int speed = 25)
        {
            return new AISequence(() =>
            {
                self.movespeed.LockTo(speed);
                if (target.HasValue)
                {
                    instance.StartCoroutine(Dashing(target.Value));
                    Glare();
                    return;
                }

                float minDistance = 35f;
                float minAngle = 50f;
                float maxAngle = 100f;

                Vector3 oldPosVector = instance.transform.position - player.transform.position;

                int count = 0;
                Vector3 rawPosition;
                do
                {
                    count++;
                    float degreeRotation = Random.Range(minAngle, maxAngle) * (Random.Range(0, 2) == 0 ? -1 : 1);
                    float distance = Random.Range(minDistance * arena.transform.localScale.x, 50f * arena.transform.localScale.x);

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
                        rawPosition = (rot * (oldPosVector * (distance / oldPosVector.magnitude))) + player.transform.position;
                    }
                    rawPosition.y = 0f;

                } while (rawPosition.magnitude > 50f);

                rawPosition.y = 1.31f;
                instance.StartCoroutine(Dashing(rawPosition));

                Glare();
            });
        }

        public static IEnumerator Dashing(Vector3 targetPosition)
        {

            eventQueue.Pause();

            physbody.velocity = Vector3.zero;

            Vector3 dashDir = (targetPosition - instance.transform.position).normalized;
            float dist;
            while ((dist = Vector3.Distance(targetPosition, instance.transform.position)) > 0f)
            {

                float dashDistance = (insaneMode ? 1.2f : 1f) * self.movespeed.Value * 4 * Time.deltaTime;

                if (dist < dashDistance)
                {
                    instance.transform.position = targetPosition;
                    break;
                }

                instance.transform.position += dashDir * (dashDistance);
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

                Vector3 oldPosVector = instance.transform.position - center;
                Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

                instance.StartCoroutine(Dashing(rot * oldPosVector));
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
            });
        }

        public static AISequence PlayerLock(bool enableLock = true)
        {
            return new AISequence(() =>
            {
                if (enableLock)
                {
                    playerLockPosition = player.transform.position;
                }
                isPlayerLocked = enableLock;
            });
        }
    }
}