using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;

namespace Moves.Unsorted
{
    public class Shoot_Hex_Curve : AISequence
    {
        public Shoot_Hex_Curve(bool clockwise=true, float offset=0f) : base
        (
            ForConcurrent(6, i => 
                new Shoot1(
                    new ProjectileCurving((float)Speed.MEDIUM * (clockwise ? 1 : -1) * 2f, true) { 
                        MaxTime = 5f, AngleOffset = offset + ((clockwise ? 1 : -1) * i * 60f)
                    }
                )
            )
        )
        {
            Description = "Shoots 6 curving projectiles " + (clockwise ? "clockwise" : "counterclockwise") + " from the boss's location.";
            Difficulty = 3f;
        }
    }
}
