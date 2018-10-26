using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Moves.Basic;
using AOEs;
using static BossController;

namespace Moves.Test
{
    public class Player_Strafe_Waves : AISequence
    {

        public Player_Strafe_Waves() : base
        (
            /*
            () => {
                List<AISequence> sequences = new List<AISequence>();
                sequences.Add(new Teleport());
                sequences.Add(new PlayerLock(true));
                for (int i = 0; i < 6; i++)
                {
                    sequences.Add(new Strafe(true, 60, 25, GameManager.Player.transform.position));
                    sequences.Add(new ShootAOE(AOE.New(self).On(-60, 60)));
                    sequences.Add(new Pause(0.75f));
                }
                sequences.Add(new PlayerLock(false));
                sequences.Add(new Pause(1f));
                return sequences.ToArray();
            }
            */
            For(6, i => new Strafe(true, 60, 25, GameManager.Player.transform.position)).Then(new ShootAOE(AOE.New(self).On(-60, 60))).Wait(0.75f)
        )
        {

        }
    }
}