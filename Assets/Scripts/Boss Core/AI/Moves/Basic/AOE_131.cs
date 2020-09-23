﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;
using static BossController;

namespace Moves.Basic 
{
    public class AOE_131 : Move 
    {
        public AOE_131()
        {
            Description = "Shoots an AOE attack in a 1-3-1 pattern.";
            Difficulty = 5f;
            Sequence = new AISequence(
                new Teleport().Wait(0.5f),
                new PlayerLock(true),
                new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 5 }.On(-60, 60)).Wait(0.19f),
                new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 20 }.On(-60, -40).On(-10, 10).On(40, 60)).Wait(0.76f),
                new ShootAOE(new AOEData { OuterSpeed = Speed.MEDIUM, FixedWidth = 5 }.On(-60, 60)).Wait(0.2f),
                new PlayerLock(false).Wait(1f)
            );
        }
    }
}
