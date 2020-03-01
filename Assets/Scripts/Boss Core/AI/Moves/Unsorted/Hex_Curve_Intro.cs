using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using AOEs;
using Constants;
using Moves.Basic;
using Projectiles;

namespace Moves.Unsorted 
{
	public class Hex_Curve_Intro : Move
    {
        public Hex_Curve_Intro()
        {
			Description = "Introduces the player to the hex curve attack";
			Difficulty = 4f;
            Sequence = new AISequence(
                new Shoot_Hex_Curve(true),
                new AOE_360().Wait(2.5f),
                new Shoot_Hex_Curve(false),
                new AOE_360().Wait(2.5f),
                new Shoot_Hex_Curve(true),
                new AOE_360(),
                new Shoot_Hex_Curve(false).Wait(1f),
                new AOE_360().Wait(1f),
                new AOE_360().Wait(1.5f),
                new Teleport().Wait(0.5f)
            );
		}
	}
}
