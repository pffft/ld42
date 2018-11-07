using AI;

namespace Moves.Basic
{
    public class Pause : InternalMove
    {
        /// <summary>
        /// Creates a new AISequence that does nothing, but has a specified duration.
        /// </summary>
        /// <param name="duration">How long we should wait for.</param>
        public Pause(float duration) : base
        (
            new AIEvent(duration, () => { })
        )
        {
            Description = "Wait for " + duration + " seconds.";
        }
    }
}
