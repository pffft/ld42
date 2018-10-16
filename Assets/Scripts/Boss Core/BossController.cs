﻿using System.Collections;
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

    //private static AIPhase phase;
    private static List<AIPhase> phases;
    private static AIPhase currentPhase;
    private static int phaseIndex = -1;

    private void Awake()
    {
        eventQueue = new EventQueue();

        self = GetComponent<CombatCore.Entity>();
        self.name = BOSS_NAME;
    }

    // Use this for initialization
    void Start()
    {
        Profiler.BeginSample("Initialize phases");
        AIPhase.Load();
        Profiler.EndSample();


        phases = new List<AIPhase>();

        phases.Add(AIPhase.PHASE_TUTORIAL_1);
        //phases.Add(AIPhase.PHASE_TUTORIAL_2);
        //phases.Add(AIPhase.PHASE_TUTORIAL_3);
        //phases.Add(AIPhase.PHASE1);
        //phases.Add(AIPhase.PHASE_TEST);

        NextPhase();
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, prompt the queue for then next event.
        eventQueue.Update();

        // If the queue ran out of events, pull the next AISequence in the phase
        if (eventQueue.Empty())
        {
            eventQueue.Add(currentPhase.GetNext());
        }
    }

    /*
     * Transitions to the next phase by loading in the next set of moves.
     */
    public static void NextPhase() {
        phaseIndex++;
        if (phaseIndex > phases.Count) {
            Debug.LogError("You win!");
		}

        currentPhase = phases[phaseIndex];
        self.healthMax = currentPhase.maxHealth;
        GameManager.Arena.RadiusInWorldUnits = currentPhase.maxArenaRadius;

        // Heal up to the new max health.
        CombatCore.Entity.HealEntity(self, float.PositiveInfinity);

        self.SetInvincible(true);
        eventQueue.Add(Teleport(World.Arena.CENTER).Wait(3f));
        eventQueue.Add(new AISequence(() => { self.SetInvincible(false); }));
    }

    public static void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(GameManager.Player.transform.position - GameManager.Boss.transform.position);
        GameManager.Boss.transform.rotation = lookRotation;
    }

}
