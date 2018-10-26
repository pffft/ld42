using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BossCore;
using static AI.AISequence;
using Moves.Basic;

namespace Projectiles
{
    public static class CallbackDictionary
    {
        public static ProjectileCallback NOTHING = self => new Pause(0f);

        public static ProjectileCallback FREEZE = self =>
            new Shoot1(
                new Projectile(self.data.entity)
                    .Start(self.transform.position)
                    .MaxTime(5f)
                    .Size(self.data.size)
            );

        public static ProjectileCallback SPAWN_6_CURVING = self =>
            For(6, i => new Shoot1(
                new ProjectileCurving(self.data.entity, (float)self.data.speed * 2f, true)
                    .Start(self.transform.position)
                    .Target(self.data.velocity)
                    .AngleOffset(i * 60f)
                    .MaxTime(3f)
                )
            );

        public static ProjectileCallback SPAWN_6 = self =>
            For(6, i => new Shoot1(
                new ProjectileCurving(self.data.entity, 0f, true)
                    .Start(self.transform.position)
                    .Target(self.data.velocity)
                    .AngleOffset(i * 60f)
                    .MaxTime(3f)
                )
            );

        // Spawns a wave at the death position.
        public static ProjectileCallback SPAWN_WAVE = self =>
            new ShootAOE(AOEs.AOE.New(self.data.entity).Start(self.transform.position).On(0, 360f));
        
        public static ProjectileCallback SPAWN_1_TOWARDS_PLAYER = self =>
            new Shoot1(
                new Projectile(self.data.entity)
                    .Start(self.transform.position)
                    .MaxTime(self.data.maxTime)
                    .Size(self.data.size)
            );

        public static ProjectileCallback SPAWN_1_HOMING_TOWARDS_PLAYER = self =>
            new Shoot1(
                new ProjectileHoming(self.data.entity)
                    .Start(self.transform.position)
                    .MaxTime(self.data.maxTime)
                    .Size(self.data.size)
                    .Speed(Speed.LIGHTNING)
            );

        public static ProjectileCallback REVERSE = (self) =>
            new Shoot1(
                new Projectile(self.data.entity)
                    .Start(self.transform.position)
                    .Target(self.data.start)
                    .MaxTime(self.data.maxTime)
                    .Size(self.data.size)
            );

        public static ProjectileCallback REVERSE_FASTER = (self) =>
        {
            if (self.data.speed == Speed.LIGHTNING)
            {
                return NOTHING(self);
            }

            Speed currentSpeed = self.data.speed;
            Speed[] speeds = (Speed[])System.Enum.GetValues(currentSpeed.GetType());
            Speed nextSpeed = speeds[System.Array.IndexOf(speeds, currentSpeed) + 1];

            return new Shoot1(
                new Projectile(self.data.entity)
                    .Start(self.transform.position)
                    .Target(self.data.start)
                    .Size(self.data.size)
                    .MaxTime(2f * 1.5f * (float)Speed.FAST / (float)nextSpeed)
                    .OnDestroyTimeout(REVERSE_FASTER)
            );
        };
    }
}
