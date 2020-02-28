using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using static BossController;
using Moves.Basic;

namespace Moves.Tutorial3
{
    public class Shoot_AOE : Move
    {

        public Shoot_AOE(int width)
        {
            Description = "Shoots an AOE " + width + " degrees wide at the player.";
            Difficulty = 1.5f;
            Sequence = new AISequence(
                new Teleport().Wait(0.25f),
                new ShootAOE(new AOEData { FixedWidth = 3f }.On(-width / 2, width / 2)),
                new Pause(1.5f)
            );
        }
    }
}
