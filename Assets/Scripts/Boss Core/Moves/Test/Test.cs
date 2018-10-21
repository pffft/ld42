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
        (() =>
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
        }
        )
        {

        }
    }
}
