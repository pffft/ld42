using UnityEngine;

namespace Hex
{
    public class GridPlane
    {
        public Vector3 Normal { get; }
        private Quaternion rotationFrom2d;
        private Quaternion rotationTo2d;

        public GridPlane(Vector3 normal)
        {
            Normal = normal.normalized;
            rotationFrom2d = Quaternion.FromToRotation(Vector3.forward, Normal);
            rotationTo2d = Quaternion.FromToRotation(Normal, Vector3.forward);
        }

        public Vector3 this[Vector2 position] => Get3dPositionAt(position);

        /// <summary>
        /// Get the worldspace position on the plane that corresponds to
        /// the given Vector2.
        /// </summary>
        /// <example>
        /// Given a GridPlane defined by the normal vector (0, 0, 1), the position (1, 1) in 2D space
        /// is equivelent to (1, 0, 1) in GridPlane space
        /// </example>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 Get3dPositionAt(Vector2 position)
        {
            return rotationFrom2d * position;
        }

        public Vector2 Get2dPositionAt(Vector3 position)
        {
            return rotationTo2d * position;
        }
    }
}
