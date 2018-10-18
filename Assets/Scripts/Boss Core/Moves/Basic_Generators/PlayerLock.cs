using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class PlayerLock : AISequence
    {
        public PlayerLock(bool enableLock = true) : base
        (
            () =>
            {
                if (enableLock)
                {
                    playerLockPosition = GameManager.Player.transform.position;
                }
                isPlayerLocked = enableLock;
            }
        )
        {
            Description = (enableLock ? "Locked onto" : "Unlocked from") + " the player.";
        }
    }
}
