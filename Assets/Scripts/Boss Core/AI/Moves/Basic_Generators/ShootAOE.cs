using AI;
using AOEs;
using BossCore;

using static BossController;

namespace Moves.Basic
{
    public class ShootAOE : InternalMove
    {
        // TODO add default AOEs
        public ShootAOE() : this(null) { }

        public ShootAOE(AOE skeleton) : base
        (
            () => 
            {
                GameManager.Boss.Glare();
                skeleton = skeleton != null ? skeleton.Clone() : new AOE();
                skeleton.Create(); 
            }
        )
        {
            Description = "Shot " + skeleton;
        }
    }
}
