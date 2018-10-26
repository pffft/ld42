using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_360 : AISequence
    {
        public AOE_360() : base
        (
            new ShootAOE(new AOE { FixedWidth = 3f }.On(0, 360))
        )
        {
            Description = "Shoots a 360 degree wide AOE.";
            Difficulty = 1f;
        }
    }
}
