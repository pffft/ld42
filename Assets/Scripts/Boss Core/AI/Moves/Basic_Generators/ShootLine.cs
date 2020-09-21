﻿using AI;
using Projectiles;
using UnityEngine;

using Constants;

namespace Moves.Basic
{
    public class ShootLine : InternalMove
    {
        public ShootLine(int amount = 50, float width = 75f, Vector3? target = null, Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM) : base
        (
            () =>
            {

                Vector3 targetPos = target.HasValue ? target.Value - GameManager.Boss.transform.position : GameManager.Player.transform.position - GameManager.Boss.transform.position;
                Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * targetPos).normalized;

                for (int i = 0; i < amount; i++)
                {
                    Vector3 spawn = GameManager.Boss.transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
                    new ProjectileData()
                    {
                        Start = spawn,
                        Target = spawn + targetPos,
                        Size = size
                    }.Create();
                }
            }
        )
        {
            Description = "Shot a line (amount=" + amount + ", width=" + width + ") at " + (target == null ? " the player" : target.ToString()) +
               " with speed " + speed + " and size " + size + ".";
        }
    }
}