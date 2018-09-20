﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileDeathHex : Projectile
    {

        public override Material GetCustomMaterial() {
            return Resources.Load<Material>("Art/Materials/GreenTransparent");
        }

		public override void OnDestroyTimeout()
		{
            Rigidbody body = GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                //Projectile.spawnCurving(entity, transform.position, body.velocity, body.velocity.magnitude * 6f, 3f, i * 60f, Speed.MEDIUM);
                Projectile.Create(entity,
                                  start:transform.position, 
                                  target:body.velocity, 
                                  angleOffset:i * 60f, 
                                  maxTime:3f, 
                                  speed:this.speed)
                          .Curving((float)speed * 2f, true);
            }
		}
	}

    public static class ProjectileDeathHexHelper
    {
        public static ProjectileDeathHex DeathHex(this Projectile projectile)
        {
            return projectile.CastTo<ProjectileDeathHex>();
        }
    }
}
