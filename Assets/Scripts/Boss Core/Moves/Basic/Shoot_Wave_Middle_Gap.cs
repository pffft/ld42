using AI;
using Projectiles;
using BossCore;

using static BossController;
using static AI.AISequence;
using static AI.SequenceGenerators;

namespace Moves.Basic
{
    public class Shoot_Wave_Middle_Gap : AISequence
    {

        public override string Description => "Shoots two 60 degree waves with a 45 degree gap in the middle.";

        public override float Difficulty => 3f;

        public Shoot_Wave_Middle_Gap() : base
        (
            Teleport().Wait(0.5f),
            Merge(
                ShootArc(150, 22.5f, 22.5f + 60f),
                ShootArc(150, -22.5f, -22.5f - 60f)
            ),
            Pause(1f)
        )
        { }
    }
}
