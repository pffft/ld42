using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

namespace Projectiles
{

    public class ProjectileCurving : Projectile
    {

        public Rigidbody body;
        public float curveAmount;

        public int count;
        public float numSpawners = 30f;

        public bool leavesTrail;

        public ProjectileCurving(Entity self, float curveAmount, bool leavesTrail)
            : base(self)
        {
            this.curveAmount = curveAmount;
            this.leavesTrail = leavesTrail;
        }

        public ProjectileCurving(float curveAmount, bool leavesTrail) 
            : this(BossController.self, curveAmount, leavesTrail)
        {}

        public new ProjectileComponent Create()
        {
            return base.Create();
            // TODO make either a new ProjectileComponent subclass, or ideally find a way
            // to set the proper overrides on a base ProjectileComponent object & return it
        }
    }

    /*
    public class ProjectileCurving : ProjectileComponent
    {
        public Rigidbody body;
        public float curveAmount;

        public int count;
        public float numSpawners = 30f;

        public bool leavesTrail;

        public override Material GetCustomMaterial()
        {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }

        public override void CustomUpdate()
        {
            Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * curveAmount, Vector3.up);
            body.velocity = rot * body.velocity;

            if (leavesTrail)
            {
                if (data.currentTime > count / numSpawners)
                {
                    count++;
                    new Projectile(data.entity)
                            .Start(transform.position)
                            .MaxTime(data.maxTime - data.currentTime)
                            .Size(Size.SMALL)
                            .Speed(BossCore.Speed.FROZEN)
                            .Create();
                }
            }
        }
	}

    public static class ProjectileCurvingHelper {
        public static ProjectileCurving Curving(this ProjectileComponent projectile, float curveAmount, bool leavesTrail)
        {
            ProjectileCurving curving = projectile.CastTo<ProjectileCurving>();

            curving.body = projectile.GetComponent<Rigidbody>();
            curving.curveAmount = curveAmount;
            curving.leavesTrail = leavesTrail;

            return curving;
        }

        public static Projectile Curving(this Projectile structure, float curveAmount, bool leavesTrail) 
        {
            structure.type = Type.CURVING;
            structure._typeParameters = new object[] { curveAmount, leavesTrail };
            return structure;
        }
    }
    */
}