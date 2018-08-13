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
    /*
    public static Ability shoot1 = new Ability("shoot1", "", null, 0f, 0, Shoot1);
    public static Ability shoot3;

    public static Ability shootWave = new Ability("shootWave", "", null, 0f, 0, ShootWave);
    public static Ability teleport = new Ability("teleport", "", null, 0f, 0, instanceTeleport);
    */

    public Ability shoot1, shoot3, shootWave, teleport;

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

        self.AddAbility(shoot1);
        self.AddAbility(shoot3);
        self.AddAbility(shootWave);
        self.AddAbility(teleport);

        AISequence.AddSequence("shoot2waves", new AISequence(
            new AIEvent(0f, teleport),
            new AIEvent(0f, shootWave, 25, 90f, 2.5f, ProjectileManager.Speed.VERY_FAST),
            new AIEvent(1f, shootWave, 25, 90f, -2.5f, ProjectileManager.Speed.VERY_FAST)
        ));

        //eventQueue.Add(1.3f, null); // delay at start to wait for cooldowns

        //eventQueue.AddRepeat(100, 1f, shootWave, 25, 120f, 0f, ProjectileManager.Speed.VERY_FAST);
        eventQueue.AddSequenceRepeat(50, "shoot2waves");

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
        bossProjectileManager.spawn1(transform.position, player.transform.position);
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
        transform.position = rawPosition;
        Debug.Log("Took " + count + " iterations.");

        //Debug.Log("Distance: " + distance + " Actual: " + (transform.position - player.transform.position).magnitude);

        //Debug.Log("Distance: " + Vector3.Distance(target, player.transform.position));
        //Debug.Log("Angle: " + angle);
        //Debug.Log("Old pos: " + transform.position + " Player: " + player.transform.position + " New pos: " + target + " Angle: " + angle);

        //transform.position = target;

        Glare();
        return true;
    }

}
