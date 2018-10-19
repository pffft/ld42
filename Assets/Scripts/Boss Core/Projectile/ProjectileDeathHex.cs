using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileDeathHex : Projectile
    {

        public ProjectileDeathHex() : this(BossController.self) { }

        public ProjectileDeathHex(Entity self) : base(self) {
            OnDestroyTimeout(CallbackDictionary.SPAWN_6_CURVING);
        }

        public override Material CustomMaterial() {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }
    }
}
