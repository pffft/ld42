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
            new ShootAOE(AOE.New(self).On(-width / 2, width / 2).FixedWidth(3f)),
            new Pause(0.5f)
        )
        {
            Description = "Shoots an AOE " + width + " degrees wide at the player.";
            Difficulty = 1.5f;
        }
    }
}
