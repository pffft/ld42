using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class ShootHomingStrafe : AISequence
    {
        public ShootHomingStrafe(bool clockwise = true, int strafeAmount = 5, int speed = 25) : base
        (
            new Strafe(clockwise, strafeAmount, speed),
            new Shoot1(Projectile.New(self).Size(Size.MEDIUM).Homing())
        )
        {
            Description = "Strafed " + strafeAmount + " degrees " + (clockwise ? "clockwise" : "counterclockwise") + " and shot a homing projectile.";
        }
    }
}
