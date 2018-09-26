﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BossCore;

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
                Projectile.New(self.data.entity)
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
                Projectile.New(self.data.entity)
                          .Start(self.transform.position)
                          .Target(body.velocity)
                          .AngleOffset(i * 60f)
                          .MaxTime(3f)
                          .Speed(self.data.speed)
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
                          .Start(self.transform.position)
                          .Target(Vector3.forward)
                          .Size(Size.MEDIUM)
                          .Speed(Speed.MEDIUM)
                          .AngleOffset(i * (360f / 50f))
                          .Create();
            }
        };

        public static ProjectileCallbackDelegate SPAWN_1_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .New(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .MaxTime(self.data.maxTime)
                .Size(self.data.size)
                .Speed(Speed.SNIPE)
                .Create();
        };

        public static ProjectileCallbackDelegate SPAWN_1_HOMING_TOWARDS_PLAYER = (self) =>
        {
            Projectile
                .New(self.data.entity)
                .Start(self.transform.position)
                .Target(null)
                .Size(self.data.size)
                .Speed(Speed.SNIPE)
                .MaxTime(self.data.maxTime)
                .Create()
                .Homing();
        };

        public static ProjectileCallbackDelegate REVERSE = (self) =>
        {
            Projectile
                .New(self.data.entity)
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

            Projectile
                .New(self.data.entity)
                .Start(self.transform.position)
                .Target(self.data.start)
                .Size(self.data.size)
                .Speed(nextSpeed)
                .MaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                .OnDestroyTimeout(REVERSE_FASTER)
                .Create();
        };
    }
}
