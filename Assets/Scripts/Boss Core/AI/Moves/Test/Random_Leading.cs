using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Constants;
using Projectiles;
using AOEs;
using Moves.Basic;

namespace Moves.Test
{
    /// <summary>
    /// I really like the lasers.
    /// </summary>
    public class Random_Leading : Move
    {
        public Random_Leading(int count = 20)
        {
            Sequence = new AISequence(
                new Shoot1(
                    new ProjectileReverse()
                    {
                        Start = Positions.RANDOM_IN_ARENA,
                        Target = Positions.PLAYER_POSITION,
                        Size = Size.HUGE,
                        Speed = Constants.Speed.FAST,
                        MaxTime = 4f
                    }
                ).Wait(0.15f).Times(count),
                new ShootAOE(new AOEData { OuterSpeed = Constants.Speed.MEDIUM, FixedWidth = 5f }.On(0, 360)).Wait(0.5f),
                new Laser(-60, 480, 5, 60),
                new Laser(60, 480, 5, 45),
                new Laser(120, 480, 5, 30),
                new Shoot1(
                    new ProjectileReverse()
                    {
                        Start = Positions.RANDOM_IN_ARENA,
                        Target = Positions.PLAYER_POSITION,
                        Size = Size.HUGE,
                        Speed = Constants.Speed.FAST,
                        MaxTime = 4f
                    }
                ).Wait(0.15f).Times(count),
                new ShootAOE(new AOEData { OuterSpeed = Constants.Speed.MEDIUM, FixedWidth = 5f }.On(0, 360)).Wait(0.5f),
                new Shoot1(
                    new ProjectileReverse()
                    {
                        Start = Positions.RANDOM_IN_ARENA,
                        Target = Positions.PLAYER_POSITION,
                        Size = Size.HUGE,
                        Speed = Constants.Speed.FAST,
                        MaxTime = 4f
                    }
                ).Wait(0.15f).Times(count),
                new ShootAOE(new AOEData { OuterSpeed = Constants.Speed.MEDIUM, FixedWidth = 5f }.On(0, 360)).Wait(0.5f)
            );
        }
    }
}
