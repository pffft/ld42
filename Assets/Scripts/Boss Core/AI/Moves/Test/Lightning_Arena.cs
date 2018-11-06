using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

using Projectiles;
using static BossController;
using BossCore;
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
                new Teleport(World.Arena.CENTER).Wait(0.25f),
                new AISequence(() =>
                {
                    List<AISequence> sequences = new List<AISequence>();

                    for (int i = 0; i < 1; i++)
                    {
                        sequences.Add(new Shoot1(
                        new ProjectileLightning { AngleOffset = i * 90f }).Wait(0.1f));
                    }
                    return sequences.ToArray();
                }),
                Pause(1f)
            );
        }
    }
}
