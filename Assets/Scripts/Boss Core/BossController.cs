using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;
using Projectiles;

public class BossController : MonoBehaviour
{

    private GameObject player;

    private Rigidbody physbody;
    public CombatCore.Entity self;

    private EventQueue eventQueue;

    private GameObject arena;

    public Ability shoot1,
        shoot3, 
        shootWave,
        shootHexCurve,
        shootLine,
        teleport,
        strafe,
        cameraMove;

    private void Awake() {
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();

        eventQueue = new EventQueue(self);

        arena = GameObject.Find("Arena");
	}

	// Use this for initialization
    void Start () {
        shoot1 = new Ability("shoot1", "", null, 0f, 0, Shoot1);
        shoot3 = new Ability("shoot3", "", null, 0f, 0, Shoot3);
        shootWave = new Ability("shootWave", "", null, 0f, 0, ShootWave);
        shootHexCurve = new Ability("shootHexCurve", "", null, 0f, 0, ShootHexCurve);
        shootLine = new Ability("shootLine", "", null, 0f, 0, ShootLine);

        teleport = new Ability("teleport", "", null, 0f, 0, Teleport);

        strafe = new Ability("strafe", "", null, 0f, 0, Strafe);

        cameraMove = new Ability("cameraMove", "", null, 0f, 0, CameraMove);

        self.AddAbility(shoot1);
        self.AddAbility(shoot3);
        self.AddAbility(shootWave);
        self.AddAbility(shootHexCurve);
        self.AddAbility(shootLine);
        self.AddAbility(teleport);
        self.AddAbility(strafe);
        self.AddAbility(cameraMove);

        AISequence.AddSequence("shoot2waves", new AISequence(
            new AIEvent(0f, teleport),
            new AIEvent(0f, shootWave, 25, 90f, 2.5f, Speed.VERY_FAST),
            new AIEvent(1f, shootWave, 25, 90f, -2.5f, Speed.VERY_FAST)
        ));

        AISequence.AddSequence("homingStrafe5", new AISequence(
            new AIEvent(0f, strafe, true, 5f, 100),
            new AIEvent(0f, shoot1, Type.HOMING)
        ));

        AISequence.AddSequence("homingStrafe10", new AISequence(
            new AIEvent(0f, strafe, true, 10f, 50),
            new AIEvent(0f, shoot1, Type.HOMING)
        ));

        AISequence.AddSequence("homingStrafe15", new AISequence(
            new AIEvent(0f, strafe, true, 15f, 30),
            new AIEvent(0f, shoot1, Type.HOMING)
        ));

        AISequence.AddSequence("homingStrafe72", new AISequence(
            new AIEvent(0f, strafe, true, 65f, 30),
            new AIEvent(0f, shoot1, Type.HOMING)
        ));

        AISequence.AddSequence("lineStrafe30", new AISequence(
            new AIEvent(0.2f, shootLine, 50, 75f, Speed.SNIPE),
            new AIEvent(0f, strafe, true, 60f, 50)
        ));

        AISequence.AddSequence("slowWaveCircle", new AISequence(
            new AIEvent(0f, shootWave, 75, 360f, 0f, Speed.SLOW, Size.LARGE),
            new AIEvent(0.5f, strafe, true, 60f, 50)
        ));

        AISequence.AddSequence("lineWaveStrafe30", new AISequence(
            new AIEvent(0.1f, shootWave, 75, 360f, 0f, Speed.SLOW, Size.LARGE),
            new AIEvent(0.3f, strafe, true, 30f, 50),
            new AIEvent(0.2f, shootLine, 50, 75f, Speed.VERY_FAST, Vector3.zero),
            new AIEvent(0f, strafe, true, 30f, 50)
        ));

        //eventQueue.Add(0f, shootHexCurve);

        // Basic attack
        eventQueue.Add(0.5f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.3f, shoot3, Type.BASIC, Size.SMALL), 10));

        // Warmup with basic attacks
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.2f, shoot3), 20));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.1f, shoot3), 35));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.05f, shoot3), 50));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.05f, shoot3), 10));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.05f, shoot3), 10));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.05f, shoot3), 10));
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequence(0.5f, AISequence.Repeat(new AIEvent(0.05f, shoot3), 10));

        // Basic attacks with wave at start, middle, and end
        eventQueue.Add(0.5f, teleport);
        eventQueue.Add(0f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.1f, shoot3), 20));
        eventQueue.Add(0f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.1f, shoot3), 20));
        eventQueue.Add(0f, shootWave, 50, 360f, 0f, Speed.MEDIUM);

        // Again
        eventQueue.Add(0f, teleport);
        eventQueue.Add(0f, shootWave, 150, 360f, 0f, Speed.MEDIUM);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.1f, shoot3), 20));
        eventQueue.Add(0f, shootWave, 150, 360f, 0f, Speed.MEDIUM);
        eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.1f, shoot3), 20));
        eventQueue.Add(0f, shootWave, 150, 360f, 0f, Speed.MEDIUM);

        // Hex curve introduction
        eventQueue.Add(0f, teleport);
        eventQueue.AddSequenceRepeat(5, "shoot2waves");
        eventQueue.Add(0f, shootHexCurve, true);
        eventQueue.Add(2.5f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(0f, shootHexCurve, false);
        eventQueue.Add(2.5f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(0f, shootHexCurve, true);
        eventQueue.Add(0f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(1f, shootHexCurve, false);
        eventQueue.Add(1f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(1.5f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(0.5f, teleport);
        eventQueue.AddSequenceRepeat(12, "homingStrafe10");
        eventQueue.AddSequenceRepeat(3, "shoot2waves");

        // Homing strafe (big and small jumps)
        eventQueue.Add(1f, cameraMove, false, new Vector3(0, 17.5f, -35));
        eventQueue.Add(1f, teleport, new Vector3(0, 1.31f, 45));
        eventQueue.AddSequenceRepeat(10, "homingStrafe72");
        eventQueue.Add(1f, teleport, new Vector3(0, 1.31f, 45));
        eventQueue.AddSequenceRepeat(45, "homingStrafe15");
        eventQueue.Add(2f, cameraMove, true);

        // 12 curve + waves
        eventQueue.Add(0.5f, teleport, new Vector3(0, 1.31f, 0));
        eventQueue.Add(0f, shootHexCurve, true, 0f);
        eventQueue.Add(0.5f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(0.5f, shootHexCurve, true, 30f);
        eventQueue.Add(1f, shootWave, 50, 360f, 0f, Speed.MEDIUM);
        eventQueue.Add(1f, shootWave, 50, 360f, 0f, Speed.MEDIUM);

        // Homing strafes + wave shooting
        eventQueue.AddSequenceRepeat(5, "shoot2waves");
        eventQueue.Add(0.2f, teleport);
        eventQueue.AddSequenceRepeat(15, "homingStrafe15");
        eventQueue.AddSequenceRepeat(2, "shoot2waves");
        eventQueue.Add(0.2f, teleport);
        eventQueue.AddSequenceRepeat(15, "homingStrafe15");
        eventQueue.AddSequenceRepeat(2, "shoot2waves");

        eventQueue.Add(0.5f, teleport, new Vector3(-30f, 1.31f, 0));

        // Circle of waves
        eventQueue.AddSequenceRepeat(6, "slowWaveCircle");
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, shootLine, 50, 75f, Speed.SLOW, Vector3.left);
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, shootLine, 50, 75f, Speed.SLOW, Vector3.right);
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, shootLine, 50, 75f, Speed.SLOW, Vector3.left);
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, shootLine, 50, 75f, Speed.SLOW, Vector3.right);

        // Jump rope warmup
        eventQueue.Add(2f, teleport, new Vector3(-45f, 1.31f, 0));
        eventQueue.Add(1f, shootLine);
        eventQueue.Add(0.5f, teleport, new Vector3(45f, 1.31f, 0));
        eventQueue.Add(1f, shootLine);
        eventQueue.Add(0f, teleport, new Vector3(-45f, 1.31f, 0));
        eventQueue.Add(1f, shootLine);
        eventQueue.Add(0f, teleport, new Vector3(45f, 1.31f, 0));
        eventQueue.Add(1f, shootLine);

        // Jump rope fast
        eventQueue.Add(1f, cameraMove, false, new Vector3(0, 17.5f, -35));
        eventQueue.Add(0f, teleport, new Vector3(-45f, 1.31f, 0), 200);
        eventQueue.Add(0.5f, shootLine, 50, 75f, Speed.VERY_FAST);
        eventQueue.Add(0f, teleport, new Vector3(45f, 1.31f, 0), 200);
        eventQueue.Add(0.5f, shootLine, 50, 75f, Speed.VERY_FAST);
        eventQueue.Add(0f, teleport, new Vector3(-45f, 1.31f, 0), 200);
        eventQueue.Add(0.5f, shootLine, 50, 75f, Speed.VERY_FAST);
        eventQueue.Add(0f, teleport, new Vector3(45f, 1.31f, 0), 200);
        eventQueue.Add(0.5f, shootLine, 50, 75f, Speed.VERY_FAST);

        // Jump rope circle with wave circles
        eventQueue.Add(0f, teleport, new Vector3(-45f, 1.31f, 0));
        eventQueue.AddSequenceRepeat(6, "lineStrafe30");
        eventQueue.AddSequenceRepeat(6, "lineWaveStrafe30");
        eventQueue.Add(2f, cameraMove, true);

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

        Type type = args.Length > 0 ? (Type)args[0] : Type.BASIC;
        Size size = args.Length > 1 ? (Size)args[1] : Size.SMALL;

        switch(type) {
            case Type.BASIC:
                Projectile.spawnBasic(self, transform.position, player.transform.position, size: size);
                break;
            case Type.HOMING:
                Projectile.spawnHoming(self, transform.position, player.transform.position, size: size);
                break;
            case Type.CURVING:
                Speed speed = Speed.FAST;
                Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * 2f, 0, speed: speed, size: size);
                break;
            default: 
                Projectile.spawnBasic(self, transform.position, player.transform.position, size: size);
                break;
        }

        return true;
    }

    delegate bool sampleDel(Entity subject, Vector3 targetPosition, params object[] args);

    private Ability Shoot1Generator(Type type, Size size) {
        sampleDel deleg = (sub, pos, args) =>
        {
            Type t = type;
            Size s = size;
            return true;
        };
        Ability newAbility = new Ability(shoot1);
        //newAbility.UseEffect = deleg;
        return newAbility;
    }

    // Shoots a 3 bullets in the direction of the player, and +/- 30 degrees
    public bool Shoot3(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();

        Type type = args.Length > 0 ? (Type)args[0] : Type.BASIC;
        Size size = args.Length > 1 ? (Size)args[1] : Size.SMALL;

        switch (type)
        {
            case Type.BASIC:
                Projectile.spawnBasic(self, transform.position, player.transform.position, size: size);
                Projectile.spawnBasic(self, transform.position, player.transform.position, angleOffset: -30, size: size);
                Projectile.spawnBasic(self, transform.position, player.transform.position, angleOffset: 30, size: size);
                break;
            case Type.HOMING:
                Projectile.spawnHoming(self, transform.position, player.transform.position, size: size);
                Projectile.spawnHoming(self, transform.position, player.transform.position, angleOffset: -30, size: size);
                Projectile.spawnHoming(self, transform.position, player.transform.position, angleOffset: 30, size: size);
                break;
            default:
                Projectile.spawnBasic(self, transform.position, player.transform.position, size: size);
                Projectile.spawnBasic(self, transform.position, player.transform.position, angleOffset: -30, size: size);
                Projectile.spawnBasic(self, transform.position, player.transform.position, angleOffset: 30, size: size);
                break;
        }
        return true;
    }

    // Shoots an arc of bullets
    // Params: [amount, arc width (degrees), offset (degrees), size, speed, type]
    public bool ShootWave(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();

        int amount = args.Length > 0 ? (int)args[0] : 1;
        float arcWidth = args.Length > 1 ? (float)args[1] : 360;
        float halfArcWidth = -arcWidth / 2f;

        float offset = args.Length > 2 ? (float)args[2] : 0f;

        Speed speed = args.Length > 3 ? (Speed)args[3] : Speed.MEDIUM;
        Size size = args.Length > 4 ? (Size)args[4] : Size.MEDIUM;


        //Debug.Log("Shooting wave with " + amount + " projectiles and spread of " + arcWidth);

        for (int i = 0; i < amount; i++) {
            Projectile.spawnBasic(
                self,
                transform.position,
                player.transform.position,
                angleOffset: halfArcWidth + offset + (i * (arcWidth / amount)),
                speed: speed,
                size: size
            );
        }

        return true;
    }

    public bool ShootHexCurve(Entity subject, Vector3 targetPosition, params object[] args) {
        
        bool clockwise = args.Length > 0 ? (bool)args[0] : true;
        float multiplier = clockwise ? 1f : -1f;

        float offset = args.Length > 1 ? (float)args[1] : 0;

        Speed speed = Speed.FAST;
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (0 * multiplier), speed);
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (60 * multiplier), speed);
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (120 * multiplier), speed);
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (180 * multiplier), speed);
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (240 * multiplier), speed);
        Projectile.spawnCurving(self, transform.position, player.transform.position, (float)speed * multiplier * 2f, 3f, offset + (300 * multiplier), speed);

        return true;
    }

    public bool ShootLine(Entity subject, Vector3 targetPosition, params object[] args) {

        int amount = args.Length > 0 ? (int)args[0] : 50;
        float width = args.Length > 1 ? (float)args[1] : 75f;
        Speed speed = args.Length > 2 ? (Speed)args[2] : Speed.FAST;
        Vector3 direction = args.Length > 3 ? (Vector3)args[3] - transform.position : (player.transform.position - transform.position);

        Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * direction).normalized;


        for (int i = 0; i < amount; i++) {
            Vector3 spawn = transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
            Projectile.spawnBasic(
                self,
                spawn,
                spawn + direction,
                speed: speed,
                size: Size.MEDIUM
            );
        }

        return true;
    }

    public bool Teleport(Entity subject, Vector3 targetPosition, params object[] args) {

        if (args.Length > 0)
        {
            Vector3 target = (Vector3)args[0];

            if (args.Length > 1) {
                self.movespeed.SetBase((int)args[1]);
            }

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

    // Params: [clockwise (bool), degrees around (float), speed (int)]
    public bool Strafe(Entity subject, Vector3 targetPosition, params object[] args) {

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

    public bool CameraMove(Entity subject, Vector3 targetPosition, params object[] args)
    {

        bool isFollow = (bool)args[0];
        CameraController.GetInstance().IsFollowing = isFollow;

        if (args.Length > 1) {
            Vector3 target = (Vector3)args[1];
            CameraController.GetInstance().Goto(target, 1);
        }

        return true;
    }

}
