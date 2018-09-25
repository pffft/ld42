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
                Projectile.Create(self.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(body.velocity)
                          .SetAngleOffset(i * 60f)
                          .SetMaxTime(3f)
                          .SetSpeed(self.speed)
                          .Curving((float)self.speed * 2f, true);
            }
        };

        public static ProjectileCallbackDelegate SPAWN_6 = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.Create(self.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(body.velocity)
                          .SetAngleOffset(i * 60f)
                          .SetMaxTime(3f)
                          .SetSpeed(self.speed)
                          .Curving(0f, false);
            }
        };

        // TODO: there should be a better way to call AISequences from these callbacks
        public static ProjectileCallbackDelegate SPAWN_WAVE = (self) =>
        {
            //BossController.ShootWave(50).events[0].action();
            for (int i = 0; i < 50; i++) {
                Projectile.Create(self.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(Vector3.forward)
                          .SetSize(Size.MEDIUM)
                          .SetSpeed(Speed.MEDIUM)
                          .SetAngleOffset(i * (360f / 50f));
            }
        };

        public static ProjectileCallbackDelegate SPAWN_1_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .Create(self.entity)
                .SetStart(self.transform.position)
                .SetTarget(null)
                .SetMaxTime(self.maxTime)
                .SetSize(self.size)
                .SetSpeed(Speed.SNIPE);
        };

        public static ProjectileCallbackDelegate SPAWN_1_HOMING_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .Create(self.entity)
                .SetStart(self.transform.position)
                .SetTarget(null)
                .SetSize(self.size)
                .SetSpeed(Speed.SNIPE)
                .SetMaxTime(self.maxTime)
                .Homing();
        };

        public static ProjectileCallbackDelegate REVERSE = (self) =>
        {
            Projectile
                .Create(self.entity)
                .SetStart(self.transform.position)
                .SetTarget(self.start)
                .SetMaxTime(self.maxTime)
                .SetSize(self.size)
                .SetSpeed(self.speed);
        };

        public static ProjectileCallbackDelegate REVERSE_FASTER = (self) =>
        {
            if (self.speed == Speed.LIGHTNING)
            {
                return;
            }

            Speed currentSpeed = self.speed;
            Speed[] speeds = (Speed[])System.Enum.GetValues(currentSpeed.GetType());
            Speed nextSpeed = speeds[System.Array.IndexOf(speeds, currentSpeed) + 1];

            Debug.Log("New speed: " + nextSpeed);

            Projectile
                .Create(self.entity)
                .SetStart(self.transform.position)
                .SetTarget(self.start)
                .SetSize(self.size)
                .SetSpeed(nextSpeed)
                .SetMaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                .OnDestroyTimeout(REVERSE_FASTER);
        };
    }
}
