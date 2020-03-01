using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileReverse : ProjectileData
    {
        private Vector3 initialTarget;

        public override void CustomCreate(Projectile component)
        {
            component.currentTime = 0;
            initialTarget = (Target.GetValue() - Start.GetValue()).normalized;
        }

        public override void CustomUpdate(Projectile component)
        {
            Velocity = (float)Speed * func(component.currentTime) * initialTarget;
        }

        private static float func(float x)
        {
            return Mathf.Min(50 * Mathf.Pow(x, 4) - (10 * Mathf.Pow(x, 2)), 1.5f);
        }
    }
}
