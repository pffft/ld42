using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

using Moves.Basic;

namespace Moves.Tutorial1
{
    public class Shoot_Arc : AISequence
    {
        public Shoot_Arc(int width=70, bool dense=false) : base
        (
            new Teleport().Wait(0.25f),
            new ShootArc(dense ? 100 : 50, -width / 2, width / 2),
            new Pause(1.5f)
        )
        {
            Description = "Shoots a" + (dense ? " dense" : "n") + " arc, " + width + " degrees wide, at the player.";
            Difficulty = 2f;
        }
    }
}
