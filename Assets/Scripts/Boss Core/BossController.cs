using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;
using AI;

using UnityEngine.Profiling;
using static AI.SequenceGenerators;

/*
 * TODO: refactor the shootXXX methods to have similar parameters.
 * "target" should be a nullable; if null and locked, then use old position;
 * if null and unlocked, use current player position; if not null, use specified target.
 * 
 * Next, add Speed/Size/Type parameters to all; angle offsets.
 * 
 * Find some way to add the code currently in Start via sequences; i.e., parameters
 * that increase in step by a certain amount, for a specified number of times.
 */
public class BossController : MonoBehaviour
{
    // The event queue that we use for planning attacks.
    public static EventQueue eventQueue;

    // A reference to the player object (for position tracking)
    public static GameObject player;

    // A reference to the game arena.
    public static GameObject arena;

    // A reference to our physics body (for self movement and dashing)
    public static Rigidbody physbody;

    // Toggles insane mode. This just makes everything a living hell.
    // Specifically, every waiting period is reduced and movement speed is buffed.
    public static bool insaneMode;

    // The Entity representing this faction. Assigned to projectiles we create.
    public static CombatCore.Entity self;
    public static string BOSS_NAME = "Boss";

    // Used for the "CameraLock" action. Keeps track of the current player position
    // for events and sequences that need a slightly out of date version.
    public static Vector3 playerLockPosition;
    public static bool isPlayerLocked;

    #region Singleton stuff
    public static BossController instance;
    #endregion

    private static AIPhase phase;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Looks like you have two BossControllers here. Stop that.");
        }

        eventQueue = new EventQueue();
        player = GameObject.Find("Player");
        arena = GameObject.Find("Arena");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();
        self.name = BOSS_NAME;
    }

    // Use this for initialization
    void Start()
    {
        Profiler.BeginSample("Initialize event queue");

        //eventQueue.AddSequence(AISequence.SWEEP_BACK_AND_FORTH);
        phase = AIPhase.PHASE_TEST;
        //phase = AIPhase.PHASE1;

        //eventQueue.Add(AISequence.CIRCLE_JUMP_ROPE.Wait(10f).Times(2));

        /*
        eventQueue.Add(new AISequence(() =>
        {
            AOE.Create(self).SetSpeed(Speed.FAST).On(-60, 60).SetFixedWidth(6);
        }).Wait(0.75f).Times(2));
        eventQueue.Add(new AISequence(() =>
        {
            AOE.Create(self).SetSpeed(Speed.FAST).On(-60, 60).SetFixedWidth(12);
        }));
        */
        //eventQueue.Add(ShootAOE(AOE.Create(self).SetSpeed(Speed.FAST).On(-60, 60).SetFixedWidth(6)).Wait(10f));
        //eventQueue.Add(ShootAOE(AOE.Create(self).SetSpeed(Speed.FAST).On(-120, 120).SetFixedWidth(6)).Wait(10f));
        //eventQueue.Add(ShootAOE(AOE.New(self).SetSpeed(Speed.FAST).On(-60, 60).SetFixedWidth(6)).Wait(0.5f).Times(20));

        //eventQueue.Add(AISequence.AOE_131_MEDIUM_LONG.Wait(0.5f).Times(10));

        Profiler.EndSample();
    }

    // Update is called once per frame
    void Update()
    {
        eventQueue.Update();
        #if true
        if (eventQueue.Empty())
        {
            eventQueue.Add(phase.GetNext());
        }
        #endif
    }

    public static void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - instance.transform.position);
        instance.transform.rotation = lookRotation;
    }

}
