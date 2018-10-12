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
    public class Tutorial2 : IMoveDictionary {

        /// <summary>
        /// Fires 10 sets of tiny projectiles in a 180 degree arc, too dense to dash through.
        /// </summary>
        public static Move FORCE_BLOCK;

        public void Load() {
            FORCE_BLOCK = new Move(
                5f,
                "FORCE_BLOCK",
                "Fires 10 sets of tiny projectiles in a 180 degree arc, too dense to dash through.",
                new AISequence(
                    Teleport().Wait(0.25f),
                    ShootArc(100, -90, 90, Projectile.New(self).Size(Size.TINY)).Wait(0.1f).Times(10),
                    Pause(4f)
                )
            );
        }
    }
}