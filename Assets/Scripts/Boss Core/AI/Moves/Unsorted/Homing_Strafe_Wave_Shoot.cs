using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;

namespace Moves.Unsorted
{
    public class Homing_Strafe_Wave_Shoot : Move
    {

        public Homing_Strafe_Wave_Shoot() {
            Difficulty = 5.5f;
            Description = "Does a homing strafe, followed by two shoot_2_waves.";
            Sequence = new AISequence(
                new Teleport().Wait(0.2f),
                new ShootHomingStrafe(strafeAmount: 15).Wait(0.01f).Times(15).Wait(0.3f), // This is hard; adding wait is reasonable
                new Shoot_2_Waves().Times(2)
            );
        }
    }
}
