using UnityEngine;

namespace Hex.Converters
{
    public class CartXYToXZ : ISpaceConverter<Vector2, Vector3>
    {
        public Vector2 convertFrom(Vector3 position) => new Vector2(position.x, position.z);

        public Vector3 convertTo(Vector2 position) => new Vector3(position.x, 0f, position.y);
    }
}
