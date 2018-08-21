using UnityEngine;
using System.Collections;

namespace Projectiles
{
    public enum Type
    {
        BASIC,
        INDESTRUCTIBLE,
        HOMING,
        CURVING,
        DEATHHEX
    }

    // Please make sure you add a link between new Projectile.Type and
    // System.Type (that extends Projectile) here, if you modify Type!
    public static readonly Dictionary<Type, System.Type> TypeClassLookup = {
        {Type.HOMING, typeof(ProjectileHoming)},
        {Type.CURVING, typeof(ProjectileCurving)},
        {Type.DEATHHEX, typeof(ProjectileDeathHex)}
    }
}
