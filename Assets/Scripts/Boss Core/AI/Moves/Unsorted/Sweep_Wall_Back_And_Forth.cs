using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Unsorted 
{
    public class Sweep_Wall_Back_And_Forth : Move
    {
        public Sweep_Wall_Back_And_Forth()
        {
            Description = "Sweeps a wall clockwise, then counterclockwise.";
            Difficulty = 6f;
            Sequence = new AISequence(
                new PlayerLock(true),
                new Sweep_Wall(true),
                new Sweep_Wall(false),
                new PlayerLock(false)
            );
        }
    }
}
