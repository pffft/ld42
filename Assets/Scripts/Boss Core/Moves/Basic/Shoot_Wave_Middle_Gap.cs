using AI;
using Projectiles;
using BossCore;

using static BossController;
using static AI.AISequence;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class Shoot_Wave_Middle_Gap : Move
    {
        public override string GetDescription()
        {
            return "Shoots two 60 degree waves with a 45 degree gap in the middle.";
        }

        public override float GetDifficulty()
        {
            return 3f;
        }

        public override float GetBeforeDelay()
        {
            return 0.5f;
        }

        public override float GetAfterDelay()
        {
            return 1f;
        }

        public override AISequence GetSequence()
        {
            return Merge(
                ShootArc(150, 22.5f, 22.5f + 60f),
                ShootArc(150, -22.5f, -22.5f - 60f)
            );
        }
    }
}
