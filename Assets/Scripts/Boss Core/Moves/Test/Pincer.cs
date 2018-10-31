using System.Collections.Generic;
using AI;
using BossCore;
using Moves.Basic;
using Projectiles;
using UnityEngine;

namespace Moves.Test
{
    public class Pincer : AISequence
    {
        public Pincer(float offset=0f, Speed speed=Speed.SNIPE) : base
        (
            () => {
                //Debug.Log("Pincer sees player at: " + GameManager.Player.transform.position);
                Debug.Log("Pincer called");
                List<AISequence> sequences = new List<AISequence>();
            ¸
                float curveAmount =
                            -4f * // base
                            (float)speed * // turning radius is tighter when we go faster
                            (20f / (GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude) * // contribution due to distance from player
                            Mathf.Sqrt(2) * Mathf.Sin(Mathf.Deg2Rad * offset); // contribution due to initial firing offset

                float maxTime = Mathf.Deg2Rad * offset / Mathf.Sin(Mathf.Deg2Rad * offset) * ((GameManager.Boss.transform.position - GameManager.Player.transform.position).magnitude / 50f);
                maxTime += 0.1f;

                sequences.Add(new Shoot1(new ProjectileCurving(curveAmount, false) { Speed = Speed.SNIPE, AngleOffset = offset, MaxTime = maxTime }));
                sequences.Add(new Shoot1(new ProjectileCurving(-curveAmount, false) { Speed = Speed.SNIPE, AngleOffset = -offset, MaxTime = maxTime }));

                return sequences.ToArray();
            }

        )
        {
            Debug.Log("Pincer created");
            Description = "Shoots two projectiles with an offset of +/-" + offset + " degrees with speed " + speed;
        }
    }
}
