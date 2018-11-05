using AI;

namespace Moves.Basic
{
    public class Pause : AISequence
    {
        public Pause(float duration) : base
        (
            new AIEvent[] { new AIEvent(duration, () => { }) }
        )
        {
            Description = "Waiting for " + duration + " seconds.";
        }
    }
}
