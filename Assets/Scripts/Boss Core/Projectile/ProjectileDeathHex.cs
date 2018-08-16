using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileDeathHex : Projectile
    {

		public override void OnDestroyTimeout()
		{
            Rigidbody body = GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.spawnCurving(entity, transform.position, body.velocity, body.velocity.magnitude * 6f, 3f, i * 60f, Speed.MEDIUM);
            }
		}
	}
}
