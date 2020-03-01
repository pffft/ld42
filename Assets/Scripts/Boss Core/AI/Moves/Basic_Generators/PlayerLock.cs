using AI;
using UnityEngine;

namespace Moves.Basic
{
    public class PlayerLock : InternalMove
    {
        // Used for the "PlayerLock" move. Keeps track of the current player position
        // for events and sequences that need a slightly out of date version.
        private static Vector3 playerLockPosition;
        private static bool isPlayerLocked;

        public static Constants.ProxyVector3 _delayed_player_position = new Constants.ProxyVector3(() => {
            return isPlayerLocked ? playerLockPosition : Constants.Positions.PLAYER_POSITION.GetValue();
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
