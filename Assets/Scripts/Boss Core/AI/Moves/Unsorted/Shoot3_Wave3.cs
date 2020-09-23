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
    public class Shoot3_Wave3 : Move
    {
        public Shoot3_Wave3()
        {
            Description = "40 basic bullets, with a 360 wave at the start, middle, and end.";
            Difficulty = 3f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                new AOE_360(),
                new Shoot3().Wait(0.1f).Times(20),
                new AOE_360(),
                new Shoot3().Wait(0.1f).Times(20),
                new AOE_360().Wait(0.5f)
            );
        }
    }
}
