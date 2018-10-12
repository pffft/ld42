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
    public class Tutorial3 : IMoveDictionary
    {
        /// <summary>
        /// Launches a 90 degree wide AOE attack at the player.
        /// </summary>
        public static Move AOE_90;

        /// <summary>
        /// Launches a 120 degree wide AOE attack at the player.
        /// </summary>
        public static Move AOE_120;

        /// <summary>
        /// Launches a 360 degree wide AOE attack.
        /// </summary>
        public static Move AOE_360;

        public void Load() {
            AOE_90 = new Move(
                1.5f,
                "AOE_90",
                "Launches a 90 degree wide AOE attack at the player.",
                new AISequence(
                    Teleport().Wait(0.25f),
                    Basic.AOE_90.Wait(1.5f)
                )
            );

            AOE_120 = new Move(
                1.5f,
                "AOE_120",
                "Launches a 120 degree wide AOE attack at the player.",
                new AISequence(
                    Teleport().Wait(0.25f),
                    Basic.AOE_120.Wait(1.5f)
                )
            );

            AOE_360 = new Move(
                1.5f,
                "AOE_360",
                "Launches a 360 degree wide AOE attack.",
                new AISequence(
                    Teleport().Wait(0.25f),
                    Basic.AOE_360.Wait(1.5f)
                )
            );
        }
    }
}