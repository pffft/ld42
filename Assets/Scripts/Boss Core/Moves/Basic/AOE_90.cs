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
            new ShootAOE(AOE.New(self).On(-45, 45).FixedWidth(3f))
        ) 
        {
            Description = "Shoots a 90 degree wide AOE at the player.";
            Difficulty = 1f;
        }
    }
}
