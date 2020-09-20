using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;

using static Constants.Positions;

namespace Moves.Unsorted 
{
    public class Jump_Rope_Fast : Move
    {
        public Jump_Rope_Fast()
        {
            Description = "Fires lines at the player from the left and right.";
            Difficulty = 4f;
            Sequence = new AISequence(
                new MoveCamera(false, new Vector3(0, 17.5f, -35)).Wait(1f),
                new Teleport(WEST_FAR, 35),
                new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
                new Teleport(EAST_FAR, 35),
                new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
                new Teleport(WEST_FAR, 35),
                new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
                new Teleport(EAST_FAR, 35),
                new ShootLine(50, 100f, speed: Speed.SNIPE).Times(2),
                new MoveCamera(true)
            );
        }
    }
}
