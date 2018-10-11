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
    public static class Tutorial3
    {
        /*
         * Launches a 90 degree wide AOE attack at the player.
         */
        public static AISequence AOE_90 = new AISequence(
            1.5f,
            Teleport().Wait(0.25f),
            Basic.AOE_90.Wait(1.5f)
        );

        /*
         * Launches a 120 degree wide AOE attack at the player.
         */
        public static AISequence AOE_120 = new AISequence(
            1.5f,
            Teleport().Wait(0.25f),
            Basic.AOE_120.Wait(1.5f)
        );

        /*
         * Launches a 360 degree wide AOE attack.
         */
        public static AISequence AOE_360 = new AISequence(
            1.5f,
            Teleport().Wait(0.25f),
            Basic.AOE_360.Wait(1.5f)
        );
    }
}