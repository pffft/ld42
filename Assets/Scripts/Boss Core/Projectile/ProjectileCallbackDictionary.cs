using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BossCore;

namespace Projectiles
{
    public static class CallbackDictionary
    {
        public static ProjectileCallbackDelegate NOTHING = (self) => { };

        public static ProjectileCallbackDelegate FREEZE = (self) =>
        {
            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .MaxTime(5f)
                .Size(self.data.size)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_6_CURVING = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                new ProjectileCurving(self.data.entity, (float)self.data.speed * 2f, true)
                    .Start(self.transform.position)
                    .Target(body.velocity)
                    .AngleOffset(i * 60f)
                    .MaxTime(3f)
                    .Create();
            }
        };

        public static ProjectileCallbackDelegate SPAWN_6 = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                new ProjectileCurving(self.data.entity, 0f, false)
                    .Start(self.transform.position)
                    .Target(body.velocity)
                    .AngleOffset(i * 60f)
                    .MaxTime(3f)
                    .Create();
            }
        };

        // Spawns a wave at the death position.
        public static ProjectileCallbackDelegate SPAWN_WAVE = (self) =>
        {
            AOEs.AOE.New(self.data.entity).Start(self.transform.position).On(0, 360f).Create();
        };

        public static ProjectileCallbackDelegate SPAWN_1_TOWARDS_PLAYER = (self) =>
        {
            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .MaxTime(self.data.maxTime)
                .Size(self.data.size)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_1_HOMING_TOWARDS_PLAYER = (self) =>
        {
            new ProjectileHoming(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .Size(self.data.size)
                .MaxTime(self.data.maxTime)
                .Create();
        };

        public static ProjectileCallbackDelegate REVERSE = (self) =>
        {
            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(self.data.start)
                .MaxTime(self.data.maxTime)
                .Size(self.data.size)
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

            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(self.data.start)
                .Size(self.data.size)
                .MaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                .OnDestroyTimeout(REVERSE_FASTER)
                .Create();
        };
    }
}
