using AI;
using AOEs;
using BossCore;

using static BossController;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class ShootAOE : AISequence
    {
        public override string Description => "Shot " + skeleton;

        private readonly AOE skeleton;

        public ShootAOE(AOE skeleton = null) : base(() => { 
            Glare();
            skeleton = skeleton != null ? skeleton.Clone() : AOE.New(self);
            skeleton.Create(); 
        })
        {
            this.skeleton = skeleton != null ? skeleton.Clone() : AOE.New(self);
        }

    }
}
