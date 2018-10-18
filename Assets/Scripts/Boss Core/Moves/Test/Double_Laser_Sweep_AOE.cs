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
    public class Double_Laser_Sweep_AOE : AISequence
    {
        public Double_Laser_Sweep_AOE() : base
        (
            new Teleport().Wait(0.5f),
            new PlayerLock(true),
            Merge(
                new Laser().Wait(1f),
                new Laser(90, -90, 5, -90).Wait(1f),
                new ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f).Speed(Speed.FAST))
            ),
            new Pause(1f),
            Merge(
                new Laser().Wait(1f),
                new Laser(90, -90, 5, -90).Wait(1f),
                new ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f).Speed(Speed.FAST))
            ),
            new PlayerLock(false),
            new Pause(1f)
        )
        {
            Description = "Sweeps two lasers back and forth, with two AOE attacks in between.";
            Difficulty = 7f;
        }

    }
}
