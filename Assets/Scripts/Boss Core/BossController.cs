using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;

using AI;

public class BossController : MonoBehaviour
{
    [Tooltip("If enabled, reduces delays between attacks and increases base movement speed.")]
    [SerializeField]
    private bool insaneMode = false;

    [Tooltip("If enabled, will print logging messages related to current attacks. Has a mild performance impact.")]
    [SerializeField]
    private bool DebugMode = true;

    [Tooltip("If enabled, the boss won't attack you or move at the start.")]
    [SerializeField]
    private bool Chill = false;

    // The Entity representing this faction. Assigned to projectiles we create.
    private CombatCore.Entity self;

    // Phase iteration logic
    private AIRoutine routine;

    // Event queue variables. This is how we schedule our attacks.
    private Queue<AISequence> queuedSequences;
    private bool paused;
    private bool running = true;

    #region Debugging code that should be refactored soon™
    // Debug code. Used to set the routine from the inspector rather than changing code.
    private enum _Routine {
        Tutorial,
        //Main,
        Test,
        TestLatest,
        //UnitTest
        Unsorted,
    }
    [SerializeField]
    private _Routine Routine = _Routine.Tutorial;
    #endregion

    void Awake()
    {
        if (self == null)
        {
            self = GetComponent<CombatCore.Entity>();
        }
        queuedSequences = new Queue<AISequence>();
    }

    // Use this for initialization
    void Start()
    {
        Profiler.BeginSample("Initialize phases");
        //AIPhase.Load();
        Profiler.EndSample();

        

        //AISequence.ShouldAllowInstantiation = true;
        switch (Routine) {
            case _Routine.Tutorial:
                routine = new Routines.Tutorial();
                break;
            case _Routine.Test:
                routine = new Routines.Test();
                break;
            case _Routine.TestLatest:
                routine = new Routines.TestLatest();
                break;
            case _Routine.Unsorted:
                routine = new Routines.Unsorted();
                break;
        }
        //AISequence.ShouldAllowInstantiation = false;

        // Kick off the execution engine
        StartCoroutine(ExecuteQueue());
        NextPhase();
    }

    /*
     * Transitions to the next phase by loading in the next set of moves.
     */
    public void NextPhase()
    {
        Debug.Log("Next phase");
        queuedSequences.Clear();
        StopAllCoroutines();
        StartCoroutine(ExecuteQueue());

        routine.NextPhase();

        self.healthMax = routine.CurrentPhase.MaxHealth;
        GameManager.Arena.RadiusInWorldUnits = routine.CurrentPhase.MaxArenaRadius;

        // Heal up to the new max health.
        CombatCore.Entity.HealEntity(self, float.PositiveInfinity);

        // Add i-frames for 3 seconds while we move to center & regain health.
        if (!Chill)
        {
            self.SetInvincible(true);
            Add(new Moves.Basic.Teleport(World.Arena.CENTER).Wait(3f));
            Add(new AISequence(() => { self.SetInvincible(false); }));
        }
        Debug.Log("Queue length: " + queuedSequences.Count);
    }

    public void ResetBoss()
    {
        // Flush the execution engine
        running = false;
        StopAllCoroutines();
        queuedSequences.Clear();

        // Reset the routine and restart it from the first phase
        routine.Reset();

        // Restart the execution engine
        running = true;
        StartCoroutine("ExecuteQueue");

        NextPhase();
    }

