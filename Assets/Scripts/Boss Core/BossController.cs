using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Projectiles;
using AI;

using UnityEngine.Profiling;

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
        //eventQueue = new EventQueue();
        //eventQueue = GameManager.EventQueue;

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

        //phases.Add(AIPhase.PHASE_TUTORIAL_1);
        //phases.Add(AIPhase.PHASE_TUTORIAL_2);
        //phases.Add(AIPhase.PHASE_TUTORIAL_3);
        //phases.Add(AIPhase.PHASE1);
        phases.Add(AIPhase.PHASE_TEST);

        NextPhase();
    }

    //private bool test = true;

    // Update is called once per frame
    void Update()
    {
        // Every frame, prompt the queue for then next event.
        GameManager.EventQueue.Update();

        // If the queue ran out of events, pull the next AISequence in the phase
        if (GameManager.EventQueue.Empty())
        {
            GameManager.EventQueue.Add(currentPhase.GetNext());
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
        GameManager.EventQueue.Add(new Moves.Basic.Teleport(World.Arena.CENTER).Wait(3f));
        GameManager.EventQueue.Add(new AISequence(() => { self.SetInvincible(false); }));
    }

    /*
     * Dashes to the provided position. 
     * 
     * This happens separately from the event queue, and will pause any future
     * events until after this dash is completed.
     */
    public static IEnumerator Dash(Vector3 targetPosition)
    {

        GameManager.EventQueue.Pause();

        Vector3 dashDir = (targetPosition - GameManager.Boss.transform.position).normalized;

        float accDist = 0f, maxDist = Vector3.Distance(targetPosition, GameManager.Boss.transform.position);
        while (accDist < maxDist)
        {
            float dashDistance = Mathf.Min((insaneMode ? 1.2f : 1f) * self.movespeed.Value * 4 * Time.deltaTime, maxDist - accDist);

            RaycastHit hit;
            if (Physics.Raycast(GameManager.Boss.transform.position, dashDir, out hit, dashDistance, 1 << LayerMask.NameToLayer("Default")))
            {
                GameManager.Boss.transform.position = hit.point;
                break;
            }

            GameManager.Boss.transform.position += dashDir * dashDistance;
            accDist += dashDistance;
            yield return null;
        }

        self.movespeed.LockTo(25);
        GameManager.EventQueue.Unpause();
    }

    /*
     * Rotates the boss model to look in the direction of the player.
     * 
     * Should be used when launching an attack at the player to let them know
     * about the boundless depths of white-hot furi that the boss has for them.
     */
    public static void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(GameManager.Player.transform.position - GameManager.Boss.transform.position);
        GameManager.Boss.transform.rotation = lookRotation;
    }

}
