using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using static AI.AISequence;

namespace Projectiles
{
    public class ProjectileLightning : Projectile
    {
        private readonly int level;
        private Vector3 initialTarget;
        private float initialMaxTime;

        private int count;
        private readonly int numSpawners = 30;

        public ProjectileLightning(int level=0) : this(BossController.self, level) { }

        public ProjectileLightning(Entity self, int level) : this(self, level, 1f) { }

        private ProjectileLightning(Entity self, int level, float initialMaxTime) : base(self) 
        {
            this.level = level;
            this.initialMaxTime = initialMaxTime;

            this.maxTime = 0.05f;

            Speed(BossCore.Speed.LIGHTNING);
            OnDestroyTimeout(LIGHTNING_RECUR);
        }

        public override Projectile MaxTime(float seconds) {
            if (level == 0)
            {
                this.initialMaxTime = seconds;
            } else 
            {
                base.MaxTime(seconds);
            }
            return this;
        }

        public override void CustomCreate(ProjectileComponent component)
        {
            // Make the target the player position, but at a radius of 100.
            // This prevents "bunching" around the true target.
            initialTarget = (100f / component.data.target.magnitude) * component.data.target;
        }

        public override Material CustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/BlueTransparent");
        }

        public override void CustomUpdate(ProjectileComponent component)
        {
            if ((component.transform.position - GameManager.Player.transform.position).magnitude < 5f)
            {
                GameObject.Destroy(component.gameObject);
            }

            if (component.currentTime > count / numSpawners)
            {
                count++;
                new Projectile()
                    .Start(component.transform.position)
                    .MaxTime(initialMaxTime - component.currentTime)
                    .Size(Projectiles.Size.SMALL)
                    .Speed(BossCore.Speed.FROZEN)
                    .Create();
            }
        }

        // Recursively generates more lightning
        private static ProjectileCallback LIGHTNING_RECUR = (self) =>
        {
            ProjectileLightning lightningSelf = self.data as ProjectileLightning;

            // Stop the recursion at level 7
            if (lightningSelf.level > 6) 
            {
                return AI.AISequence.Pause(0f);
            }

            int times;
            if (lightningSelf.level < 3)
            {
                times = Random.Range(2, 3);
            }
            else
            {
                times = Random.Range(1, 2);
            }


            return Merge(
                For(times, i => 
                    new Moves.Basic.Shoot1(
                        new ProjectileLightning(lightningSelf.entity, lightningSelf.level + 1, lightningSelf.initialMaxTime - lightningSelf.maxTime)
                            .Start(self.transform.position)
                            .Target(lightningSelf.initialTarget)
                            .AngleOffset(lightningSelf.angleOffset - 45f + Random.Range(0, 90f))
                            .MaxTime(Random.Range(0.05f, 0.15f))
                            .Speed(BossCore.Speed.LIGHTNING)
                            .OnDestroyTimeout(LIGHTNING_RECUR)
                    )
                )
            );
        };
    }
}