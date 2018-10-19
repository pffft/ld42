using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{

    public class ProjectileCurving : Projectile
    {

        private Rigidbody body;
        private readonly float curveAmount;

        private int count;
        private readonly float numSpawners = 30f;

        private readonly bool leavesTrail;

        public ProjectileCurving(float curveAmount, bool leavesTrail)
            : this(BossController.self, curveAmount, leavesTrail)
        { }

        public ProjectileCurving(Entity self, float curveAmount, bool leavesTrail)
            : base(self)
        {
            this.curveAmount = curveAmount;
            this.leavesTrail = leavesTrail;
        }


        public override void CustomCreate(ProjectileComponent component)
        {
            body = component.GetComponent<Rigidbody>();
        }

        public override Material CustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }

        public override void CustomUpdate(ProjectileComponent component)
        {
            Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * curveAmount, Vector3.up);
            body.velocity = rot * body.velocity;

            if (leavesTrail)
            {
                if (currentTime > count / numSpawners)
                {
                    count++;
                    new Projectile(entity)
                            .Start(component.transform.position)
                            .MaxTime(maxTime - currentTime)
                            .Size(Projectiles.Size.SMALL)
                            .Speed(BossCore.Speed.FROZEN)
                            .Create();
                }
            }
        }
    }
}