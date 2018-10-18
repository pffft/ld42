using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class ShootArc : AISequence
    {
        public override string Description => "Shot an arc (density=" + density + ", from=" + from + ", to=" + to + ") of " +
            (skeleton == null ? "default projectiles at the player." : skeleton + ".");

        public override float Difficulty => 1.5f;

        private readonly int density;
        private readonly float from;
        private readonly float to;
        private readonly Projectile skeleton;

        public ShootArc(int density = 50, float from = 0, float to = 360, Projectile skeleton = null) : base
        (
            () =>
            {
                Glare();

                // Ensure that "from" is always less than "to".
                if (to < from)
                {
                    float temp = from;
                    from = to;
                    to = temp;
                }

                float step = 360f / density;
                Projectile clone = skeleton ?? Projectile.New(self).Size(Size.MEDIUM);
                for (float i = from; i <= to; i += step)
                {
                    Projectile newStruc = clone.Clone();
                    newStruc.AngleOffset(newStruc.angleOffset + i).Create();
                }
            }
        )
        {
            this.density = density;
            this.from = from;
            this.to = to;
            this.skeleton = skeleton ?? Projectile.New(self).Size(Size.MEDIUM);
        }
    }
}
