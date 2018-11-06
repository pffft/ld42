using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Projectiles;
using Moves.Basic;
using static World.Arena;

namespace Moves.Unsorted
{
    public class AOE_Test_2 : Move
    {
        //// Lets the player know the cardinal directions will be dangerous soon.
        private static AISequence TELEGRAPH_CARDINAL = new AISequence(
            Merge(
                new ShootArc(100, -7, 7, new Projectile { Target = SOUTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -7, 7, new Projectile { Target = WEST_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -7, 7, new Projectile { Target = NORTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -7, 7, new Projectile { Target = EAST_FAR }).Wait(0.2f).Times(3)
            ),
            Merge(
                new ShootArc(100, -15, 15, new Projectile { Target = SOUTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -15, 15, new Projectile { Target = WEST_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -15, 15, new Projectile { Target = NORTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -15, 15, new Projectile { Target = EAST_FAR }).Wait(0.2f).Times(3)
            ),
            Merge(
                new ShootArc(100, -25, 25, new Projectile { Target = SOUTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -25, 25, new Projectile { Target = WEST_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -25, 25, new Projectile { Target = NORTH_FAR }).Wait(0.2f).Times(3),
                new ShootArc(100, -25, 25, new Projectile { Target = EAST_FAR }).Wait(0.2f).Times(3)
            )
        );


        public AOE_Test_2() {
            Sequence = new AISequence(
                TELEGRAPH_CARDINAL,

                new ShootAOE(
                    new AOE {
                    OuterSpeed = BossCore.Speed.MEDIUM,
                    InnerSpeed = BossCore.Speed.SLOW,
                    Target = Vector3.forward,
                    MaxTime = 2f,
                    // TODO add ability to change values in callbacks like before
                    //OnDestroyTimeout = (self) => self.data.Clone().Freeze().RotationSpeed(20f).MaxTime(12.6f).Create(),
                    OnDestroyOutOfBounds = AOECallbackDictionary.DONT_DESTROY_OOB
                    }
                ).Wait(3.2f)

                /*
                    AOE.New(self)
                         .Speed(Speed.MEDIUM)
                         .InnerSpeed(Speed.SLOW)
                         .Target(Vector3.forward)
                         .MaxTime(2f)
                         .On(-22.5f, 22.5f)
                         .On(90 - 22.5f, 90 + 22.5f)
                         .On(180 - 22.5f, 180 + 22.5f)
                         .On(270 - 22.5f, 270 + 22.5f)
                         .OnDestroyTimeout((self) => self.Clone().Freeze().RotationSpeed(20f).MaxTime(12.6f).Create())
                         .OnDestroyOutOfBounds(AOECallbackDictionary.DONT_DESTROY_OOB)
                .Wait(3.2f),
                */

                /*
                new ShootAOE(AOE.New(self)
                         .Speed(Speed.FAST)
                         .InnerSpeed(Speed.SNAIL)
                         .Target(Vector3.forward)
                         .MaxTime(1f)
                         .On(-22.5f, 22.5f)
                         .On(90 - 22.5f, 90 + 22.5f)
                         .On(180 - 22.5f, 180 + 22.5f)
                         .On(270 - 22.5f, 270 + 22.5f)
                         .OnDestroyTimeout((self) => self.Clone().Freeze().RotationSpeed(20f).MaxTime(10.4f).Create())
                        )
                .Wait(1.2f),

                new ShootAOE(AOE.New(self)
                         .Speed(Speed.FAST)
                         .InnerScale(0f)
                         .MaxTime(0.6f)
                         .InnerSpeed(Speed.FROZEN)
                         .Target(Vector3.forward)
                         .On(0, 360f)
                         .OnDestroyTimeout((self) => self.Clone().Freeze().MaxTime(9.6f).Create())
                        )
                .Wait(0.6f),

                new ShootArc(100, 0, 360).Wait(1.5f).Times(6).Wait(5f)
                */

            );
        }

        void HandleAOECallbackDelegate(AOEComponent self)
        {
        }

    }
}
