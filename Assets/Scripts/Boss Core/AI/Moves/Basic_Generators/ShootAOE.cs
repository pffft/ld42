using AI;
using AOEs;
using Constants;

using static BossController;

namespace Moves.Basic
{
    public class ShootAOE : InternalMove
    {
        // TODO add default AOEs
        public ShootAOE() : this(null) { }

        public ShootAOE(AOEData skeleton) : base
        (
            () => 
            {
                GameManager.Boss.Glare();
                skeleton = skeleton != null ? skeleton.Clone() : new AOEData();
                skeleton.Create(); 
            }
        )
        {
            Description = "Shot " + skeleton;
        }
    }
}
