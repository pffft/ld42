using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_120 : AISequence
    {
        public AOE_120() : base
        (
            new ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f))
        ) 
        {
            Description = "Shoots a 120 degree wide AOE at the player.";
            Difficulty = 1f;
        }
    }
}
