using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Projectiles
{
    public static class CallbackDictionary
    {
        public static ProjectileCallbackDelegate NOTHING = (self) => { Debug.Log("Test"); };

        public static ProjectileCallbackDelegate SPAWN_6_CURVING = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.New(self.data.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(body.velocity)
                          .SetAngleOffset(i * 60f)
                          .SetMaxTime(3f)
                          .SetSpeed(self.data.speed)
                          .Curving((float)self.data.speed * 2f, true)
                          .Create();
            }
        };

        public static ProjectileCallbackDelegate SPAWN_6 = (self) =>
        {
            Debug.Log("SPAWN 6 called");
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                Projectile.New(self.data.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(body.velocity)
                          .SetAngleOffset(i * 60f)
                          .SetMaxTime(3f)
                          .SetSpeed(self.data.speed)
                          .Curving(0f, false)
                          .Create();
            }
        };

        // TODO: there should be a better way to call AISequences from these callbacks
        public static ProjectileCallbackDelegate SPAWN_WAVE = (self) =>
        {
            //BossController.ShootWave(50).events[0].action();
            for (int i = 0; i < 50; i++) {
                Projectile.New(self.data.entity)
                          .SetStart(self.transform.position)
                          .SetTarget(Vector3.forward)
                          .SetSize(Size.MEDIUM)
                          .SetSpeed(Speed.MEDIUM)
                          .SetAngleOffset(i * (360f / 50f))
                          .Create();
            }
        };

        public static ProjectileCallbackDelegate SPAWN_1_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .New(self.data.entity)
                .SetStart(self.transform.position)
                .SetTarget(null)
                .SetMaxTime(self.data.maxTime)
                .SetSize(self.data.size)
                .SetSpeed(Speed.SNIPE)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_1_HOMING_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .New(self.data.entity)
                .SetStart(self.transform.position)
                .SetTarget(null)
                .SetSize(self.data.size)
                .SetSpeed(Speed.SNIPE)
                .SetMaxTime(self.data.maxTime)
                .Create()
                .Homing();
        };

        public static ProjectileCallbackDelegate REVERSE = (self) =>
        {
            Projectile
                .New(self.data.entity)
                .SetStart(self.transform.position)
                .SetTarget(self.data.start)
                .SetMaxTime(self.data.maxTime)
                .SetSize(self.data.size)
                .SetSpeed(self.data.speed)
                .Create();
        };

        public static ProjectileCallbackDelegate REVERSE_FASTER = (self) =>
        {
            if (self.data.speed == Speed.LIGHTNING)
            {
                return;
            }

            Speed currentSpeed = self.data.speed;
            Speed[] speeds = (Speed[])System.Enum.GetValues(currentSpeed.GetType());
            Speed nextSpeed = speeds[System.Array.IndexOf(speeds, currentSpeed) + 1];

            Debug.Log("New speed: " + nextSpeed);

            Projectile
                .New(self.data.entity)
                .SetStart(self.transform.position)
                .SetTarget(self.data.start)
                .SetSize(self.data.size)
                .SetSpeed(nextSpeed)
                .SetMaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                .OnDestroyTimeout(REVERSE_FASTER)
                .Create();
        };
    }
}
