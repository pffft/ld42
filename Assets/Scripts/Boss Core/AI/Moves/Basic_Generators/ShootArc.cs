using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class ShootArc : InternalMove
    {

        public ShootArc() : this(50, 0, 360, ProjectileData.DEFAULT) { }

        public ShootArc(ProjectileData skeleton) : this(50, 0, 360, skeleton) { }

        public ShootArc(int density, float from, float to, ProjectileData skeleton = null) : base
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
                ProjectileData clone = skeleton ?? new ProjectileData { Size = Size.MEDIUM };
                for (float i = from; i <= to; i += step)
                {
                    ProjectileData newStruc = clone.Clone();
                    newStruc.AngleOffset = newStruc.AngleOffset + i;
                    newStruc.Create();
                }
            }
        )
        {
            Description = "Shot an arc (density=" + density + ", from=" + from + ", to=" + to + ") of " +
                (skeleton == ProjectileData.DEFAULT ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
