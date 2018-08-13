using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class BossController : MonoBehaviour
{

    private GameObject player;

    private Rigidbody physbody;
    public CombatCore.Entity self;

    private EventQueue eventQueue;

    private ProjectileManager bossProjectileManager;

    private GameObject arena;

    public Ability shoot1,
        shoot3, 
        shootWave, 
        teleport,
        strafe;

    private void Awake() {
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();

        eventQueue = new EventQueue(self);
        bossProjectileManager = new ProjectileManager(self);

        arena = GameObject.Find("Arena");
	}

	// Use this for initialization
    void Start () {
        shoot1 = new Ability("shoot1", "", null, 0f, 0, Shoot1);
        shoot3 = new Ability("shoot3", "", null, 0f, 0, Shoot3);
        shootWave = new Ability("shootWave", "", null, 0f, 0, ShootWave);

        teleport = new Ability("teleport", "", null, 0f, 0, Teleport);

        strafe = new Ability("strafe", "", null, 0f, 0, Strafe);

        self.AddAbility(shoot1);
        self.AddAbility(shoot3);
        self.AddAbility(shootWave);
        self.AddAbility(teleport);
        self.AddAbility(strafe);

        AISequence.AddSequence("shoot2waves", new AISequence(
            new AIEvent(0f, teleport),
            new AIEvent(0f, shootWave, 25, 90f, 2.5f, ProjectileManager.Speed.VERY_FAST),
            new AIEvent(1f, shootWave, 25, 90f, -2.5f, ProjectileManager.Speed.VERY_FAST)
        ));

        AISequence.AddSequence("homingStrafe5", new AISequence(
            new AIEvent(0f, strafe, true, 5f, 100),
            new AIEvent(0f, shoot1)
        ));

        AISequence.AddSequence("homingStrafe10", new AISequence(
            new AIEvent(0f, strafe, true, 10f, 50),
            new AIEvent(0f, shoot1)
        ));

        AISequence.AddSequence("homingStrafe15", new AISequence(
            new AIEvent(0f, strafe, true, 15f, 30),
            new AIEvent(0f, shoot1)
        ));

        AISequence.AddSequence("homingStrafe72", new AISequence(
            new AIEvent(0f, strafe, true, 65f, 30),
            new AIEvent(0f, shoot1)
        ));

        //eventQueue.Add(1.3f, null); // delay at start to wait for cooldowns

        //eventQueue.AddRepeat(100, 1f, shootWave, 25, 120f, 0f, ProjectileManager.Speed.VERY_FAST);
        //eventQueue.AddSequenceRepeat(50, "shoot2waves");
        //eventQueue.AddSequenceRepeat(10, "homingStrafe");

        //eventQueue.Add(2f, null);
        //eventQueue.Add(1f, strafe);
        //eventQueue.AddRepeat(10, 0.0f, shoot1);
        //eventQueue.AddSequence("shoot2waves");
        //eventQueue.Add(0.2f, teleport);
        eventQueue.Add(0.2f, teleport, new Vector3(0, 1.31f, 45));
        eventQueue.AddSequenceRepeat(40, "homingStrafe72");

        /*
        eventQueue.AddSequence("homingStrafe");
        eventQueue.AddSequence("homingStrafe");
        eventQueue.AddSequence("shoot2waves");
        eventQueue.AddSequence("homingStrafe");
        eventQueue.AddSequence("shoot2waves");
        */

        //eventQueue.AddRepeat(1000, 0.2f, shoot1);

        /*
        eventQueue.Add(0f, teleport);
        eventQueue.Add(0.1f, shootWave, 25, 120f, -5f);
        eventQueue.Add(0.8f, shootWave, 25, 120f, 5f);

        eventQueue.Add(0f, teleport);
        eventQueue.Add(0.1f, shootWave, 25, 120f, -5f);
        eventQueue.Add(0.8f, shootWave, 25, 120f, 5f);

        eventQueue.Add(0f, teleport);
        eventQueue.Add(0.1f, shootWave, 25, 120f, -5f);
        eventQueue.Add(0.8f, shootWave, 25, 120f, 5f);
        */

        /*
        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shootWave, 45, 360f, -10f);
        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shoot3);
        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shoot1);
        */
	}
	
	// Update is called once per frame
	void Update () {
        //Vector3 playerPos = player.transform.position;

        eventQueue.Update();
	}

    public void Glare() {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = lookRotation;
    }

    // Shoots a single bullet in the direction of the player
    private bool Shoot1(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();
        bossProjectileManager.spawnHoming(transform.position, player.transform.position);
        return true;
    }

    // Shoots a 3 bullets in the direction of the player, and +/- 30 degrees
    public bool Shoot3(Entity subject, Vector3 targetPosition, params object[] args) {
        Debug.Log("Shoot 3 called!");
        Glare();
        bossProjectileManager.spawn1(transform.position, player.transform.position);
        bossProjectileManager.spawn1(transform.position, player.transform.position, -30);
        bossProjectileManager.spawn1(transform.position, player.transform.position, 30);
        return true;
    }

    // Shoots an arc of bullets
    // Params: [amount, arc width (degrees), offset (degrees), speed]
    // TODO: add default values
    public bool ShootWave(Entity subject, Vector3 targetPosition, params object[] args) {
        //Debug.Log("Shoot wave called! Boss position: " + transform.position);
        Glare();

        int amount = (int)args[0];
        if (amount == 0) amount = 1;

        float arcWidth = (float)args[1];

        float halfArcWidth = -arcWidth / 2f;

        float offset = (float)args[2];

        ProjectileManager.Speed speed = (ProjectileManager.Speed)args[3];

        //Debug.Log("Shooting wave with " + amount + " projectiles and spread of " + arcWidth);

        for (int i = 0; i < amount; i++) {
            bossProjectileManager.spawn1(
                transform.position,
                player.transform.position,
                halfArcWidth + offset + (i * (arcWidth / amount)),
                speed
            );
        }

        return true;
    }

    public bool Teleport(Entity subject, Vector3 targetPosition, params object[] args) {

        if (args.Length > 0)
        {
            Vector3 target = (Vector3)args[0];
            StartCoroutine(Dashing(target));
            Glare();
            return true;
        }

        float minDistance = 35f;
        float minAngle = 50f;
        float maxAngle = 100f;

        Vector3 oldPosVector = transform.position - player.transform.position;

        int count = 0;
        Vector3 rawPosition;
        do {
            count++;
            float degreeRotation = Random.Range(minAngle, maxAngle) * (Random.Range(0, 2) == 0 ? -1 : 1);
            float distance = Random.Range(minDistance * arena.transform.localScale.x, 50f * arena.transform.localScale.x);

            if (count == 15)
            {
                degreeRotation = Random.Range(0, 359f);
                distance = Random.Range(15, 49f);
            }

            Quaternion rot = Quaternion.AngleAxis(degreeRotation, Vector3.up);

            if (count == 15)
            {
                rawPosition = rot * (distance * Vector3.forward);
            }
            else
            {
                rawPosition = (rot * (oldPosVector * (distance / oldPosVector.magnitude))) + player.transform.position;
            }
            rawPosition.y = 0f;

        } while (rawPosition.magnitude > 50f);

        rawPosition.y = 1.31f;
        //transform.position = rawPosition;
        StartCoroutine(Dashing(rawPosition));

        Glare();
        return true;
    }

    private IEnumerator Dashing(Vector3 targetPosition) {

        eventQueue.Pause();
        physbody.velocity = Vector3.zero;

        Vector3 dashDir = (targetPosition - transform.position).normalized;
        float dist;
        while ((dist = Vector3.Distance(targetPosition, transform.position)) > 0f) {
            
            float dashDistance = self.movespeed.Value * 4 * Time.deltaTime;

            if (dist < dashDistance)
            {
                transform.position = targetPosition;
                break;
            }

            transform.position += dashDir * (dashDistance);
            yield return null;
        }

        self.movespeed.SetBase(100);
        eventQueue.Unpause();
    }

    private IEnumerator DashingMany(Vector3[] targets) {
        
        int index = 0;

        while (index != targets.Length) {
            //Debug.Log("Parsing index: " + index);
            physbody.velocity = Vector3.zero;

            Vector3 dashDir = (targets[index] - transform.position).normalized;
            float dist;
            while ((dist = Vector3.Distance(targets[index], transform.position)) > 0f)
            {

                float dashDistance = self.movespeed.Value * 4 * Time.deltaTime;

                if (dist < dashDistance)
                {
                    transform.position = targets[index];
                    index++;
                    break;
                }

                //Debug.DrawLine(transform.position, transform.position + (dashDir * dashDistance), Color.yellow, 1f);
                transform.position += dashDir * (dashDistance);
                yield return null;
            }
        }

    }

    // Params: [clockwise (bool), degrees around (float), speed (int)]
    public bool Strafe(Entity subject, Vector3 targetPosition, params object[] args) {

        Debug.Log("Strafing!");

        bool clockwise = (bool)args[0];

        Vector3 center = Vector3.zero;
        float degrees = (float)args[1];

        int speed = (int)args[2];
        self.movespeed.SetBase(speed);

        Vector3 oldPosVector = transform.position - center;
        Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

        StartCoroutine(Dashing(rot * oldPosVector));
        return true;
    }

}
