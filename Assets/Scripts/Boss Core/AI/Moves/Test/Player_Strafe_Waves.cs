using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using AOEs;
using static BossController;

namespace Moves.Test
{
    public class Player_Strafe_Waves : Move
    {

        public Player_Strafe_Waves()
        {
            Sequence = For(6, i => new Strafe(true, 60, 25, GameManager.Player.transform.position)).Then(new ShootAOE(new AOEData().On(-60, 60))).Wait(0.75f);
        }
    }
}