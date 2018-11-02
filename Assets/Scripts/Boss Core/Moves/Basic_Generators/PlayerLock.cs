using AI;
using UnityEngine;

namespace Moves.Basic
{
    public class PlayerLock : AISequence
    {
        // Used for the "PlayerLock" move. Keeps track of the current player position
        // for events and sequences that need a slightly out of date version.
        private static Vector3 playerLockPosition;
        private static bool isPlayerLocked;

        public static BossCore.ProxyVector3 _delayed_player_position = new BossCore.ProxyVector3(() => {
            return isPlayerLocked ? playerLockPosition : PLAYER_POSITION.GetValue();
        });

        public PlayerLock(bool enableLock = true) : base
        (
            () =>
            {
                if (enableLock) {
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
