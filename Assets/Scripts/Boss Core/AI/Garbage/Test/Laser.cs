using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using static BossController;

using Moves.Basic;

namespace Moves.Test
{
    public class Laser : AISequence
    {
        public Laser(float startOffset = -90, float finalOffset = 90, float width = 5, float angularSpeed = 90) : base
        (
            new ShootAOE(
                new AOE
                {
                    InnerSpeed = Speed.FROZEN,
                    OuterSpeed = Speed.FROZEN,
                    InnerScale = 0f,
                    Scale = 100f,
                    AngleOffset = startOffset,
                    RotationSpeed = angularSpeed,
                    MaxTime = (finalOffset - startOffset) / angularSpeed,
                    OnDestroyOutOfBounds = AOECallbackDictionary.DONT_DESTROY_OOB
                }.On(0, width)
            )
        ) 
        {
            Description = "A laser that sweeps from " + startOffset + " to " + finalOffset + " with a beam width of " + width + " and speed " + angularSpeed;
            Difficulty = 5f;
        }
    }
}
