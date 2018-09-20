using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileDeathHex : Projectile
    {

        public override Material GetCustomMaterial() {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }
    }

    public static class ProjectileDeathHexHelper
    {
        public static ProjectileDeathHex DeathHex(this Projectile projectile)
        {
            return (ProjectileDeathHex)projectile.CastTo<ProjectileDeathHex>().OnDestroyTimeout(CallbackDictionary.SPAWN_6_CURVING);
        }
    }
}
