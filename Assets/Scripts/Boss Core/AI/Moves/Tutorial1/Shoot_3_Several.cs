using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AI;

using Moves.Basic;

namespace Moves.Tutorial1
{
    public class Shoot_3_Several : Move
    {
        public Shoot_3_Several()
        {
            Description = "Shoots between 7 and 12 three-way projectiles at the player.";
            Difficulty = 3f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                // TODO add some way to include random values here
                new AISequence(() =>
                {
                    return new Shoot3().Wait(0.1f).Times(Random.Range(7, 12));
                }),
                new Pause(1.5f)
            );
        }
    }
}
