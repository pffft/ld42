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
                    GameManager.Boss.playerLockPosition = GameManager.Player.transform.position;
                }
                GameManager.Boss.isPlayerLocked = enableLock;
            }
        )
        {
            Description = (enableLock ? "Locked onto" : "Unlocked from") + " the player.";
        }
    }
}
