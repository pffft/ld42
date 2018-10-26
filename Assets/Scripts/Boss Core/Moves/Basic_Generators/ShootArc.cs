using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class ShootArc : AISequence
    {

        public ShootArc(int density = 50, float from = 0, float to = 360, Projectile skeleton = null) : base
        (
            () =>
            {
                //long start = System.DateTime.Now.Ticks;
                Glare();

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
                //long time = System.DateTime.Now.Ticks - start;
                //UnityEngine.Debug.Log("Time taken : " + (time / 10000f / 1000f));
            }
        )
        {
            Description = "Shot an arc (density=" + density + ", from=" + from + ", to=" + to + ") of " +
              (skeleton == null ? "default projectiles at the player." : skeleton + ".");
        }
    }
}
