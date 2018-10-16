using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot3 : Move
    {
        private readonly Projectile skeleton;

        public Shoot3() : this(null) { }

        public Shoot3(Projectile skeleton = null)
        {
            this.skeleton = skeleton != null ? skeleton.Clone() : Projectile.New(self);
        }

        public override string GetDescription()
        {
            return "Shot three " + (skeleton == null ? "default projectiles at the player." : skeleton + ".");
        }

        public override float GetDifficulty()
        {
            return 1.5f;
        }

        public override bool DoesTeleportAtStart()
        {
            return false;
        }

        public override AISequence GetSequence()
        {
            return new AISequence(() =>
            {
                Glare();

                for (int i = 0; i < 3; i++)
                {
                    Projectile newStruc = skeleton.Clone();
                    newStruc.angleOffset = -30 + (30 * i) + newStruc.angleOffset;
                    newStruc.Create();
                }
            });
        }
    }
}
