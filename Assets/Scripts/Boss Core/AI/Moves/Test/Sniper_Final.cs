using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using Projectiles;
using AOEs;
using Constants;
using static Constants.Positions;

namespace Moves.Test
{
    public class Sniper_Final : Move
    {
        public Sniper_Final()
        {
            Sequence = new AISequence(
                new Teleport(NORTH_FAR).Wait(0.5f),
                new ShootAOE(new AOEData
                {
                    OuterSpeed = Speed.SLOW,
                    InnerSpeed = Speed.FROZEN,
                    InnerScale = 0f,
                    Target = SOUTH_FAR,
                    MaxTime = 2f,
                    OnDestroyTimeout =
                    self =>
                    {
                        // TODO write a better API for this
                        AOEData clone = self.data.Clone();
                        clone.MaxTime = 10f;
                        clone.Freeze();
                        clone.Create();
                    }
                }.On(-80, 80)
                             )
                .Wait(1f),
                new AISequence(
                    new AISequence(
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(0.25f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 20f, Target = SOUTH_FAR }.On(-15, 15)).Wait(0.75f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(1f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(0.25f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 20f, Target = SOUTH_FAR }.On(-45, -15).On(15, 45)).Wait(0.75f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(1f)
                    ).Times(2),
                    new AISequence(
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)),
                        new ShootAOE(new AOEData { Start = WEST_FAR, Target = EAST_FAR, OuterSpeed = Speed.FAST, FixedWidth = 7f }.On(-90, 90)).Wait(0.25f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 20f, Target = SOUTH_FAR }.On(-15, 15)).Wait(0.75f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(1f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)),
                        new ShootAOE(new AOEData { Start = EAST_FAR, Target = WEST_FAR, OuterSpeed = Speed.FAST, FixedWidth = 7f }.On(-90, 90)).Wait(0.25f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 20f, Target = SOUTH_FAR }.On(-45, -15).On(15, 45)).Wait(0.75f),
                        new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 7f, Target = SOUTH_FAR }.On(-80, 80)).Wait(1f)
                    ).Times(2),
                    new Shoot1(new ProjectileData { Speed = Speed.SNAIL, Size = Size.HUGE, MaxTime = 1f, OnDestroyTimeout = CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER }).Wait(2.5f),
                    new Shoot1(new ProjectileData { Speed = Speed.SNAIL, Size = Size.HUGE, MaxTime = 1f, OnDestroyTimeout = CallbackDictionary.SPAWN_1_HOMING_TOWARDS_PLAYER }).Wait(2.5f)
                )
            );
        }
    }
}
