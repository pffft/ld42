using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot1 : AISequence
    {
        public Shoot1(Projectile skeleton = null) : base
        (
            () =>
            {
                Glare();

                Projectile newStruc = skeleton != null ? skeleton.Clone() : new Projectile();
                UnityEngine.Debug.Log("Shoot1 called. Struc is " + newStruc.GetType());
                newStruc.Create();
            }
        )
        {
            Description = "Shot one " + (skeleton == null ? "default projectile at the player." : skeleton + ".");
        }
    }
}
