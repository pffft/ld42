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
    public class Laser_Sweep_With_AOE : AISequence
    {
        public Laser_Sweep_With_AOE() : base
        (
            new Teleport().Wait(0.5f),
            new PlayerLock(true),
            Merge(
                new Laser().Wait(1f),
                new Pause(1f).Then(new ShootAOE(new AOE { FixedWidth = 3f, OuterSpeed = Speed.FAST }.On(-60, 60)))
            ),
            new Pause(1f),
            Merge(
                new Laser(90, -90, 5, -90).Wait(1f),
                new Pause(1f).Then(new ShootAOE(new AOE { FixedWidth = 3f, OuterSpeed = Speed.FAST }.On(-60, 60)))
            ),
            new PlayerLock(false),
            new Pause(1f)
        )
        {
            Description = "Sweeps a laser back and forth, with two AOE attacks in between.";
            Difficulty = 5.5f;
        }

    }
}
