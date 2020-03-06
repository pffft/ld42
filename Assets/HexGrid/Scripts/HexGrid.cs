using System;
using UnityEngine;

namespace Hex
{
    /// <summary>
    /// Defines a Hexagonal space for conversion to and from Cartesian space.
    /// An implementation of https://www.redblobgames.com/grids/hexagons/.
    /// </summary>
    public class HexGrid
    {
        /// <summary>
        /// Outer radius of an individual hex (the distance of a vertex from the center)
        /// </summary>
        public float CellRadius { get; }

        /// <summary>
        /// Width of an individual hex
        /// </summary>
        public float CellWidth { get; }

        /// <summary>
        /// Height of an individual hex
        /// </summary>
        public float CellHeight { get; }

        private readonly Vector2 qBasisVecHtC;
        private readonly Vector2 rBasisVecHtC;
        private readonly Vector2 qBasisVecCtH;
        private readonly Vector2 rBasisVecCtH;

        public HexGrid() : this(1) { }
        public HexGrid(float hexRadius)
        {
            float sqrt3 = (float)Math.Sqrt(3f);
            CellRadius = hexRadius;
            CellWidth = 2 * hexRadius;
            CellHeight = sqrt3 * hexRadius;

            qBasisVecHtC = hexRadius * new Vector2(1.5f, 0f);
            rBasisVecHtC = hexRadius * new Vector2(sqrt3 / 2f, sqrt3);

            qBasisVecCtH = new Vector2(2f / 3f, 0f) / hexRadius;
            rBasisVecCtH = new Vector2(-1f / 3f, sqrt3 / 3f) / hexRadius;
        }

        public Vector2 HexToCart(VectorHex position)
        {
            return new Vector2(
                qBasisVecHtC.x * position.q,
                rBasisVecHtC.x * position.q + rBasisVecHtC.y * position.r);
        }

        public VectorHexF CartToHexF(Vector2 position)
        {
            return new VectorHexF(
                qBasisVecCtH.x * position.x,
                rBasisVecCtH.x * position.x + rBasisVecCtH.y * position.y);
        }

        public VectorHex CartToHex(Vector2 position)
        {
            return (VectorHex)CartToHexF(position);
        }

        public Vector2 Normalize(Vector2 position)
        {
            return HexToCart(CartToHex(position));
        }
    }
}
