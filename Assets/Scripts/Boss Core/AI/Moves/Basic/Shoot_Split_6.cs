using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using BossCore;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Unsorted
{
    public class Shoot_Split_6 : Move
    {
        public Shoot_Split_6()
        {
            Description = "Shoots a projectile that splits into 6 more projectiles.";
            Difficulty = 4f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                new Shoot1(
                    new ProjectileCurving(0f, false)
                    {
                        MaxTime = 0.25f,
                        Speed = Speed.VERY_FAST,
                        OnDestroyTimeout = CallbackDictionary.SPAWN_6
                    }
                ),
                new Pause(0.5f)
            );
        }
    }
}
