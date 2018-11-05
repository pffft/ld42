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
    public class Pincer_Sweep : AISequence
    {
        public Pincer_Sweep() : base
        (
            /*
            () => {
                List<AISequence> sequences = new List<AISequence>();
                for (int i = 150; i >= 0; i -= 5) {
                    sequences.Add(new Pincer(i).Wait(0.05f));
                }
                return sequences.ToArray();
            }
            */
            For(150, 0, -5, i => new Pincer(i).Wait(0.05f))
        )
        {
            Description = "Sweeps pincer projectiles from +150 to +0 degrees";
        }
    }
}
