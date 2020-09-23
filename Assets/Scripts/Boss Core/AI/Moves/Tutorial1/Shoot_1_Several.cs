﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

using Moves.Basic;

namespace Moves.Tutorial1
{
    public class Shoot_1_Several : Move
    {
        public Shoot_1_Several()
        {
            Description = "Shoots between 5 and 10 default projectiles at the player.";
            Difficulty = 2f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                GenerateRandom(ran => new Shoot1().Wait(0.1f).Times((int) (5 + (ran * 5)))),
                new Pause(1.5f)
            );
        }
    }
}
