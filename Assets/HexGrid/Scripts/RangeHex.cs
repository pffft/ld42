using System;
using System.Collections;
using System.Collections.Generic;

namespace Hex
{
    /// <summary>
    /// Hexagonal equivelent of a Rect; represents an area in Hexagonal space.
    /// An implementation of https://www.redblobgames.com/grids/hexagons/.
    /// </summary>
    public struct RangeHex : IEnumerable<VectorHex>, ICloneable
    {
        public VectorHex Center { get; }
        public int Radius { get; }

        public int Area => (3 * Radius * Radius) + (3 * Radius) + 1;

        public static VectorHex[] IntersectionOf(params RangeHex[] ranges)
        {
            if (ranges.Length == 0)
            {
                return new VectorHex[0];
            }

            HashSet<VectorHex> hexSet = new HashSet<VectorHex>(ranges[0]);
            for (int i = 1; i < ranges.Length; i++)
            {
                hexSet.IntersectWith(ranges[i]);
            }

            VectorHex[] hexes = new VectorHex[hexSet.Count];
            hexSet.CopyTo(hexes);
            return hexes;
        }

        public static VectorHex[] UnionOf(params RangeHex[] ranges)
        {
            if (ranges.Length == 0)
            {
                return new VectorHex[0];
            }

            HashSet<VectorHex> hexSet = new HashSet<VectorHex>(ranges[0]);
            for (int i = 1; i < ranges.Length; i++)
            {
                hexSet.UnionWith(ranges[i]);
            }

            VectorHex[] hexes = new VectorHex[hexSet.Count];
            hexSet.CopyTo(hexes);
            return hexes;
        }

        public RangeHex(VectorHex center, int radius)
        {
            Center = center;
            Radius = Math.Abs(radius);
        }

        /// <summary>
        /// Get all the hex coordinates within "Radius" of "Center".
        /// Results start with lowest Q and R, and end with highest.
        /// </summary>
        /// <returns></returns>
        public VectorHex[] GetAll()
        {
            VectorHex[] hexes = new VectorHex[Area];
            int count = 0;
            for (int q = -Radius; q <= Radius; q++)
            {
                int rUpper = Math.Min(Radius, -q + Radius);
                for (int r = Math.Max(-Radius, -q - Radius); r <= rUpper; r++)
                {
                    hexes[count++] = Center + new VectorHex(q, r);
                }
            }
            return hexes;
        }

        /// <summary>
        /// Get all the hex coordinates exactly "Radius" units from "Center".
        /// Starts at lowest R and circles clockwise.
        /// </summary>
        /// <returns></returns>
        public VectorHex[] GetRing() => GetRing(Radius);

        public VectorHex[] GetRing(int radius)
        {
            if (radius == 0)
            {
                return new VectorHex[] { Center };
            }

            VectorHex[] hexes = new VectorHex[6 * radius];
            int count = 0;
            VectorHex hex = Center + (VectorHex.directions[4] * radius);
            for (int side = 0; side < 6; side++)
            {
                for (int length = 0; length < radius; length++)
                {
                    hexes[count++] = hex;
                    hex = hex + VectorHex.directions[side];
                }
            }
            return hexes;
        }

        /// <summary>
        /// Get all the hex coordinates within "Radius" of "Center".
        /// Results start at center and spiral outward.
        /// </summary>
        /// <returns></returns>
        public VectorHex[] GetAllInSpiral()
        {
            VectorHex[] hexes = new VectorHex[Area];
            int hexCount = 0;
            for (int i = 0; i <= Radius; i++)
            {
                VectorHex[] ring = GetRing(i);
                ring.CopyTo(hexes, hexCount);
                hexCount += ring.Length;
            }
            return hexes;
        }

        public IEnumerator<VectorHex> GetEnumerator()
        {
            for (int q = -Radius; q <= Radius; q++)
            {
                int rUpper = Math.Min(Radius, -q + Radius);
                for (int r = Math.Max(-Radius, -q - Radius); r <= rUpper; r++)
                {
                    yield return Center + new VectorHex(q, r);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetAll().GetEnumerator();

        public object Clone() => new RangeHex(Center, Radius);

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(RangeHex))
            {
                RangeHex other = (RangeHex)obj;
                return (Center == other.Center) && (Radius == other.Radius);
            }
            return false;
        }

        public override int GetHashCode() => new { Center, Radius }.GetHashCode();

        public override string ToString() => $"RangeHex({Center}, {Radius})";
    }
}
