using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;
using static Constants.Positions;

namespace Moves.Unsorted
{
    public class Wave_Reverse : Move
    {

        public Wave_Reverse()
        {
            Description = "Fires a wave of projectiles that, on death, reverse direction.";
            Difficulty = 4f;
            Sequence = ForConcurrent(
                50,
                i => new Shoot1
                (
                    new ProjectileHoming
                    {
                        Speed = Speed.FAST,
                        Size = Size.MEDIUM,
                        MaxTime = 1f,
                        AngleOffset = i * (360f / 50f),
                        OnDestroyTimeout = CallbackDictionary.REVERSE
                    }
                )
            ).Wait(2f);
        }
    }
}
