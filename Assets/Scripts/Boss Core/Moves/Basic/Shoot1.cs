using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot1 : AISequence
    {
        public override string Description => "Shot one " + (skeleton == null ? "default projectile at the player." : skeleton + ".");

        public override float Difficulty => 1.5f;

        private readonly Projectile skeleton;

        public Shoot1(Projectile skeleton = null) : base
        (
            () =>
            {
                Glare();

                Projectile newStruc = skeleton != null ? skeleton.Clone() : Projectile.New(self);
                newStruc.Create();
            }
        )
        {
            this.skeleton = skeleton != null ? skeleton.Clone() : Projectile.New(self);
        }
    }
}
