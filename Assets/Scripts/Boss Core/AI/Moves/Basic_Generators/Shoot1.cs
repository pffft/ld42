using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot1 : InternalMove
    {

        public Shoot1() : this(ProjectileData.DEFAULT) { }

        public Shoot1(ProjectileData skeleton) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                ProjectileData newStruc = skeleton.Clone();
                newStruc.Create();
            }
        )
        {
            Description = "Shot one " + (skeleton == ProjectileData.DEFAULT ? "default projectile at the player." : skeleton + ".");
        }
    }
}
