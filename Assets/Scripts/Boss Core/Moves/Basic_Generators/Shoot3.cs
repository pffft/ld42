using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot3 : AISequence
    {

        public Shoot3(Projectile skeleton = null) : base
        (
            () =>
            {
                Glare();

                for (int i = 0; i < 3; i++)
                {
                    Projectile newStruc = skeleton != null ? skeleton.Clone() : new Projectile();
                    newStruc.angleOffset = -30 + (30 * i) + newStruc.angleOffset;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot three " + (skeleton == null ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
