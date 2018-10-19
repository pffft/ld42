﻿using System.Collections;
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
                .Speed(Speed.FROZEN)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_6_CURVING = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                new Projectile(self.data.entity)
                          .Start(self.transform.position)
                          .Target(body.velocity)
                          .AngleOffset(i * 60f)
                          .MaxTime(3f)
                          .Speed(self.data.speed)
                          .Curving((float)self.data.speed * 2f, true)
                          .Create();
            }
        };

        public static ProjectileCallbackDelegate SPAWN_6 = (self) =>
        {
            Rigidbody body = self.GetComponent<Rigidbody>();
            for (int i = 0; i < 6; i++)
            {
                new Projectile(self.data.entity)
                          .Start(self.transform.position)
                          .Target(body.velocity)
                          .AngleOffset(i * 60f)
                          .MaxTime(3f)
                          .Speed(self.data.speed)
                          .Curving(0f, false)
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
                .Speed(Speed.SNIPE)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_1_HOMING_TOWARDS_PLAYER = (self) =>
        {
            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .Size(self.data.size)
                .Speed(Speed.LIGHTNING)
                .MaxTime(self.data.maxTime)
                .Create()
                .Homing();
        };

        public static ProjectileCallbackDelegate REVERSE = (self) =>
        {
            new Projectile(self.data.entity)
                .Start(self.transform.position)
                .Target(self.data.start)
                .MaxTime(self.data.maxTime)
                .Size(self.data.size)
                .Speed(self.data.speed)
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
                .Speed(nextSpeed)
                .MaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                .OnDestroyTimeout(REVERSE_FASTER)
                .Create();
        };

        public static ProjectileCallbackDelegate LIGHTNING_RECUR = (self) =>
        {
            ProjectileLightning lightningSelf = self as ProjectileLightning;
            int times;
            if (lightningSelf.level < 3)
            {
                times = Random.Range(2, 3);
            }
            else
            {
                times = Random.Range(1, 2);
            }

            for (int i = 0; i < times; i++)
            {
                new Projectile(self.data.entity)
                    .Start(self.transform.position)
                    .Target(lightningSelf.initialTarget)
                    .AngleOffset(self.data.angleOffset - 45f + Random.Range(0, 90f))
                    .Speed(Speed.LIGHTNING)
                    .MaxTime(Random.Range(0.05f, 0.15f))
                    .OnDestroyTimeout(LIGHTNING_RECUR)
                    .Lightning(lightningSelf.level + 1)
                    .Create();
            }
        };
    }
}
