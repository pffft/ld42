using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

using Moves.Basic;

namespace Moves.Tutorial1
{
    public class Shoot_1_Several : AISequence
    {
        public Shoot_1_Several() : base
        (
            new Teleport().Wait(0.25f),
            new AISequence(() =>
            {
                return new Shoot1().Wait(0.1f).Times(Random.Range(5, 10));
            }),
            new Pause(1.5f)
        )
        {
            Description = "Shoots between 5 and 10 default projectiles at the player.";
            Difficulty = 2f;
        }
    }
}
