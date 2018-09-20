using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public static class CallbackDictionary
    {
        public static ProjectileCallbackDelegate NOTHING = (self) => { };

        public static ProjectileCallbackDelegate SPAWN_6_CURVING = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.Create(self.entity,
                                  start: self.transform.position,
                                  target: body.velocity,
                                  angleOffset: i * 60f,
                                  maxTime: 3f,
                                  speed: self.speed)
                          .Curving((float)self.speed * 2f, true);
            }
        };

        public static ProjectileCallbackDelegate SPAWN_6 = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.Create(self.entity,
                                  start: self.transform.position,
                                  target: body.velocity,
                                  angleOffset: i * 60f,
                                  maxTime: 3f,
                                  speed: self.speed)
                          .Curving(0f, false);
            }
        };

        // TODO: there should be a better way to call AISequences from these callbacks
        public static ProjectileCallbackDelegate SPAWN_WAVE = (self) =>
        {
            //BossController.ShootWave(50).events[0].action();
            for (int i = 0; i < 50; i++) {
                Projectile
                    .Create(self.entity)
                    .SetStart(self.transform.position)
                    .SetTarget(Vector3.forward)
                    .SetSize(Size.MEDIUM)
                    .SetSpeed(Speed.MEDIUM)
                    .SetAngleOffset(i * (360f / 50f));
            }
        };
    }
}
