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
        public Laser(float startOffset=-90, float finalOffset=90, float width=5, float angularSpeed=90) : base
        (
            new ShootAOE(
                AOE.New(self)
                .On(0, width)
                .InnerScale(0f)
                .Scale(100f)
                .AngleOffset(startOffset)
                .Freeze()
                .RotationSpeed(angularSpeed)
                .MaxTime((finalOffset - startOffset) / angularSpeed)
                .OnDestroyOutOfBounds(AOECallbackDictionary.DONT_DESTROY_OOB)
            )
        ) 
        {
            Description = "A laser that sweeps from " + startOffset + " to " + finalOffset + " with a beam width of " + width + " and speed " + angularSpeed;
            Difficulty = 5f;
        }
    }
}
