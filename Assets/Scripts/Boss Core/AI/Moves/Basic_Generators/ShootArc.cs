using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class ShootArc : InternalMove
    {

        public ShootArc() : this(50, 0, 360, Projectile.DEFAULT) { }

        public ShootArc(Projectile skeleton) : this(50, 0, 360, skeleton) { }

        public ShootArc(int density, float from, float to, Projectile skeleton = null) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                // Ensure that "from" is always less than "to".
                if (to < from)
                {
                    float temp = from;
                    from = to;
                    to = temp;
                }

                float step = 360f / density;
                Projectile clone = skeleton ?? new Projectile { Size = Size.MEDIUM };
                for (float i = from; i <= to; i += step)
                {
                    Projectile newStruc = clone.Clone();
                    newStruc.AngleOffset = newStruc.AngleOffset + i;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot an arc (density=" + density + ", from=" + from + ", to=" + to + ") of " +
                (skeleton == Projectile.DEFAULT ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
