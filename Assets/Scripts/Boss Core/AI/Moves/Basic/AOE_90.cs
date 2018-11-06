using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_90 : Move
    {
        public AOE_90()
        {
            Description = "Shoots a 90 degree wide AOE at the player.";
            Difficulty = 1f;
            Sequence = new ShootAOE(new AOE { FixedWidth = 3f }.On(-45, 45));
        }
    }
}
