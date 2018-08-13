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

        public int count = 0;
        public float numSpawners = 30f;

		public override void CustomUpdate()
        {
            Quaternion rot = Quaternion.AngleAxis(Time.deltaTime * curveAmount, Vector3.up);
            body.velocity = rot * body.velocity;

            if (currentTime > count / numSpawners)
            {
                count++;
                Projectile.spawnBasic(entity, transform.position, Vector3.zero, maxTime - currentTime, size: Size.SMALL, speed: Speed.FROZEN);
            }
		}

		public override void Initialize(params object[] args)
		{
            body = GetComponent<Rigidbody>();
            curveAmount = (float)args[0];
		}
	}
}