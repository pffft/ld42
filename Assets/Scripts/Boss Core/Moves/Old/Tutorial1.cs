using AI;
using AOEs;
using BossCore;
using Projectiles;
using System.Collections.Generic;
using UnityEngine;

using static AI.AISequence;
using static AI.SequenceGenerators;
using static BossController;

namespace Moves
{
    //public class Tutorial1 : IMoveDictionary
    //{
    //    /// <summary>
    //    /// Shoots between 5 and 10 default projectiles at the player.
    //    /// </summary>
    //    public static Move SHOOT_1_SEVERAL;

    //    /// <summary>
    //    /// Shoots between 7 and 12 3-way projectiles at the player.
    //    /// </summary>
    //    public static Move SHOOT_3_SEVERAL;

    //    /// <summary>
    //    /// Shoots a 70 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_70;

    //    /// <summary>
    //    /// Shoots a 120 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_120;

    //    /// <summary>
    //    /// Shoots a 150 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_150;

    //    /// <summary>
    //    /// Shoots a dense 70 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_70_DENSE;

    //    /// <summary>
    //    /// Shoots a dense 120 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_120_DENSE;

    //    /// <summary>
    //    /// Shoots a dense 150 degree arc at the player.
    //    /// </summary>
    //    public static Move SHOOT_ARC_150_DENSE;

    //    public void Load() {

    //        SHOOT_1_SEVERAL = new Move(
    //            2f,
    //            "SHOOT_1_SEVERAL",
    //            "Shoots between 5 and 10 default projectiles at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                new AISequence(() =>
    //                {
    //                    return Shoot1().Wait(0.1f).Times(Random.Range(5, 10));
    //                }).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_3_SEVERAL = new Move(
    //            3f,
    //            "SHOOT_3_SEVERAL",
    //            "Shoots between 7 and 12 3-way projectiles at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                new AISequence(() =>
    //                {
    //                    return Shoot3().Wait(0.1f).Times(Random.Range(7, 12));
    //                }).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_70 = new Move(
    //            2f,
    //            "SHOOT_ARC_70",
    //            "Shoots a 70 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(50, -35, 35).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_120 = new Move(
    //            2f,
    //            "SHOOT_ARC_120",
    //            "Shoots a 120 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(50, -60, 60).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_150 = new Move(
    //            2f,
    //            "SHOOT_ARC_150",
    //            "Shoots a 150 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(50, -75, 75).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_70_DENSE = new Move(
    //            2f,
    //            "SHOOT_ARC_70",
    //            "Shoots a dense 70 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(100, -35, 35).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_120_DENSE = new Move(
    //            2f,
    //            "SHOOT_ARC_120",
    //            "Shoots a dense 120 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(100, -60, 60).Wait(1.5f)
    //            )
    //        );

    //        SHOOT_ARC_150_DENSE = new Move(
    //            2f,
    //            "SHOOT_ARC_150",
    //            "Shoots a dense 150 degree arc at the player.",
    //            new AISequence(
    //                Teleport().Wait(0.25f),
    //                ShootArc(100, -75, 75).Wait(1.5f)
    //            )
    //        );
    //    }
    //}
}
