using System;
using UnityEngine;

namespace Hex
{
    /// <summary>
    /// A floating point hexagonal coordinate or vector.
    /// An implementation of https://www.redblobgames.com/grids/hexagons/.
    /// </summary>
    [Serializable]
    public struct VectorHexF
    {
        private const float TOLERANCE = 0.001f;

        [field: SerializeField] public float q { get; }
        [field: SerializeField] public float r { get; }
        public float s { get { return -q - r; } }
        public float magnitude => (Math.Abs(q) + Math.Abs(r) + Math.Abs(s)) / 2;

        public static VectorHexF operator +(VectorHexF left, VectorHexF right)
        {
            return new VectorHexF(left.q + right.q, left.r + right.r);
        }

        public static VectorHexF operator -(VectorHexF left, VectorHexF right)
        {
            return new VectorHexF(left.q - right.q, left.r - right.r);
        }

        public static VectorHexF operator *(VectorHexF left, float scalar)
        {
            return new VectorHexF(left.q * scalar, left.r * scalar);
        }

        public static VectorHexF operator *(float scalar, VectorHexF right)
        {
            return right * scalar;
        }

        public static VectorHexF operator /(VectorHexF left, VectorHexF right)
        {
            return new VectorHexF(left.q / right.q, left.r / right.r);
        }

        public static VectorHexF operator /(VectorHexF left, float scalar)
        {
            return new VectorHexF(left.q / scalar, left.r / scalar);
        }

        public static explicit operator VectorHex(VectorHexF v)
        {
            int rq = (int)Math.Round(v.q);
            int rr = (int)Math.Round(v.r);
            int rs = (int)Math.Round(v.s);
            float dq = Math.Abs(rq - v.q);
            float dr = Math.Abs(rr - v.r);
            float ds = Math.Abs(rs - v.s);
            if (dq > dr && dq > ds)
            {
                rq = -rr - rs;
            }
            else if (dr > ds)
            {
                rr = -rq - rs;
            }

            return new VectorHex(rq, rr);
        }

        public static VectorHexF Lerp(VectorHexF a, VectorHexF b, float t) => a * (1f - t) + b * t;

        public VectorHexF(float q, float r)
        {
            this.q = q;
            this.r = r;
            if (Mathf.Abs(q + r + s) > TOLERANCE)
                throw new ArgumentException($"{q} + {r} + {s} != 0");
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(VectorHexF))
            {
                VectorHexF other = (VectorHexF)obj;
                return (q == other.q) && (r == other.r);
            }
            return false;
        }

        public override int GetHashCode() => new { q, r }.GetHashCode();

        public override string ToString() => $"HexF({q}, {r}, {s})";
    }
}
