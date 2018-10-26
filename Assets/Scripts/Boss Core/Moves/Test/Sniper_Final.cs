using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using static BossController;
using static World.Arena;
using BossCore;

namespace Moves.Test
{
    public class Sniper_Final : AISequence
    {
        public Sniper_Final() : base
        (
            new Teleport(NORTH_FAR).Wait(0.5f),
            new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.SLOW).InnerSpeed(Speed.FROZEN).InnerScale(0f).Target(SOUTH_FAR).MaxTime(2f)
                         .OnDestroyTimeout(self => self.Clone().Freeze().MaxTime(10f).Create()))
            .Wait(1f),
            new AISequence(
                new AISequence(
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(0.25f),
                    new ShootAOE(AOE.New(self).On(-15, 15).Speed(Speed.MEDIUM).FixedWidth(20f).Target(SOUTH_FAR)).Wait(0.75f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(1f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(0.25f),
                    new ShootAOE(AOE.New(self).On(-45, -15).On(15, 45).Speed(Speed.MEDIUM).FixedWidth(20f).Target(SOUTH_FAR)).Wait(0.75f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(1f)
                ).Times(2),
                new AISequence(
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)),
                    new ShootAOE(AOE.New(self).Start(WEST_FAR).Target(EAST_FAR).On(-90, 90).Speed(Speed.FAST).FixedWidth(7f)).Wait(0.25f),
                    new ShootAOE(AOE.New(self).On(-15, 15).Speed(Speed.MEDIUM).FixedWidth(20f).Target(SOUTH_FAR)).Wait(0.75f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(1f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)),
                    new ShootAOE(AOE.New(self).Start(EAST_FAR).Target(WEST_FAR).On(-90, 90).Speed(Speed.FAST).FixedWidth(7f)).Wait(0.25f),
                    new ShootAOE(AOE.New(self).On(-45, -15).On(15, 45).Speed(Speed.MEDIUM).FixedWidth(20f).Target(SOUTH_FAR)).Wait(0.75f),
                    new ShootAOE(AOE.New(self).On(-80, 80).Speed(Speed.MEDIUM).FixedWidth(7f).Target(SOUTH_FAR)).Wait(1f)
                ).Times(2),
                new Shoot1(new Projectile { Speed = Speed.SNAIL, Size = Size.HUGE, MaxTime = 1f, OnDestroyTimeout = CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER }).Wait(2.5f),
                new Shoot1(new Projectile { Speed = Speed.SNAIL, Size = Size.HUGE, MaxTime = 1f, OnDestroyTimeout = CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER }).Wait(2.5f)
            )
        )
        { }
    }
}
