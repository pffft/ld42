using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot3 : InternalMove
    {

        public Shoot3(Projectile skeleton = null) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                for (int i = 0; i < 3; i++)
                {
                    Projectile newStruc = skeleton != null ? skeleton.Clone() : new Projectile();
                    newStruc.AngleOffset = -30 + (30 * i) + newStruc.AngleOffset;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot three " + (skeleton == null ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
