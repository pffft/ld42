using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileLightning : ProjectileComponent
    {
        public int level;
        public Vector3 initialTarget;

        private int count;
        private int numSpawners = 30;

        public override Material GetCustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/BlueTransparent");
        }

        public override void CustomUpdate()
        {
            if (level >= 7)
            {
                Destroy(this.gameObject);
            }

            if ((transform.position - GameManager.Player.transform.position).magnitude < 5f)
            {
                Destroy(this.gameObject);
            }

            if (data.currentTime > count / numSpawners)
            {
                count++;
                new Projectile()
                        .Start(transform.position)
                        .MaxTime(1f - data.currentTime)
                        .Size(Size.SMALL)
                        .Speed(BossCore.Speed.FROZEN)
                        .Create();
            }

        }
    }

    public static class ProjectileLightningHelper
    {
        public static ProjectileLightning Lightning(this ProjectileComponent projectile, int level)
        {
            ProjectileLightning lightning = projectile.CastTo<ProjectileLightning>();
            lightning.level = level;
            lightning.initialTarget = projectile.data.target;
            return lightning;
        }

        public static Projectile Lightning(this Projectile structure, int level)
        {
            structure.type = Type.LIGHTNING;
            structure._typeParameters = new object[] { level };
            return structure;
        }
    }
}