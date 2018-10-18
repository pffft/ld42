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

        public override float Difficulty => 1f;

        public AOE_360() : base(new ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f))) { }
    }
}