    private void Add(AISequence sequence)
    {
        // Debug mode provides additional information when executing an event.
        // TODO: possibly move this into a static analysis done when loading in an AIPhase/AIRoutine.
        if (GameManager.Boss.DebugMode)
        {
            if (sequence == null)
            {
                Debug.LogError("Null AISequence added to queue.");
                return;
            }

            // "glue" AISequences are special: AISequences followed by "Then" or "Wait" 
            // won't have descriptions, but can be identified by being direct instances
            // of the "AISequence" class (vs. subclasses for every other move).
            //
            // These guys don't need to have a valid difficulty or description.
            bool isGlueSequence = sequence.Name.Equals("AISequence");

            // Warn about unnamed sequences. By default, this shouldn't be called; the standard name is valid.
            if (sequence.Name == null) 
            {
                Debug.LogWarning("Found AISequence without a name. Description: " + sequence.Description ?? "not provided.");
            }

            // Warn if there's a named sequence without a description.
            //
            if (sequence.Description == null && !isGlueSequence)
            {
                Debug.LogWarning("Found AISequence with a name, but without a description. Name: " + sequence.Name);
            }

            // Warn about default descriptions.
            if (sequence.Description != null && sequence.Description.Equals("Your description here")) 
            {
                Debug.LogWarning("Found AISequence with default description. Name: " + sequence.Name);
            }

            // Warn if there's a sequence with too high a difficulty.
            if (sequence.Difficulty >= 8f)
            {
                Debug.LogWarning("Found AISequence with very high difficulty (" + sequence.Difficulty + "). Name: " + sequence.Name);
            }

            // Warn about default difficulty (-1). Glue sequences can ignore this.
            if (Mathf.Abs(sequence.Difficulty - -1) < 0.01f && !isGlueSequence) 
            {
                Debug.LogWarning("Found AISequence with default difficulty (-1). Name: " + sequence.Name);
            }

            // Warn about invalid difficulty (<= 0). Glue sequences can ignore this.
            if (sequence.Difficulty <= 0f && !isGlueSequence) 
            {
                Debug.LogWarning("Found AISequence with invalid difficulty (<= 0). Name: " + sequence.Name);
            }

            // If we (finally) get to the end and still have a valid sequence, then we print out what it does.
            Debug.Log("Added AISequence" +
                      (sequence.Name.Equals("AISequence") ? " " : " \"" + sequence.Name + "\" ") +
                      "to queue. Here's what it says it'll do: \"" +
                      (sequence.Description ?? sequence.ToString()) + "\".");
        }

        queuedSequences.Enqueue(sequence);
    }

    void Update()
    {
        if (Chill) {
            return;
        }

        // If the queue ran out of events, pull the next AISequence in the phase
        if (queuedSequences.Count == 0)
        {
            Add(routine.CurrentPhase.GetNext());
        }
    }


    /// <summary>
    /// The main "thread" that executes the next event on the event queue.
    /// If there aren't any sequences, then we spin until there are some.
    /// 
    /// TODO: either here or in "Execute", have some way to interrupt the current
    /// executing AISequence so the next phase can be started immediately. Also so
    /// that if the player interrupts, we can do something about it.
    /// </summary>
    private IEnumerator ExecuteQueue()
    {
        while (running)
        {
            //Profiler.BeginSample("Event Queue");
            while (queuedSequences.Count == 0)
            {
                yield return new WaitForSeconds(0.05f);
            }

            AISequence nextSequence = queuedSequences.Peek();
            yield return Execute(nextSequence);
            queuedSequences.Dequeue();
            //Profiler.EndSample();
        }
    }

    /// <summary>
    /// Executes a given AISequence. This will wait for the duration specified
    /// in any events that make up this sequence. This will also respect the 
    /// "paused" variable, and wait until it is true before continuing to the
    /// next event.
    /// </summary>
    /// <param name="sequence">The sequence to be executed.</param>
    private IEnumerator Execute(AISequence sequence)
    {
        if (sequence.events != null)
        {
            for (int i = 0; i < sequence.events.Length; i++)
            {
                // If the event queue is paused, then wait until it's unpaused.
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

    /// <summary>
    /// Runs an AISequence immediately. Used primarily for callbacks that can't 
    /// be properly scheduled on the event queue.
    /// </summary>
    /// <param name="sequence">The sequence to be executed immediately.</param>
    public void ExecuteAsync(AISequence sequence)
    {
        GameManager.Boss.StartCoroutine(Execute(sequence));
    }

    /*
     * Dashes to the provided position. 
     * 
     * This happens separately from the event queue, and will pause any future
     * events until after this dash is completed.
     */
    public IEnumerator Dash(Vector3 targetPosition)
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
    public void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(GameManager.Player.transform.position - GameManager.Boss.transform.position);
        GameManager.Boss.transform.rotation = lookRotation;
    }

}
