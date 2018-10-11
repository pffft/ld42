using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileDeathHex : ProjectileComponent
    {

        public override Material GetCustomMaterial() {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }
    }

    public static class ProjectileDeathHexHelper
    {
        public static ProjectileDeathHex DeathHex(this ProjectileComponent projectile)
        {
            return (ProjectileDeathHex)projectile
                .CastTo<ProjectileDeathHex>();
        }

        public static Projectile DeathHex(this Projectile structure) {
            structure.type = Type.DEATHHEX;
            structure.OnDestroyTimeout(CallbackDictionary.SPAWN_6_CURVING);
            return structure;
        }
    }
}
