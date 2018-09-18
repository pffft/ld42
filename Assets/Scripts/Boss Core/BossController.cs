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
    // A reference to the player object (for position tracking)
    private static GameObject player;

    // A reference to our physics body (for self movement and dashing)
    private static Rigidbody physbody;

    // A reference to the game arena.
    private static GameObject arena;

    // The event queue. This is filled with sequences representing future actions.
    private static EventQueue eventQueue;

    #region Arena Location constants
    public static float BOSS_HEIGHT = 1.31f;
    private static float FAR = 45f;
    private static float MED = 30f;
    private static float CLOSE = 15f;

    public static Vector3 CENTER = new Vector3(0, BOSS_HEIGHT, 0);

    public static Vector3 NORTH_FAR = new Vector3(0f, BOSS_HEIGHT, 45f);
    public static Vector3 SOUTH_FAR = new Vector3(0f, BOSS_HEIGHT, -45f);
    public static Vector3 EAST_FAR = new Vector3(45f, BOSS_HEIGHT, 0);
    public static Vector3 WEST_FAR = new Vector3(-45f, BOSS_HEIGHT, 0);

    public static Vector3 NORTH_MED = new Vector3(0f, BOSS_HEIGHT, 30f);
    public static Vector3 SOUTH_MED = new Vector3(0f, BOSS_HEIGHT, -30f);
    public static Vector3 EAST_MED = new Vector3(30f, BOSS_HEIGHT, 0);
    public static Vector3 WEST_MED = new Vector3(-30f, BOSS_HEIGHT, 0);

    public static Vector3 NORTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, 15f);
    public static Vector3 SOUTH_CLOSE = new Vector3(0f, BOSS_HEIGHT, -15f);
    public static Vector3 EAST_CLOSE = new Vector3(15f, BOSS_HEIGHT, 0);
    public static Vector3 WEST_CLOSE = new Vector3(-15f, BOSS_HEIGHT, 0);
    #endregion

    // The Entity representing this faction. Assigned to projectiles we create.
    public static CombatCore.Entity self;

    // Used for the "CameraLock" action. Keeps track of the current player position
    // for events and sequences that need a slightly out of date version.
    public static Vector3 playerLockPosition;
    public static bool isPlayerLocked = false;

    #region Singleton stuff
    public static BossController instance = null;
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
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();

        eventQueue = new EventQueue(self);

        arena = GameObject.Find("Arena");
    }

    // Use this for initialization
    void Start()
    {
        Profiler.BeginSample("Initialize event queue");

        // 4 way sweep, with waves and homing bullets; then a reversal and speedup
        // TODO: Find a way to turn this into an AISequence using a method!!!
        /*
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                eventQueue.Add(ShootWave(4, 360, j * 6f, target: Vector3.forward).Wait(0.1f));
            }
            eventQueue.Add(Shoot1(Type.HOMING, Size.LARGE));
            for (int j = 7; j < 15; j++)
            {
                eventQueue.Add(ShootWave(4, 360, j * 6f, target: Vector3.forward).Wait(0.1f));
            }
            eventQueue.AddSequence(AISequence.SHOOT_360);
        }
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                eventQueue.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
            }
            eventQueue.Add(Shoot1(Type.HOMING, Size.LARGE));
            for (int j = 5; j < 10; j++)
            {
                eventQueue.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
            }
            eventQueue.Add(Shoot1(Type.HOMING, Size.LARGE));
            for (int j = 10; j < 15; j++)
            {
                eventQueue.Add(ShootWave(4, 360, j * -6f, target: Vector3.forward).Wait(0.1f));
            }
            eventQueue.AddSequence(AISequence.SHOOT_360);
        }

        eventQueue.Add(Teleport(CENTER).Wait(2f));

        // Experimental sweep
        // TODO: Find a way to turn this into an AISequence using a method!!!
        eventQueue.Add(PlayerLock(true));
        for (int i = -30; i < 90; i += 5)
        {
            eventQueue.Add(Shoot1(angleOffset: i).Wait(0.01f));
        }
        for (int i = 30; i >= -90; i -= 5)
        {
            eventQueue.Add(Shoot1(angleOffset: i).Wait(0.01f));
        }
        eventQueue.Add(PlayerLock(false));
        */

        /*
        phase = AIPhase.PHASE1;
        for (int i = 0; i < 10; i++) {
            eventQueue.AddSequence(phase.GetNext());
        }
        */


        Profiler.EndSample();
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 playerPos = player.transform.position;

        eventQueue.Update();
    }

    public static void Glare()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - instance.transform.position);
        instance.transform.rotation = lookRotation;
    }

    public static AIEvent Shoot1(Type type = Type.BASIC, Size size = Size.SMALL, Vector3? target = null, float angleOffset = 0f)
    {
        return new AIEvent(0f, () =>
        {
            Glare();

            Vector3 targetPos = player.transform.position;
            if (isPlayerLocked)
            {
                targetPos = playerLockPosition;
            }
            if (target.HasValue)
            {
                targetPos = target.Value;
            }

            switch (type)
            {
                case Type.BASIC:
                    Projectile.spawnBasic(self, instance.transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
                case Type.HOMING:
                    Projectile.spawnHoming(self, instance.transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
                case Type.CURVING:
                    Speed speed = Speed.FAST;
                    Projectile.spawnCurving(self, instance.transform.position, targetPos, (float)speed * 2f, 0, speed: speed, size: size, angleOffset: angleOffset);
                    break;
                default:
                    Projectile.spawnBasic(self, instance.transform.position, targetPos, size: size, angleOffset: angleOffset);
                    break;
            }
        });
    }

    public static AIEvent Shoot3(Type type = Type.BASIC, Size size = Size.SMALL)
    {
        return new AIEvent(0f, () =>
        {
            switch (type)
            {
                case Type.BASIC:
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, size: size);
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, angleOffset: -30, size: size);
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, angleOffset: 30, size: size);
                    break;
                case Type.HOMING:
                    Projectile.spawnHoming(self, instance.transform.position, player.transform.position, size: size);
                    Projectile.spawnHoming(self, instance.transform.position, player.transform.position, angleOffset: -30, size: size);
                    Projectile.spawnHoming(self, instance.transform.position, player.transform.position, angleOffset: 30, size: size);
                    break;
                default:
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, size: size);
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, angleOffset: -30, size: size);
                    Projectile.spawnBasic(self, instance.transform.position, player.transform.position, angleOffset: 30, size: size);
                    break;
            }
        });
    }

    // Shoots an arc of bullets
    public static AIEvent ShootWave(int amount = 1, float arcWidth = 360f, float offset = 0f, Speed speed = Speed.MEDIUM, Size size = Size.MEDIUM, Type type = Type.BASIC, Vector3? target = null)
    {
        return new AIEvent(0f, () =>
        {
            Glare();

            float halfArcWidth = -arcWidth / 2f;

            for (int i = 0; i < amount; i++)
            {
                
                Vector3 source = instance.transform.position;
                Vector3 sink = target.HasValue ? target.Value : player.transform.position;
                Vector3 direction = instance.transform.position - player.transform.position;

                float angleOffset = halfArcWidth + offset + (i * (arcWidth / amount));

                switch(type) {
                    case Type.BASIC:
                    case Type.INDESTRUCTIBLE:
                        Projectile.spawnBasic(
                            self,
                            source,// + Quaternion.AngleAxis(angleOffset, Vector3.up) * (10 * direction.normalized),
                            sink,
                            angleOffset: angleOffset,
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
        });
    }

    public static AIEvent ShootHexCurve(bool clockwise = true, float offset = 0f)
    {
        return ShootHexCurve(clockwise, offset, new Vector3(0, 1.31f, -1f));
    }

    // Shoots a hexagonal pattern of curving projectiles.
    public static AIEvent ShootHexCurve(bool clockwise, float offset, Vector3 target) {
        return new AIEvent(0f, () =>
        {
            float multiplier = clockwise ? 1f : -1f;

            Speed speed = Speed.FAST;
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (0 * multiplier), speed);
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (60 * multiplier), speed);
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (120 * multiplier), speed);
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (180 * multiplier), speed);
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (240 * multiplier), speed);
            Projectile.spawnCurving(self, instance.transform.position, target, (float)speed * multiplier * 2f, 3f, offset + (300 * multiplier), speed);
        });
    }

    public static AIEvent ShootLine(int amount = 50, float width = 75f, Speed speed = Speed.MEDIUM, Vector3? target = null) {
        return new AIEvent(0f, () =>
        {

            Vector3 targetPos = target.HasValue ? target.Value - instance.transform.position : player.transform.position - instance.transform.position;
            Vector3 leftDirection = (Quaternion.AngleAxis(90, Vector3.up) * targetPos).normalized;

            for (int i = 0; i < amount; i++)
            {
                Vector3 spawn = instance.transform.position + ((i - (amount / 2f)) * (width / amount) * leftDirection);
                Projectile.spawnBasic(
                    self,
                    spawn,
                    spawn + targetPos,
                    speed: speed,
                    size: Size.MEDIUM
                );
            }
        });
    }

    public static AIEvent ShootDeathHex(float maxTime1 = 1f)
    {
        return new AIEvent(0f, () =>
        {

            for (int i = 0; i < 6; i++)
            {
                Projectile.spawnDeathHex(self, instance.transform.position, player.transform.position, maxTime1, i * 60f);
            }
        });
    }

    public static AIEvent Teleport(Vector3? target = null, int speed = 100) {
        return new AIEvent(0f, () =>
        {

            if (target.HasValue)
            {
                self.movespeed.SetBase(speed);
                instance.StartCoroutine(Dashing(target.Value));
                Glare();
                return;
            }

            float minDistance = 35f;
            float minAngle = 50f;
            float maxAngle = 100f;

            Vector3 oldPosVector = instance.transform.position - player.transform.position;

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
            instance.StartCoroutine(Dashing(rawPosition));

            Glare();
        });
    }

    public static IEnumerator Dashing(Vector3 targetPosition) {

        eventQueue.Pause();

        physbody.velocity = Vector3.zero;

        Vector3 dashDir = (targetPosition - instance.transform.position).normalized;
        float dist;
        while ((dist = Vector3.Distance(targetPosition, instance.transform.position)) > 0f) {
            
            float dashDistance = self.movespeed.Value * 4 * Time.deltaTime;

            if (dist < dashDistance)
            {
                instance.transform.position = targetPosition;
                break;
            }

            instance.transform.position += dashDir * (dashDistance);
            yield return null;
        }

        self.movespeed.SetBase(100);
        eventQueue.Unpause();
    }

    public static AIEvent Strafe(bool clockwise = true, float degrees = 10f, int speed = 100, Vector3 center = default(Vector3))
    {
        return new AIEvent(0f, () =>
        {
            self.movespeed.SetBase(speed);

            Vector3 oldPosVector = instance.transform.position - center;
            Quaternion rot = Quaternion.AngleAxis(degrees, clockwise ? Vector3.up : Vector3.down);

            instance.StartCoroutine(Dashing(rot * oldPosVector));
        });
    }

    public static AIEvent CameraMove(bool isFollow = false, Vector3? targetPosition = null)
    {
        return new AIEvent(0f, () =>
        {
            CameraController.GetInstance().IsFollowing = isFollow;

            if (targetPosition.HasValue)
            {
                CameraController.GetInstance().Goto(targetPosition.Value, 1);
            }
        });
    }

    public static AIEvent PlayerLock(bool enableLock = true)
    {
        return new AIEvent(0f, () =>
        {
            if (enableLock)
            {
                playerLockPosition = player.transform.position;
            }
            isPlayerLocked = enableLock;
        });
    }

}
