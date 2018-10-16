using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AOEs;
using static BossController;

namespace Moves.Basic
{
    public class Definitions : MoveLoader
    {
        /// <summary>
        /// Shoots a single small, medium speed projectile at the player.
        /// </summary>
        public static Shoot1 SHOOT_1;

        /// <summary>
        /// Shoots three small, medium speed projectiles at the player.
        /// </summary>
        public static Shoot3 SHOOT_3;

        /// <summary>
        /// Shoots 2 90 waves as one block, encouraging dodging through them.
        /// </summary>
        public static Shoot_2_Waves SHOOT_2_WAVES;

        /// <summary>
        /// Shoots two 60 degree waves with a 45 degree gap in the middle.
        /// </summary>
        public static Move SHOOT_WAVE_MIDDLE_GAP;

        /// <summary>
        /// Shoots a 360 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_360;

        /// <summary>
        /// Shoots a 120 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_120;

        /// <summary>
        /// Shoots a 90 degree AOE attack with a width of 3.
        /// </summary>
        public static Move AOE_90;

        /// <summary>
        /// Fires a line of projectiles at the player, then strafes 60 degrees.
        /// </summary>
        public static Move LINE_STRAFE_60;

        /// <summary>
        /// Shoots a medium-slow 360 AOE, then strafes 60 degrees.
        /// </summary>
        public static Move SLOW_WAVE_CIRCLE;

        /// <summary>
        /// Shoots a 120 degree AOE, followed by 3 20 degree wide AOEs, followed 
        /// by another 120 degree AOE; all connected.
        /// </summary>
        public static Move AOE_131_MEDIUM_LONG;

        /// <summary>
        /// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position.
        /// </summary>
        public static Sweep SWEEP;

        /// <summary>
        /// Shoots a sweep from +30 degrees to -90 degrees offset from the player's current position.
        /// </summary>
        public static Move SWEEP_REVERSE;

        /// <summary>
        /// Shoots a sweep from -30 degrees to +90 degrees offset from the player's current position, 
        /// then another from -90 to +30 degrees.
        /// </summary>
        public static Move SWEEP_BACK_AND_FORTH;

        /// <summary>
        /// Sweeps both left and right at the same time.
        /// </summary>
        public static Move SWEEP_BOTH;

        /// <summary>
        /// Shoots a projectile that splits into 6 more, going outwards from the death point.
        /// </summary>
        public static Move SPLIT_6;

        /// <summary>
        /// Shoots a projectile that splits into 6 curving projectiles.
        /// </summary>
        public static Move SPLIT_6_CURVE;

        public override void Load() {

            AOE_90 = new ShootAOE(AOE.New(self).On(-45f, 45f).FixedWidth(3f));
            AOE_120 = new ShootAOE(AOE.New(self).On(-60f, 60f).FixedWidth(3f));
            AOE_360 = new ShootAOE(AOE.New(self).On(0, 360).FixedWidth(3f));

            base.Load();
        }
    }
}
