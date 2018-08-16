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

    public static Vector3 playerLockPosition;

    public Ability shoot1,
        shoot3,
        shootWave,
        shootHexCurve,
        shootLine,
        shootSweep,
        shootDeathHex,
        teleport,
        strafe,
        cameraMove,
        playerLock;

    private void Awake()
    {
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();

        eventQueue = new EventQueue(self);

        arena = GameObject.Find("Arena");
    }

    // Use this for initialization
    void Start()
    {
        shoot1 = new Ability("shoot1", "", null, 0f, 0, null);
        shoot3 = new Ability("shoot3", "", null, 0f, 0, null);
        shootWave = new Ability("shootWave", "", null, 0f, 0, null);
        shootHexCurve = new Ability("shootHexCurve", "", null, 0f, 0, null);
        shootLine = new Ability("shootLine", "", null, 0f, 0, null);
        shootSweep = new Ability("shootSweep", "", null, 0f, 0, null);
        shootDeathHex = new Ability("shootDeathHex", "", null, 0f, 0, null);

        teleport = new Ability("teleport", "", null, 0f, 0, null);

        strafe = new Ability("strafe", "", null, 0f, 0, null);

        cameraMove = new Ability("cameraMove", "", null, 0f, 0, null);
        playerLock = new Ability("playerLock", "", null, 0f, 0, null);

        self.AddAbility(shoot1);
        self.AddAbility(shoot3);
        self.AddAbility(shootWave);
        self.AddAbility(shootHexCurve);
        self.AddAbility(shootLine);
        self.AddAbility(shootSweep);
        self.AddAbility(shootDeathHex);
        self.AddAbility(teleport);
        self.AddAbility(strafe);
        self.AddAbility(cameraMove);

        AISequence.AddSequence("shoot2waves", new AISequence(
            new AIEvent(0f, Teleport()),
            new AIEvent(0f, ShootWave(25, 90f, 2.5f, speed: Speed.VERY_FAST)),
            new AIEvent(1f, ShootWave(25, 90f, -2.5f, speed: Speed.VERY_FAST))
        ));

        AISequence.AddSequence("shootWaveMiddleGap", new AISequence(
            new AIEvent(0f, ShootWave(25, 60f, -45f, speed: Speed.VERY_FAST)),
            new AIEvent(0f, ShootWave(25, 60f, 45f, speed: Speed.VERY_FAST))
        ));

        AISequence.AddSequence("homingStrafe5", new AISequence(
            new AIEvent(0f, Strafe(true, 5f, 100)),
            new AIEvent(0f, Shoot1(Type.HOMING, Size.MEDIUM))
        ));

        AISequence.AddSequence("homingStrafe10", new AISequence(
            new AIEvent(0f, Strafe(true, 10f, 50)),
            new AIEvent(0f, Shoot1(Type.HOMING, Size.MEDIUM))
        ));

        AISequence.AddSequence("homingStrafe15", new AISequence(
            new AIEvent(0f, Strafe(true, 15f, 30)),
           new AIEvent(0f, Shoot1(Type.HOMING, Size.MEDIUM))
        ));

        AISequence.AddSequence("homingStrafe72", new AISequence(
            new AIEvent(0f, Strafe(true, 65f, 50)),
            new AIEvent(0f, Shoot1(Type.HOMING, Size.MEDIUM))
        ));

        AISequence.AddSequence("lineStrafe30", new AISequence(
            new AIEvent(0.2f, ShootLine(50, 75f, Speed.SNIPE)),
            new AIEvent(0f, Strafe(true, 60f, 50))
        ));

        AISequence.AddSequence("slowWaveCircle", new AISequence(
            new AIEvent(0f, ShootWave(75, 360f, 0f, speed: Speed.MEDIUM_SLOW, size: Size.LARGE)),
            new AIEvent(0.5f, Strafe(true, 60f, 50))
        ));

        AISequence.AddSequence("lineWaveStrafe30", new AISequence(
            new AIEvent(0.1f, ShootWave(75, 360f, 0f, speed: Speed.SLOW, size: Size.LARGE)),
            new AIEvent(0.3f, Strafe(true, 30f, 50)),
            new AIEvent(0.2f, ShootLine(50, 100f, Speed.VERY_FAST, Vector3.zero)),
            new AIEvent(0f, Strafe(true, 30f, 50))
        ));


        //eventQueue.Add(0f, shootHexCurve);

        // Basic attack
        //eventQueue.Add(0.5f, Teleport());
        //eventQueue.AddSequence(AISequence.Repeat(new AIEvent(0.3f, Shoot3(Type.BASIC, Size.SMALL)), 10));
        //eventQueue.Add(100f, Shoot3(Type.HOMING, Size.HUGE));

        //eventQueue.AddRepeat(1f, ShootWave(50, 360, 0, type: Type.HOMING, speed: Speed.FAST), 10);

        //eventQueue.Add(2f, ShootWave(20, 360f, 0f, reverseDirection: true));
        //eventQueue.AddRepeat(0.5f, ShootDeathHex(2f), 10);

        /*
        eventQueue.Add(2f, PlayerLock(true));
        eventQueue.AddSequence(1f, ShootSweep());
        eventQueue.AddSequence(1f, ShootSweep(clockwise: false, startOffset: 30f));
        eventQueue.Add(0f, PlayerLock(false));

        eventQueue.Add(2f, Teleport());
        eventQueue.AddSequence(1f, ShootSweep());
        eventQueue.AddSequence(1f, ShootSweep(clockwise: false, startOffset: 30f));
        */

        // Warmup with basic attacks
        /*
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.2f, Shoot3(), 20);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.1f, Shoot3(), 35);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.05f, Shoot3(), 50);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.05f, Shoot3(), 10);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.05f, Shoot3(), 10);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.05f, Shoot3(), 10);
        eventQueue.Add(0f, Teleport());
        eventQueue.AddRepeat(0.05f, Shoot3(), 10);
        */

        /*
        // Basic attacks with wave at start, middle, and end
        eventQueue.Add(0.5f, Teleport());
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(), 20);
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(), 20);
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));

        // Again
        eventQueue.Add(0f, Teleport());
        eventQueue.Add(0f, ShootWave(150, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(), 20);
        eventQueue.Add(0f, ShootWave(150, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(), 20);
        eventQueue.Add(0f, ShootWave(150, 360f, 0f));

        // Hex curve introduction
        eventQueue.Add(0f, Teleport());
        eventQueue.AddSequenceRepeat(5, "shoot2waves");
        eventQueue.Add(0f, ShootHexCurve(true));
        eventQueue.Add(2.5f, ShootWave(50, 360f, 0f));
        eventQueue.Add(0f, ShootHexCurve(false));
        eventQueue.Add(2.5f, ShootWave(50, 360f, 0f));
        eventQueue.Add(0f, ShootHexCurve(true));
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.Add(1f, ShootHexCurve(false));
        eventQueue.Add(1f, ShootWave(50, 360f, 0f));
        eventQueue.Add(1.5f, ShootWave(50, 360f, 0f));
        eventQueue.Add(0.5f, Teleport());
        eventQueue.AddSequenceRepeat(12, "homingStrafe10");
        eventQueue.AddSequenceRepeat(3, "shoot2waves");

        // Homing strafe (big and small jumps)
        eventQueue.Add(1f, CameraMove(false, new Vector3(0, 17.5f, -35)));
        eventQueue.Add(1f, Teleport(new Vector3(0, 1.31f, 45)));
        eventQueue.AddSequenceRepeat(10, "homingStrafe72");
        eventQueue.Add(1f, Teleport(new Vector3(0, 1.31f, 45)));
        eventQueue.AddSequenceRepeat(45, "homingStrafe15");
        eventQueue.Add(2f, CameraMove(true));

        eventQueue.AddRepeat(1f, ShootDeathHex(1.5f), 10);

        // 12 curve + waves
        eventQueue.Add(1.5f, Teleport(new Vector3(0, 1.31f, 0)));
        eventQueue.Add(0f, ShootHexCurve(true, 0f));
        eventQueue.Add(0.5f, ShootWave(50, 360f, 0f));
        eventQueue.Add(0.5f, ShootHexCurve(true, 30f));
        eventQueue.Add(1f, ShootWave(50, 360f, 0f));
        eventQueue.Add(1f, ShootWave(50, 360f, 0f));

        // Homing strafes + wave shooting
        eventQueue.AddSequenceRepeat(5, "shoot2waves");
        eventQueue.Add(0.2f, Teleport());
        eventQueue.AddSequenceRepeat(15, "homingStrafe15");
        eventQueue.AddSequenceRepeat(2, "shoot2waves");
        eventQueue.Add(0.2f, Teleport());
        eventQueue.AddSequenceRepeat(15, "homingStrafe15");
        eventQueue.AddSequenceRepeat(2, "shoot2waves");

        // Sequence of partial waves, some with gaps
        eventQueue.Add(0.25f, Teleport(new Vector3(0f, 1.31f, 0f)));
        eventQueue.Add(0.75f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.5f, ShootWave(8, 120f, -10f, Speed.VERY_FAST, Size.LARGE)); // "toothy" wave
        eventQueue.Add(0.5f, ShootWave(8, 120f, 10f, Speed.VERY_FAST, Size.LARGE));
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.Add(0.75f, ShootWave(75, 360f, 0f, Speed.VERY_FAST));

        // Next 3 are homing strafes with a wave-gap-wave fire after
        eventQueue.Add(0.25f, Teleport());
        eventQueue.AddSequenceRepeat(12, "homingStrafe5");
        eventQueue.Add(0.25f, Teleport(speed: 200));
        eventQueue.Add(0.5f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.Add(0.25f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));

        eventQueue.Add(0.25f, Teleport());
        eventQueue.AddSequenceRepeat(12, "homingStrafe5");
        eventQueue.Add(0.25f, Teleport(speed: 200));
        eventQueue.Add(0.5f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.Add(0.25f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));

        eventQueue.Add(0.25f, Teleport());
        eventQueue.AddSequenceRepeat(12, "homingStrafe5");
        eventQueue.Add(0.25f, Teleport(speed: 200));
        eventQueue.Add(0.5f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));
        eventQueue.AddSequence(0.5f, "shootWaveMiddleGap");
        eventQueue.Add(0.25f, ShootWave(25, 90f, 0f, Speed.VERY_FAST));


        // Circle of waves
        eventQueue.Add(0.5f, Teleport(new Vector3(-30f, 1.31f, 0)));
        eventQueue.AddSequenceRepeat(6, "slowWaveCircle");
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, ShootLine(50, 75f, Speed.MEDIUM_SLOW, Vector3.left));
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, ShootLine(50, 75f, Speed.MEDIUM_SLOW, Vector3.right));
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, ShootLine(50, 75f, Speed.MEDIUM_SLOW, Vector3.left));
        eventQueue.AddSequenceRepeat(3, "slowWaveCircle");
        eventQueue.Add(0f, ShootLine(50, 75f, Speed.MEDIUM_SLOW, Vector3.right));
        */

        /*
        // Jump rope warmup
        // Kind of easy and boring?
        eventQueue.Add(2f, Teleport(new Vector3(-45f, 1.31f, 0)));
        eventQueue.Add(1f, ShootLine(speed: Speed.FAST));
        eventQueue.Add(0.5f, Teleport(new Vector3(45f, 1.31f, 0)));
        eventQueue.Add(1f, ShootLine(speed: Speed.FAST));
        eventQueue.Add(0f, Teleport(new Vector3(-45f, 1.31f, 0)));
        eventQueue.Add(1f, ShootLine(speed: Speed.FAST));
        eventQueue.Add(0f, Teleport(new Vector3(45f, 1.31f, 0)));
        eventQueue.Add(1f, ShootLine(speed: Speed.FAST));
        */

        // Jump rope fast
        eventQueue.Add(1f, CameraMove(false, new Vector3(0, 17.5f, -35)));
        eventQueue.Add(0f, Teleport(new Vector3(-45f, 1.31f, 0), 200));
        eventQueue.Add(0f, ShootLine(50, 100f, Speed.SNIPE));
        eventQueue.Add(0f, Teleport(new Vector3(45f, 1.31f, 0), 200));
        eventQueue.Add(0f, ShootLine(50, 100f, Speed.SNIPE));
        eventQueue.Add(0f, Teleport(new Vector3(-45f, 1.31f, 0), 200));
        eventQueue.Add(0f, ShootLine(50, 100f, Speed.SNIPE));
        eventQueue.Add(0f, Teleport(new Vector3(45f, 1.31f, 0), 200));
        eventQueue.Add(0f, ShootLine(50, 100f, Speed.SNIPE));
        eventQueue.Add(0f, CameraMove(true));

        // A very painful double hex curve, with 360 waves and targeted shooting
        eventQueue.Add(1f, Teleport(Vector3.zero));
        eventQueue.Add(0.5f, ShootHexCurve(true, 0f));
        eventQueue.Add(0.5f, ShootHexCurve(true, 30f));
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.05f, Shoot3(Type.BASIC, Size.MEDIUM), 20);
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.05f, Shoot3(Type.BASIC, Size.MEDIUM), 20);
        eventQueue.Add(0f, PlayerLock());
        eventQueue.AddSequence(ShootSweep(clockwise: false, size: Size.MEDIUM));
        eventQueue.Add(0f, ShootHexCurve(false, 0f));
        eventQueue.Add(0f, ShootHexCurve(false, 30f));
        eventQueue.AddRepeat(0.1f, Shoot3(Type.HOMING, Size.MEDIUM), 10);
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(Type.HOMING, Size.MEDIUM), 5);
        eventQueue.Add(0f, ShootWave(50, 360f, 0f));
        eventQueue.AddRepeat(0.1f, Shoot3(Type.HOMING, Size.MEDIUM), 5);
        eventQueue.Add(0.5f, ShootWave(50, 360f, 0f));
        eventQueue.Add(0.5f, ShootWave(50, 360f, 0f));

        // Jump rope circle with wave circles (hard to dodge both at once)
        eventQueue.Add(0f, Teleport(new Vector3(-45f, 1.31f, 0)));
        eventQueue.AddSequenceRepeat(6, "lineStrafe30");
        eventQueue.AddSequenceRepeat(6, "lineWaveStrafe30");
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 playerPos = player.transform.position;

        eventQueue.Update();
    }

    public void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        transform.rotation = lookRotation;
    }

    private Ability Shoot1(Type type = Type.BASIC, Size size = Size.SMALL, Vector3? target = null, float angleOffset = 0f)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            Type t = type;
            Size s = size;

            Glare();

            Vector3 targetPos = player.transform.position;
            if (target.HasValue)
            {
                targetPos = target.Value;
            }

            switch (type)
            {
                case Type.BASIC:
                    Projectile.spawnBasic(self, transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
                case Type.HOMING:
                    Projectile.spawnHoming(self, transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
                case Type.CURVING:
                    Speed speed = Speed.FAST;
                    Projectile.spawnCurving(self, transform.position, targetPos, (float)speed * 2f, 0, speed: speed, size: size, angleOffset: angleOffset);
                    break;
                default:
                    Projectile.spawnBasic(self, transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
            }

            return true;
        };
        Ability newAbility = new Ability(shoot1);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    private Ability Shoot3(Type type = Type.BASIC, Size size = Size.SMALL)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
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
        };
        Ability newAbility = new Ability(shoot3);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    // Shoots an arc of bullets
    private Ability ShootWave(int amount = 1, float arcWidth = 360f, float offset = 0f, Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM, Type type = Type.BASIC, bool reverseDirection = false)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            Glare();

            float halfArcWidth = -arcWidth / 2f;

            for (int i = 0; i < amount; i++)
            {
                
                Vector3 source = reverseDirection ? player.transform.position + new Vector3(0, 1.31f, 0) : transform.position;
                Vector3 sink = player.transform.position;
                Vector3 direction = transform.position - player.transform.position;

                float angleOffset = halfArcWidth + offset + (i * (arcWidth / amount));

                switch(type) {
                    case Type.BASIC:
                    case Type.INDESTRUCTIBLE:
                        Projectile.spawnBasic(
                            self,
                            source,// + Quaternion.AngleAxis(angleOffset, Vector3.up) * (10 * direction.normalized),
                            sink,
                            angleOffset: reverseDirection ? 0 : angleOffset,
                            speed: speed,
                            size: size
                        );
                        break;
                    case Type.HOMING:
                        Projectile.spawnHoming(
                            self,
                            source,// + Quaternion.AngleAxis(angleOffset, Vector3.up) * (10 * Vector3.back),
                            sink,
                            angleOffset: angleOffset,
                            speed: speed,
                            size: size
                        );
                        break;
                }
            }

            return true;
        };
        Ability newAbility = new Ability(shootWave);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    private Ability ShootHexCurve(bool clockwise = true, float offset = 0f)
    {
        return ShootHexCurve(clockwise, offset, new Vector3(0, 1.31f, -1f));
    }

    // Shoots a hexagonal pattern of curving projectiles.
    private Ability ShootHexCurve(bool clockwise, float offset, Vector3 target) {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            float multiplier = clockwise ? 1f : -1f;

            Speed speed = Speed.FAST;
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (0 * multiplier), speed);
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (60 * multiplier), speed);
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (120 * multiplier), speed);
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (180 * multiplier), speed);
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (240 * multiplier), speed);
            Projectile.spawnCurving(self, transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (300 * multiplier), speed);

            return true;
        };
        Ability newAbility = new Ability(shootWave);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    public Ability ShootLine(int amount = 50, float width = 75f, Speed speed = Speed.MEDIUM, Vector3? target = null) {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            Vector3 targetPos = target.HasValue ? target.Value - transform.position : player.transform.position - transform.position;
            Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * targetPos).normalized;

            for (int i = 0; i < amount; i++)
            {
                Vector3 spawn = transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
                Projectile.spawnBasic(
                    self,
                    spawn,
                    spawn + targetPos,
                    speed: speed,
                    size: Size.MEDIUM
                );
            }

            return true;
        };
        Ability newAbility = new Ability(shootWave);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    public AISequence ShootSweep(int amount = 15, bool clockwise = true, float startOffset = -30f, 
                                 float degrees = 90f, Speed speed = Speed.MEDIUM, Size size = Size.SMALL, Type type = Type.BASIC)
    {
        AIEvent[] sweepEvents = new AIEvent[amount];
        Vector3 initialPlayerPosition = BossController.playerLockPosition;
        for (int i = 0; i < amount; i++)
        {
            float offset = startOffset + ((clockwise ? 1f : -1f) * i * (degrees / amount));
            sweepEvents[i] = new AIEvent(0f, Shoot1(target: initialPlayerPosition, angleOffset: offset));
        }
        return new AISequence(sweepEvents);
    }

    public Ability ShootDeathHex(float maxTime1 = 1f)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            Projectile.spawnDeathHex(self, transform.position, player.transform.position, maxTime1);
            return true;
        };
        Ability newAbility = new Ability(shootDeathHex);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    public Ability Teleport(Vector3? target = null, int speed = 100) {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            if (target.HasValue)
            {
                self.movespeed.SetBase(speed);
                StartCoroutine(Dashing(target.Value));
                Glare();
                return true;
            }

            float minDistance = 35f;
            float minAngle = 50f;
            float maxAngle = 100f;

            Vector3 oldPosVector = transform.position - player.transform.position;

            int count = 0;
            Vector3 rawPosition;
            do
            {
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
        };
        Ability newAbility = new Ability(teleport);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
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

    public Ability Strafe(bool clockwise = true, float degrees = 10f, int speed = 100, Vector3 center = default(Vector3))
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            self.movespeed.SetBase(speed);

            Vector3 oldPosVector = transform.position - center;
            Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

            StartCoroutine(Dashing(rot * oldPosVector));
            return true;
        };
        Ability newAbility = new Ability(strafe);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    public Ability CameraMove(bool isFollow = false, Vector3? targetPosition = null)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            CameraController.GetInstance().IsFollowing = isFollow;

            if (targetPosition.HasValue)
            {
                CameraController.GetInstance().Goto(targetPosition.Value, 1);
            }

            return true;
        };
        Ability newAbility = new Ability(strafe);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

    public Ability PlayerLock(bool enableLock = true)
    {
        Ability.UseEffect deleg = (sub, pos, args) =>
        {
            if (enableLock)
            {
                playerLockPosition = player.transform.position;
            }
            return true;
        };
        Ability newAbility = new Ability(strafe);
        newAbility.Effect = deleg;
        self.AddAbility(newAbility);
        return newAbility;
    }

}
