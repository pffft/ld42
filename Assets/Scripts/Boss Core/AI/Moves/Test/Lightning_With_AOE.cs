using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using AOEs;
using Constants;
using Moves.Test;
using Moves.Basic;
using static BossController;

namespace Moves.Test
{
    public class Lightning_With_AOE : Move
    {
        public Lightning_With_AOE()
        { 
            Description = "Lightning with aoe";
            Difficulty = 6f;
            Sequence = new AISequence(
                new ShootAOE(new AOEData
                {
                    AngleOffset = (-25),
                    RotationSpeed = (15f),
                    FixedWidth = (10f),
                    OuterSpeed = (Speed.MEDIUM_SLOW)
                }
                     .On(-22.5f, 22.5f)
                     .On(90 - 22.5f, 90 + 22.5f)
                     .On(180 - 22.5f, 180 + 22.5f)
                     .On(270 - 22.5f, 270 + 22.5f)
                ).Wait(1.5f),
                new Lightning_Arena().Wait(0.5f),
                new ShootAOE(new AOEData
                {
                    AngleOffset = (-25),
                    RotationSpeed = (15f),
                    FixedWidth = (10f),
                    OuterSpeed = (Speed.MEDIUM_SLOW)
                }
                     .On(-22.5f, 22.5f)
                     .On(90 - 22.5f, 90 + 22.5f)
                     .On(180 - 22.5f, 180 + 22.5f)
                     .On(270 - 22.5f, 270 + 22.5f)
                ).Wait(1.5f),
                new Lightning_Arena().Wait(2.5f)
            );
        }
    }
}
