using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
    /// <summary>
    /// An integer hexagonal coordinate or vector. 
    /// An implementation of https://www.redblobgames.com/grids/hexagons/.
    /// </summary>
    [Serializable]
    public struct VectorHex
    {
        /// <summary>
        /// Positive S.
        /// </summary>
        public static VectorHex pS => new VectorHex(1, 0);

        /// <summary>
        /// Positive R.
        /// </summary>
        public static VectorHex pR => new VectorHex(0, 1);

        /// <summary>
        /// Positive Q.
        /// </summary>
        public static VectorHex pQ => new VectorHex(-1, 1);

        /// <summary>
        /// Negative S.
        /// </summary>
        public static VectorHex nS => new VectorHex(-1, 0);

        /// <summary>
        /// Negative R.
        /// </summary>
        public static VectorHex nR => new VectorHex(0, -1);

        /// <summary>
        /// Negative Q.
        /// </summary>
        public static VectorHex nQ => new VectorHex(1, -1);

        public static VectorHex zero => new VectorHex(0, 0);

        public static VectorHex[] directions => new VectorHex[] { pS, pR, pQ, nS, nR, nQ };

        [field: SerializeField] public int q { get; }
        [field: SerializeField] public int r { get; }
        public int s => -q - r;
        public int magnitude => (Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2;

        public static bool operator ==(VectorHex left, VectorHex right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VectorHex left, VectorHex right)
        {
            return !left.Equals(right);
        }

        public static VectorHex operator +(VectorHex left, VectorHex right)
        {
            return new VectorHex(left.q + right.q, left.r + right.r);
        }

        public static VectorHex operator -(VectorHex left, VectorHex right)
        {
            return new VectorHex(left.q - right.q, left.r - right.r);
        }

        public static VectorHex operator *(VectorHex left, int scalar)
        {
            return new VectorHex(left.q * scalar, left.r * scalar);
        }

        public static VectorHex operator *(int scalar, VectorHex right)
        {
            return right * scalar;
        }

        public static VectorHex operator /(VectorHex left, VectorHex right)
        {
            return new VectorHex(left.q / right.q, left.r / right.r);
        }

        public static VectorHex operator /(VectorHex left, int scalar)
        {
            return new VectorHex(left.q / scalar, left.r / scalar);
        }

        public static VectorHex operator >>(VectorHex a, int times) => a.RotateRight(times);

        public static VectorHex operator <<(VectorHex a, int times) => a.RotateLeft(times);

        public static implicit operator VectorHexF(VectorHex v) => new VectorHexF(v.q, v.r);

        public static int Distance(VectorHex a, VectorHex b) => (a - b).magnitude;

        public static IEnumerator<VectorHex> GetAllInLine(VectorHex a, VectorHex b)
        {
            int distance = Distance(a, b);
            for (int i = 0; i <= distance; i++)
            {
                yield return (VectorHex)VectorHexF.Lerp(a, b, 1f / distance * i);
            }
        }

        public VectorHex(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        /// <summary>
        /// Creates a VectorHex that is "times" clockwise rotations away from this vector
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public VectorHex RotateRight(int times)
        {
            VectorHex a = this;
            for (int i = 0; i < times; i++)
            {
                a = new VectorHex(-a.r, -a.s);
            }
            return a;
        }

        /// <summary>
        /// Creates a VectorHex that is "times" counter-clockwise rotations away from this vector
        /// </summary>
        /// <param name="times"></param>
        /// <returns></returns>
        public VectorHex RotateLeft(int times)
        {
            VectorHex a = this;
            for (int i = 0; i < times; i++)
            {
                a = new VectorHex(-a.s, -a.q);
            }
            return a;
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(VectorHex))
            {
                VectorHex other = (VectorHex)obj;
                return (q == other.q) && (r == other.r);
            }
            return false;
        }

        public override int GetHashCode() => new { q, r }.GetHashCode();

        public override string ToString() => $"Hex({q}, {r}, {s})";
    }
}
