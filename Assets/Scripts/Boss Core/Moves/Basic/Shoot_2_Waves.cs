using AI;
using Projectiles;
using BossCore;

using static BossController;

namespace Moves.Basic
{
    public class Shoot_2_Waves : AISequence
    {
        public Shoot_2_Waves() : base
        (
            new Teleport().Wait(0.5f),
            new ShootArc(100, -45f, 45f, new Projectile().AngleOffset(-2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            new ShootArc(100, -45f, 45f, new Projectile().AngleOffset(2.5f).Size(Size.MEDIUM).Speed(Speed.VERY_FAST)),
            new Pause(1f)
        )
        { 
            Description = "Shoots 2 90 waves as one block, encouraging dodging through them.";
            Difficulty = 4f;
        }
    }
}
