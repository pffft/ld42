using AI;
using Projectiles;
using static BossController;

using Constants;

namespace Moves.Basic
{
    public class ShootWall : InternalMove
    {
        public ShootWall(float angleOffset) : base
        (
            Merge(
                new ShootArc(100, angleOffset + -60, angleOffset + -60 + 28, new ProjectileData { Speed = Speed.SLOW }),
                new ShootArc(100, angleOffset + 20, angleOffset + 60, new ProjectileData { Speed = Speed.SLOW })
            )
        )
        {
            Description = "Shot a wall with offset " + angleOffset + " at the player.";
        }
    }
}
