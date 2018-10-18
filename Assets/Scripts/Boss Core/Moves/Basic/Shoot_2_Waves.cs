using AI;
using Projectiles;
using BossCore;

using static BossController;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class Shoot_2_Waves : AISequence
    {
        public override string Description => "Shoots 2 90 waves as one block, encouraging dodging through them.";

        public override float Difficulty => 4f;

        public Shoot_2_Waves() : base
        (
            Teleport().Wait(0.5f),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            ShootArc(100, -45f, 45f, Projectile.New(self).AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            Pause(1f)
        )
        { }
    }
}
