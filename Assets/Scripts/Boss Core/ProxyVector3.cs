using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BossCore
{
    public class ProxyVector3 : ProxyVariable<Vector3>
    {
        private Vector3 offset;

        public static ProxyVector3 PLAYER_POSITION = new ProxyVector3(() => { return GameManager.Player.transform.position + World.Arena.CENTER; });
        public static ProxyVector3 BOSS_POSITION = new ProxyVector3(() => { return GameManager.Boss.transform.position + World.Arena.CENTER; });
        public static ProxyVector3 RANDOM_IN_ARENA = new ProxyVector3(() =>
        {
            float angle = Random.value * 360;
            float distance = Random.Range(0, GameManager.Arena.RadiusInWorldUnits);
            return distance * (Quaternion.AngleAxis(angle, Vector3.up) * Vector3.forward) + World.Arena.CENTER;
        });

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
