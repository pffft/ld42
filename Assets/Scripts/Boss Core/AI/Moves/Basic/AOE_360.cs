using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class AOE_360 : Move
    {
        public AOE_360()
        {
            Description = "Shoots a 360 degree wide AOE.";
            Difficulty = 1f;
            Sequence = new ShootAOE(new AOEData { FixedWidth = 3f }.On(0, 360));
        }
    }
}
