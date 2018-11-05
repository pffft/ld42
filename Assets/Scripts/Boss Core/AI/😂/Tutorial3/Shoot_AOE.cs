using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using static BossController;
using Moves.Basic;

namespace Moves.Tutorial3
{
    public class Shoot_AOE : AISequence
    {

        public Shoot_AOE(int width) : base
        (
            new Teleport().Wait(0.25f),
            new ShootAOE(new AOE { FixedWidth = 3f }.On(-width / 2, width / 2)),
            new Pause(0.5f)
        )
        {
            Description = "Shoots an AOE " + width + " degrees wide at the player.";
            Difficulty = 1.5f;
        }
    }
}
