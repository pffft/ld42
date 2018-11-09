using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot3 : InternalMove
    {

        public Shoot3() : this(Projectile.DEFAULT) { }

        public Shoot3(Projectile skeleton) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                for (int i = 0; i < 3; i++)
                {
                    Projectile newStruc = skeleton.Clone();
                    newStruc.AngleOffset = -30 + (30 * i) + newStruc.AngleOffset;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot three " + (skeleton == Projectile.DEFAULT ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
