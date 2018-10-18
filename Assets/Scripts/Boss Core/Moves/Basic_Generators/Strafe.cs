using AI;
using Projectiles;
using static BossController;

using UnityEngine;

namespace Moves.Basic
{
    public class Strafe : AISequence
    {
        public Strafe(bool clockwise = true, float degrees = 10f, int speed = 25, Vector3 center = default(Vector3)) : base
        (
            () =>
            {
                self.movespeed.LockTo(speed);

                Vector3 oldPosVector = GameManager.Boss.transform.position - center;
                Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

                GameManager.Boss.StartCoroutine(Dash(rot * oldPosVector));
            }
        )
        {}
    }
}
