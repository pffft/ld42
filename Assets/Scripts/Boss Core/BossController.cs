using System.Collections;
using System.Collections.Generic;
using CombatCore;
using UnityEngine;

public class BossController : MonoBehaviour
{

    private GameObject player;

    private Rigidbody physbody;
    protected CombatCore.Entity self;

    private EventQueue eventQueue;

    bool flag = true;
    int teleportCounter = 0;

    Ability shoot1, shoot3, shootWave, teleport;

    private void Awake() {
        player = GameObject.Find("Player");

        physbody = GetComponent<Rigidbody>();
        self = GetComponent<CombatCore.Entity>();

        eventQueue = new EventQueue(this);
	}

	// Use this for initialization
    void Start () {
        shoot1 = new Ability("shoot1", "", null, 0.1f, 0, Shoot1);
        shoot3 = new Ability("shoot3", "", null, 0.1f, 0, Shoot3);
        shootWave = new Ability("shootWave", "", null, 1.2f, 0, ShootWave);

        teleport = new Ability("teleport", "", null, 1.2f, 0, Teleport);

        self.AddAbility(shoot1);
        self.AddAbility(shoot3);
        self.AddAbility(shootWave);
        self.AddAbility(teleport);

        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shootWave, 45, 360f, -10f);
        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shoot3);
        eventQueue.Add(0f, teleport);
        eventQueue.Add(1.2f, shoot1);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerPos = player.transform.position;

        //if (self.GetAbility(0).Use(self, playerPos)) { }

        //if (Input.GetKey(KeyCode.P)) {
        //if (self.GetAbility(0).Use(self, playerPos, new object[]{30, 120f})) {}

        /*
        Vector3 randomArenaPos = new Vector3(Random.Range(-30, 30), 0, Random.Range(-30, 30));
        self.GetAbility(3).Use(self, randomArenaPos);

        if (flag) {
            self.GetAbility(2).Use(self, playerPos, 45, 360f, -10f);
        } else {
            self.GetAbility(2).Use(self, playerPos, 45, 360f, 10f);
        }
        flag = !flag;
        */

        eventQueue.Update();




        //}
	}

    public void Glare() {
        Quaternion lookRotation = Quaternion.LookRotation(transform.position - player.transform.position);
        transform.rotation = lookRotation;
    }

    private bool Shoot1(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();
        ProjectileManager.spawn1(transform.position, player.transform.position);
        return true;
    }

    public bool Shoot3(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();
        ProjectileManager.spawn1(transform.position, player.transform.position);
        ProjectileManager.spawn1(transform.position, player.transform.position, -30);
        ProjectileManager.spawn1(transform.position, player.transform.position, 30);
        return true;
    }

    public bool ShootWave(Entity subject, Vector3 targetPosition, params object[] args) {
        Glare();

        int amount = (int)args[0];
        if (amount == 0) amount = 1;

        float arcWidth = (float)args[1];

        float halfArcWidth = -arcWidth / 2f;

        float offset = (float)args[2];

        Debug.Log("Shooting wave with " + amount + " projectiles and spread of " + arcWidth);

        for (int i = 0; i < amount; i++) {
            ProjectileManager.spawn1(
                transform.position,
                player.transform.position,
                halfArcWidth + offset + (i * (arcWidth / amount)),
                ProjectileManager.Speed.SLOW
            );
        }

        return true;
    }

    public bool Teleport(Entity subject, Vector3 targetPosition, params object[] args) {
        Vector3 randomArenaPos = new Vector3(Random.Range(-30, 30), 1.31f, Random.Range(-30, 30));
        transform.position = randomArenaPos;
        Glare();
        return true;
    }

    class EventQueue {

        private BossController outerReference;
        private Queue<AIEvent> events;
        private AIEvent lastEvent;
        private float lastTime;

        public EventQueue(BossController reference) {
            outerReference = reference;

            events = new Queue<AIEvent>();
            lastEvent = null;
            lastTime = 0;
        }

        public void Add(float duration, Ability ability, params object[] pars) {
            float start = 0;
            if (Time.time > lastTime) {
                start = Time.time;
            } else {
                start = lastTime;
                lastTime += duration;
            }

            events.Enqueue(new AIEvent(start, duration, ability, pars));
        }

        public void Update() {

            // if the player is too aggressive, you can ignore the q here

            if (events.Count == 0) return;
            AIEvent iEvent = events.Peek();
            Debug.Log("Top event is " + iEvent.ability.name);

            if (Time.time >= iEvent.startTime) {
                // If the event is new, we fire it
                if (lastEvent != iEvent) {
                    iEvent.ability.Use(outerReference.self, Vector3.zero, iEvent.parameters);
                    lastEvent = iEvent;
                }

                // If the event is stale, remove it from the q
                if (Time.time >= iEvent.startTime + iEvent.duration) {
                    events.Dequeue();
                }
            }
        }

        private class AIEvent {
            public float startTime;
            public float duration;
            public Ability ability;
            public object[] parameters;

            public AIEvent(float start, float duration, Ability ability, params object[] parameters) {
                this.startTime = start;
                this.duration = duration;
                this.ability = ability;
                this.parameters = parameters;
            }
        }
    }
}
