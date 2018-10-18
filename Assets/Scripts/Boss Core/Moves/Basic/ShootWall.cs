using AI;
using Projectiles;
using static BossController;

using BossCore;

namespace Moves.Basic
{
    public class ShootWall : AISequence
    {
        public override string Description => "Shot a wall with offset " + angleOffset + " at the player."

        public override float Difficulty => 1.5f;

        private readonly float angleOffset;

        public ShootWall(float angleOffset) : base
        (
            AISequence.Merge(
                new ShootArc(100, angleOffset + -60, angleOffset + -60 + 28, Projectile.New(self).Speed(Speed.SLOW)),
                new ShootArc(100, angleOffset + 20, angleOffset + 60, Projectile.New(self).Speed(Speed.SLOW))
            )
        )
        {
            this.angleOffset = angleOffset;
        }
    }
}
