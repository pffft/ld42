using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic2
{
    public class Shoot1 : Move2
    {
        private readonly Projectile skeleton;

        public Shoot1(Projectile skeleton = null) {
            this.skeleton = skeleton == null ? skeleton.Clone() : Projectile.New(self);
        }

        public override string GetDescription()
        {
            return "Shot a single " + (skeleton == null ? "default projectile at the player." : skeleton + ".");
        }

        public override float GetDifficulty()
        {
            return 1f;
        }

        public override AISequence GetSequence()
        {
            return new AISequence(() =>
            {
                Glare();
                skeleton.Create();
            });
        }
    }
}
