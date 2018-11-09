using System.Collections.Generic;
using AI;
using Constants;
using Moves.Basic;
using Projectiles;
using UnityEngine;


namespace Moves.Test
{
    public class Pincer : Move
    {
        public Pincer(float offset = 0f, Speed speed = Speed.SNIPE) {
            float curveAmount =
                -4f * // base
                (float)speed * // turning radius is tighter when we go faster
                (20f / (GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude) * // contribution due to distance from player
                Mathf.Sqrt(2) * Mathf.Sin(Mathf.Deg2Rad * offset); // contribution due to initial firing offset

            float maxTime = Mathf.Deg2Rad * offset / Mathf.Sin(Mathf.Deg2Rad * offset) * ((GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude / 50f);
            maxTime += 0.1f;

            Description = "Shoots two projectiles with an offset of +/-" + offset + " degrees with speed " + speed;
            Sequence = Merge(
                new Shoot1(new ProjectileCurving(curveAmount, false) { Speed = Speed.SNIPE, AngleOffset = offset, MaxTime = maxTime }),
                new Shoot1(new ProjectileCurving(-curveAmount, false) { Speed = Speed.SNIPE, AngleOffset = -offset, MaxTime = maxTime })
            );
        }
   
    }
}
