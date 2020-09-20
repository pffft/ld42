using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using Constants;
using Moves.Basic;

namespace Moves.Test
{
    public class Lightning_Arena : Move
    {
        public Lightning_Arena()
        { 
            Description = "Spawns lightning on the whole arena";
            Difficulty = 5f;
            Sequence = new AISequence(
                new Teleport(Constants.Positions.CENTER).Wait(0.25f),
                For(4, i => new Shoot1(new ProjectileLightning { AngleOffset = i * 90f }).Wait(0.1f)),
                Pause(1f)
            );
        }
    }
}
