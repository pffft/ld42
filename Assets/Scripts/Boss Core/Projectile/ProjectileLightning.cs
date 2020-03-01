using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

using static AI.AISequence;

namespace Projectiles
{
    public class ProjectileLightning : ProjectileData
    {
        private readonly int level;
        private Vector3 initialTarget;
        private float initialMaxTime;

        private int count;
        private readonly int numSpawners = 30;

        public ProjectileLightning() : this(0, 1f) { }

        private ProjectileLightning(int level) : this(level, 1f) { }

        private ProjectileLightning(int level, float initialMaxTime)
        {
            this.level = level;
            this.initialMaxTime = initialMaxTime;

            base.MaxTime = 0.05f;

            Speed = Constants.Speed.LIGHTNING;
            //OnDestroyTimeout = LIGHTNING_RECUR;
        }

        public override float MaxTime
        {
            get
            {
                return base.MaxTime;
            }

            set
            {
                if (level == 0) {
                    this.initialMaxTime = value;
                } else {
                    base.MaxTime = value;
                }
            }
        }

        public override void CustomCreate(Projectile component)
        {
            // Make the target the player position, but at a radius of 100.
            // This prevents "bunching" around the true target.
            initialTarget = (100f / component.data.Target.GetValue().magnitude) * component.data.Target.GetValue();
        }

        public override Material CustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/BlueTransparent");
        }

        public override void CustomUpdate(Projectile component)
        {
            if ((component.transform.position - GameManager.Player.transform.position).magnitude < 5f)
            {
                GameObject.Destroy(component.gameObject);
            }

            if (component.currentTime > count / numSpawners)
            {
                count++;
                new ProjectileData
                {
                    Start = component.transform.position,
                    MaxTime = initialMaxTime - component.currentTime,
                    Size = Size.SMALL,
                    Speed = Constants.Speed.FROZEN
                }.Create();
            }
        }

        // Recursively generates more lightning
        // TODO move this into the CustomUpdate so this compiles
        /*
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
                        new ProjectileLightning(lightningSelf.level + 1, lightningSelf.initialMaxTime - lightningSelf.MaxTime)
                        {
                            Start = self.transform.position,
                            Target = lightningSelf.initialTarget,
                            AngleOffset = lightningSelf.AngleOffset - 45f + Random.Range(0, 90f),
                            MaxTime = Random.Range(0.05f, 0.15f),
                            Speed = BossCore.Speed.LIGHTNING,
                            OnDestroyTimeout = LIGHTNING_RECUR
                        }
                    )
                )
            );
        };
        */
    }
}