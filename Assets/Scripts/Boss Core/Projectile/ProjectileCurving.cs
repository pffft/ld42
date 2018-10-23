﻿using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{

    public class ProjectileCurving : Projectile
    {

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

        public override Material CustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }

        public override void CustomUpdate(ProjectileComponent component)
        {
            Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * curveAmount, Vector3.up);
            //body.velocity = rot * body.velocity;
            velocity = rot * velocity;

            if (leavesTrail)
            {
                if (component.currentTime > count / numSpawners)
                {
                    count++;
                    new Projectile(entity)
                            .Start(component.transform.position)
                            .MaxTime(maxTime - component.currentTime)
                            .Size(Projectiles.Size.SMALL)
                            .Speed(BossCore.Speed.FROZEN)
                            .Create();
                }
            }
        }
    }
}