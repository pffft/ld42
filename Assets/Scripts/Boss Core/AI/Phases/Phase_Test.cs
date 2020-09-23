using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;

namespace Phases
{
    public class Phase_Test : AIPhase
    {
        public Phase_Test()
        {
            //AddSequence(10, new Moves.Test.Lightning_Arena().Times(2));
            //AddSequence(10, new Moves.Test.Quick_Waves());
            //AddSequence(10, new Moves.Test.Double_Laser_Sweep_AOE());
            //AddSequence(10, new Moves.Test.Double_Laser_Sweep());
            //AddSequence(10, new Moves.Test.Pincer_Sweep());
            //AddSequence(10, new Moves.Test.SpinReverse().Wait(2f));
            //AddSequence(10, new Moves.Test.Random_Leading());
            //AddSequence(10, new Moves.Test.Sniper_Final());
            //AddSequence(10, new Moves.Basic.Shoot_Death_Hex());
            AddSequence(10, new Moves.Test.Horseshoe_AOE());
        }
    }
}