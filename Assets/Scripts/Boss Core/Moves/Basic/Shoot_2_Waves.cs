using AI;
using Projectiles;
using BossCore;

using static BossController;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class Shoot_2_Waves : Move
    {
        public override string GetDescription()
        {
            return "Shoots 2 90 waves as one block, encouraging dodging through them.";
        }

        public override float GetDifficulty()
        {
            return 4f;
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
            return new AISequence(
                ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
                ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST))
            );
        }
    }
}
