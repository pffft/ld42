using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: move all boss controller sequences in here
// make a JSON parser to make this job easier?
public partial class AISequence {

    private static BossController bc = BossController.GetInstance();

    public static AISequence SHOOT3_WAVE3 = new AISequence(
        new AIEvent(0.5f, bc.Teleport()),
        new AIEvent(0f, bc.ShootWave(50, 360f, 0f)),
        AISequence.Repeat(new AIEvent(0.1f, bc.Shoot3()), 20),
        new AIEvent(0f, bc.ShootWave(50, 360f, 0f)),
        AISequence.Repeat(new AIEvent(0.1f, bc.Shoot3()), 20),
        new AIEvent(0f, bc.ShootWave(50, 360f, 0f))
    );
}
