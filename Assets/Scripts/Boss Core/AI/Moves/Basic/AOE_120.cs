using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_120 : Move
    {
        public AOE_120()
        {
            Description = "Shoots a 120 degree wide AOE at the player.";
            Difficulty = 1f;
            Sequence = new ShootAOE(new AOEData { FixedWidth = 3f }.On(-60, 60));
        }
    }
}
