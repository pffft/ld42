using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public class ProxyFloat : ProxyVariable<float>
    {
        private float mulOffset = 1;
        private float addOffset = 0;

        public ProxyFloat(ProxiedValueGet get) : base(get) { }

        public override float GetValue()
        {
            float proxyVal = (float)get();
            return (mulOffset * proxyVal) + addOffset;
        }

        // Creates a new proxy vector3 that just holds a simple pass-through value.
        public static implicit operator ProxyFloat(float other)
        {
            return new ProxyFloat(() => other);
        }

        public static ProxyFloat operator +(ProxyFloat thisValue, float offset) {
            return new ProxyFloat(thisValue.get)
            {
                mulOffset = thisValue.mulOffset,
                addOffset = thisValue.addOffset + offset
            };
        }

        public static ProxyFloat operator -(ProxyFloat thisValue, float offset)
        {
            return new ProxyFloat(thisValue.get)
            {
                mulOffset = thisValue.mulOffset,
                addOffset = thisValue.addOffset - offset
            };
        }

        public static ProxyFloat operator *(ProxyFloat thisValue, float offset)
        {
            return new ProxyFloat(thisValue.get)
            {
                mulOffset = thisValue.mulOffset * offset,
                addOffset = thisValue.addOffset * offset
            };
        }

        public static ProxyFloat operator /(ProxyFloat thisValue, float offset)
        {
            return new ProxyFloat(thisValue.get)
            {
                mulOffset = thisValue.mulOffset / offset,
                addOffset = thisValue.addOffset / offset
            };
        }

    }
}
