using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCore
{
    public class ProxyVector3 : ProxyVariable<Vector3>
    {
        private Vector3 offset;

        public ProxyVector3(ProxiedValueGet get) : base(get) { }

        public override Vector3 GetValue()
        {
            return ((Vector3)get()) + offset;
        }

        // Creates a new proxy vector3 that just holds a simple pass-through value.
        public static implicit operator ProxyVector3(Vector3 other) {
            return new ProxyVector3(() => other);
        }

        public static ProxyVector3 operator +(ProxyVector3 thisValue, Vector3 offset) {
            return new ProxyVector3(thisValue.get)
            {
                offset = thisValue.offset + offset
            };
        }
    }
}
