using AI;
using AOEs;
using BossCore;

using static BossController;

namespace Moves.Basic
{
    public class ShootAOE : AISequence
    {
        public ShootAOE(AOE skeleton = null) : base
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
