using AI;
using Projectiles;
using BossCore;

using static BossController;
using static AI.AISequence;

namespace Moves.Basic
{
    public class Shoot_Wave_Middle_Gap : Move
    {
        public Shoot_Wave_Middle_Gap()
        {
            Description = "Shoots two 60 degree waves with a 45 degree gap in the middle.";
            Difficulty = 3f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                Merge(
                    new ShootArc(150, 22.5f, 22.5f + 60f),
                    new ShootArc(150, -22.5f, -22.5f - 60f)
                ),
                new Pause(1f)
            );
        }

    }
}
