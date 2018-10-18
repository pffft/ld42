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

        public override float Difficulty => 1f;

        public AOE_120() : base(new ShootAOE(AOE.New(self).On(-60, 60).FixedWidth(3f))) { }
    }
}
