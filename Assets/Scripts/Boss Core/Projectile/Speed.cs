using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public enum Speed
    {
        FROZEN = 0,
        SNAIL = 10,
        SLOW = 15,
        MEDIUM_SLOW = 20,
        MEDIUM = 25,
        FAST = 35,
        VERY_FAST = 45,
        SNIPE = 50, // This is realistically the fastest speed you should use.
        LIGHTNING = 60
    };
}