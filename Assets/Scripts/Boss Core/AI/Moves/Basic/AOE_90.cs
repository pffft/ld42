using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_90 : AISequence
    {
        public AOE_90() : base
        (
            new ShootAOE(new AOE { FixedWidth = 3f }.On(-45, 45))
        ) 
        {
            Description = "Shoots a 90 degree wide AOE at the player.";
            Difficulty = 1f;
        }
    }
}
