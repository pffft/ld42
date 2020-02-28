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
    public class Double_Laser_Sweep : Move
    {
        public Double_Laser_Sweep()
        {
            Description = "Sweeps two lasers back and forth.";
            Difficulty = 6.5f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                new PlayerLock(true),
                Merge(
                    new Laser().Wait(1f),
                    new Laser(90, -90, 5, -90).Wait(1f)
                ),
                new Pause(1f),
                Merge(
                    new Laser().Wait(1f),
                    new Laser(90, -90, 5, -90).Wait(1f)
                ),
                new PlayerLock(false),
                new Pause(1f)
            );
        }

    }
}
