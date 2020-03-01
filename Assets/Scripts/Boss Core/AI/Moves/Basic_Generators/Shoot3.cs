using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot3 : InternalMove
    {

        public Shoot3() : this(ProjectileData.DEFAULT) { }

        public Shoot3(ProjectileData skeleton) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                for (int i = 0; i < 3; i++)
                {
                    ProjectileData newStruc = skeleton.Clone();
                    newStruc.AngleOffset = -30 + (30 * i) + newStruc.AngleOffset;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot three " + (skeleton == ProjectileData.DEFAULT ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
