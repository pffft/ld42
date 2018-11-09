using AI;
using Projectiles;
using UnityEngine;
using static BossController;

namespace Moves.Basic
{
    public class MoveCamera : InternalMove
    {
        public MoveCamera(bool isFollow, Vector3? targetPosition = null) : base
        (
            () =>
            {
                CameraController.GetInstance().IsFollowing = isFollow;

                if (targetPosition.HasValue)
                {
                    CameraController.GetInstance().Goto(targetPosition.Value, 1);
                }
            }
        )
        {
            Description = "Moved the camera to " + targetPosition + ". Camera is now " + (isFollow ? "" : "not") + " following.";
        }
    }
}
