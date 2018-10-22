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
                Glare();
                skeleton = skeleton != null ? skeleton.CloneWithCallbacks() : AOE.New(self);
                skeleton.Create(); 
            }
        )
        {
            Description = "Shot " + skeleton;
        }
    }
}
