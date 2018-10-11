using AI;
using AOEs;
using BossCore;
using Projectiles;
using System.Collections.Generic;

using static AI.AISequence;
using static AI.SequenceGenerators;
using static BossController;

namespace Moves
{
    public static class Tutorial2 {

        public static AISequence FORCE_BLOCK = new AISequence(
            5f,
            Teleport().Wait(0.25f),
            ShootArc(100, -90, 90, Projectile.New(self).Size(Size.TINY)).Wait(0.1f).Times(10),
            Pause(4f)
        );
    }
}