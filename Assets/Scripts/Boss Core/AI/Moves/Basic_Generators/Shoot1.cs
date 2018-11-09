using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot1 : InternalMove
    {

        public Shoot1() : this(Projectile.DEFAULT) { }

        public Shoot1(Projectile skeleton) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                Projectile newStruc = skeleton.Clone();
                newStruc.Create();
            }
        )
        {
            Description = "Shot one " + (skeleton == Projectile.DEFAULT ? "default projectile at the player." : skeleton + ".");
        }
    }
}
