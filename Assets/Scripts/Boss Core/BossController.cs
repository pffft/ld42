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
    // Toggles insane mode. This just makes everything a living hell.
    // Specifically, every waiting period is reduced and movement speed is buffed.
    public static bool insaneMode;

    [SerializeField]
    public bool DebugMode = true;

    // The Entity representing this faction. Assigned to projectiles we create.
    public static CombatCore.Entity self;
    public static string BOSS_NAME = "Boss";

    // Used for the "PlayerLock" move. Keeps track of the current player position
    // for events and sequences that need a slightly out of date version.
    public static Vector3 playerLockPosition;
    public static bool isPlayerLocked;

    // Phase iteration logic
    private static List<AIPhase> phases;
    private static AIPhase currentPhase;
    private static int phaseIndex = -1;

    // Event queue variables. How we schedule our attacks.
    private static Queue<AISequence> queuedSequences;
    private static Queue<AIEvent> events;
    private static bool paused;
    private static bool running;

    private void Awake()
    {
        //eventQueue = new EventQueue();
        //eventQueue = GameManager.EventQueue;

        queuedSequences = new Queue<AISequence>();
        events = new Queue<AIEvent>();
        paused = false;
        running = true;

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
        phases.Add(AIPhase.PHASE1);
        //phases.Add(AIPhase.PHASE_TEST);
        //phases.Add(AIPhase.PHASE_UNIT_TEST);

        StartCoroutine(ExecuteQueue());
        NextPhase();
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
        Add(new Moves.Basic.Teleport(World.Arena.CENTER).Wait(3f));
        Add(new AISequence(() => { self.SetInvincible(false); }));
    }

    public static void Add(AISequence sequence)
    {
        if (GameManager.Boss.DebugMode)
        {
            if (sequence == null)
            {
                Debug.LogError("Null AISequence added to queue.");
                return;
            }

            // Generate warning if there's a named sequence without a description.
            //
            // Note that "glue" AISequences are allowed; those that don't subclass AISequence 
            // don't need to provide a description. This includes subclassed AISequences that
            // have additional Wait()s or Then()s called.
            if (sequence.Description == null && !sequence.Name.Equals("AISequence"))
            {
                Debug.LogWarning("Found AISequence with a name, but without a description. Name: " + sequence.Name);
            }

            // Generate warning if there's a sequence with too high a difficulty.
            if (sequence.Difficulty >= 8f)
            {
                Debug.LogWarning("Found AISequence \"" + sequence.Name + "\" with very high difficulty: " + sequence.Difficulty + ". ");
            }

            Debug.Log("Added AISequence" +
                      (sequence.Name.Equals("AISequence") ? " " : " \"" + sequence.Name + "\" ") +
                      "to queue. Here's what it says it'll do: \"" +
                      (sequence.Description ?? sequence.ToString()) + "\".");
        }

        queuedSequences.Enqueue(sequence);
    }

    // Update is called once per frame
    void Update()
    {
        // Every frame, prompt the queue for then next event.
        //GameManager.EventQueue.Update();

        // If the queue ran out of events, pull the next AISequence in the phase
        if (queuedSequences.Count == 0)
        {
            Add(currentPhase.GetNext());
        }
    }


    // event queue
    private IEnumerator ExecuteQueue()
    {
        while (running)
        {
            //Profiler.BeginSample("Event Queue");
            while (queuedSequences.Count == 0)
            {
                yield return new WaitForSeconds(0.05f);
            }

            AISequence nextSequence = queuedSequences.Dequeue();
            yield return Execute(nextSequence);
            //Profiler.EndSample();
        }
    }

    // event queue
    private static IEnumerator Execute(AISequence sequence)
    {
        if (sequence.events != null)
        {
            for (int i = 0; i < sequence.events.Length; i++)
            {
                while (paused)
                {
                    yield return new WaitForSecondsRealtime(0.05f);
                }

                sequence.events[i].action?.Invoke();
                // TODO reduce the wait time if the above invocation takes too long 
                yield return new WaitForSecondsRealtime(sequence.events[i].duration);
            }
        }
        else
        {
            AISequence[] children = sequence.GetChildren();
            for (int i = 0; i < children.Length; i++)
            {
                yield return Execute(children[i]);
            }
        }
    }

    public static void ExecuteAsync(AISequence sequence)
    {
        GameManager.Boss.StartCoroutine(Execute(sequence));
    }

    /*
     * Dashes to the provided position. 
     * 
     * This happens separately from the event queue, and will pause any future
     * events until after this dash is completed.
     */
    public static IEnumerator Dash(Vector3 targetPosition)
    {
        paused = true;

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
        paused = false;
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
