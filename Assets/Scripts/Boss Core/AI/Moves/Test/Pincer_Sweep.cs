using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using Constants;
using static BossController;

namespace Moves.Test
{
    public class Pincer_Sweep : Move
    {
        public Pincer_Sweep()
        {
            Description = "Sweeps pincer projectiles from +150 to +0 degrees";
            Sequence = For(150, 0, -5, i => new Pincer(i, Speed.SNIPE).Wait(0.05f));
        }
    }
}
