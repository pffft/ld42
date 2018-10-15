using AI;
using AOEs;
using BossCore;

using static BossController;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class Aoe_360 : Move
    {
        public override string GetDescription()
        {
            return "Shoots a 360 degree AOE attack with a width of 3.";
        }

        public override float GetDifficulty()
        {
            return 1.5f;
        }

        public override AISequence GetSequence()
        {
            return new AISequence(ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f)));
        }
    }
}
