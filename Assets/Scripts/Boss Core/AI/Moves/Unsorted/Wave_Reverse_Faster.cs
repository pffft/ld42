using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static World.Arena;

namespace Moves.Unsorted
{
    public class Wave_Reverse_Faster : Move
    {

        public Wave_Reverse_Faster()
        {
            Description = "Fires a wave of projectiles that, on death, reverse direction. These speed up until they hit max speed.";
            Difficulty = 5f;
            Sequence = For(
                50,
                i => new Shoot1
                (
                    new ProjectileHoming
                    {
                        Speed = Speed.FAST,
                        Size = Size.MEDIUM,
                        MaxTime = 1f,
                        AngleOffset = i * (360f / 50f),
                        OnDestroyTimeout = CallbackDictionary.REVERSE_FASTER
                    }
                )
            ).Wait(2f);
        }
    }
}
