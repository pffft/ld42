using AI;
using AOEs;
using BossCore;

using static BossController;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class ShootAOE : Move
    {
        private readonly AOE skeleton;

        public ShootAOE() : this(null) {}

        public ShootAOE(AOE skeleton) {
            this.skeleton = skeleton != null ? skeleton.Clone() : AOE.New(self);
        }

        public override string GetDescription()
        {
            return "Shot " + skeleton;
        }

        public override float GetDifficulty()
        {
            return 1.5f;
        }

        public override float GetBeforeDelay()
        {
            return 0.5f;
        }

        public override float GetAfterDelay()
        {
            return 0.5f;
        }

        public override AISequence GetSequence()
        {
            return new AISequence(() => { Glare(); skeleton.Create(); });
        }
    }
}
