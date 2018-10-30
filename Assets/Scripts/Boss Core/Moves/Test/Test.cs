using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using BossCore;
using static BossController;

namespace Moves.Test
{
    public class Test : AISequence
    {
        public Test() : base
        (
            /*
            new PlayerLock(true),
            new AISequence(() =>
            {
                List<AISequence> sequences = new List<AISequence>();

                for (int angle = 0; angle < 72; angle += 6)
                {
                    sequences.Add(new ShootWall(angleOffset: angle).Wait(0.1f));
                }

                for (int angle = 72; angle >= 0; angle -= 6)
                {
                    sequences.Add(new ShootWall(angleOffset: angle).Wait(0.1f));
                }
                return sequences.ToArray();
            }),
            new PlayerLock(false)
            */
            //For(6, i => new Shoot1(new Projectile().AngleOffset(i * 60)).Wait(0.25f)).Wait(0.5f)
            new ShootArc(500, 0, 360, new Projectile { MaxTime = 2f }).Wait(0.05f)
        )
        {
        }
    }
}
