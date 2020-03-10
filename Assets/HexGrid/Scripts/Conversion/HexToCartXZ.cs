using UnityEngine;

namespace Hex.Converters
{
    public class HexToCartXZ : ISpaceConverter<VectorHex, Vector3>
    {
        public HexGrid Grid { get; }
        private CartXYToXZ cartXYToXZ;

        public HexToCartXZ(HexGrid grid)
        {
            Grid = grid;
            cartXYToXZ = new CartXYToXZ();
        }

        public VectorHex convertFrom(Vector3 position) => Grid.CartToHex(cartXYToXZ.convertFrom(position));

        public Vector3 convertTo(VectorHex position) => cartXYToXZ.convertTo(Grid.HexToCart(position));
    }
}
