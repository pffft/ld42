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
                new Projectile
                {
                    Start = self.transform.position,
                    MaxTime = 5f,
                    Size = self.data.Size
                }
            );

        public static ProjectileCallback SPAWN_6_CURVING = self =>
            ForConcurrent(6, i => new Shoot1(
                new ProjectileCurving((float)self.data.Speed * 2f, true)
                {
                    Start = self.transform.position,
                    Target = self.data.Velocity,
                    AngleOffset = i * 60f,
                    MaxTime = 3f
                }
            ));

        public static ProjectileCallback SPAWN_6 = self =>
            ForConcurrent(6, i => new Shoot1(
                new ProjectileCurving(0f, true)
                {
                    Start = self.transform.position,
                    Target = self.data.Velocity,
                    AngleOffset = i * 60f,
                    MaxTime = 3f
                }
            ));

        // Spawns a wave at the death position.
        public static ProjectileCallback SPAWN_WAVE = self =>
            new ShootAOE(new AOEs.AOE { Start = self.transform.position }.On(0, 360f));
        
        public static ProjectileCallback SPAWN_1_TOWARDS_PLAYER = self =>
            new Shoot1(
                new Projectile
                {
                    Start = self.transform.position,
                    MaxTime = self.data.MaxTime,
                    Size = self.data.Size
                }
            );

        public static ProjectileCallback SPAWN_1_HOMING_TOWARDS_PLAYER = self =>
            new Shoot1(
                new ProjectileHoming
                {
                    Start = self.transform.position,
                    MaxTime = self.data.MaxTime,
                    Size = self.data.Size,
                    Speed = Speed.LIGHTNING
                }
            );

        public static ProjectileCallback REVERSE = (self) =>
            new Shoot1(
                new Projectile
                {
                    Start = self.transform.position,
                    Target = self.data.Start,
                    MaxTime = self.data.MaxTime,
                    Size = self.data.Size
                }
            );

        public static ProjectileCallback REVERSE_FASTER = (self) =>
        {
            if (self.data.Speed == Speed.LIGHTNING)
            {
                return NOTHING(self);
            }

            Speed currentSpeed = self.data.Speed;
            Speed[] speeds = (Speed[])System.Enum.GetValues(currentSpeed.GetType());
            Speed nextSpeed = speeds[System.Array.IndexOf(speeds, currentSpeed) + 1];

            return new Shoot1(
                new Projectile
                {
                    Start = self.transform.position,
                    Target = self.data.Start,
                    Size = self.data.Size,
                    MaxTime = 2f * 1.5f * (float)Speed.FAST / (float)nextSpeed,
                    OnDestroyTimeout = REVERSE_FASTER
                }
            );
        };
    }
}
