﻿using System.Collections;
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
    public class Wave_Reverse_Target_Homing : Move
    {

        public Wave_Reverse_Target_Homing()
        {
            Description = "Fires a wave of projectiles that, on death, turn into homing projectiles aimed at the player.";
            Difficulty = 7f;
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
                        OnDestroyTimeout = CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER
                    }
                )
            ).Wait(2f);
        }
    }
}