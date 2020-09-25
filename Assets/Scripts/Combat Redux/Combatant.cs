using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [AddComponentMenu("Combat/Combatant")]
    public class Combatant : MonoBehaviour
    {
        public const int DEATH_THRESHOLD = 0;

        [SerializeField]
        private int maximumHealth;

        public int MaximumHealth
        {
            get => maximumHealth;
            set => maximumHealth = value;
        }

        [SerializeField]
        private int health;

        public int Health
        {
            get => health;
            set => health = value;
        }

        //TODO what ds?
        private HashSet<Status> activeStatuses = new HashSet<Status>();
        private HashSet<StatusComponent> appliedStatusComponents = new HashSet<StatusComponent>();

        public event OnDamageEvent.Delegate onDamageDealt;
        public event OnDamageEvent.Delegate onDamageTaken;
        public event OnDeathEvent.Delegate onDeath;

        public void Update()
        {
            ClampHealth();

            //TODO update status durations + decay stacks
        }

        public bool isAlive() => health > DEATH_THRESHOLD;

        public bool isDead() => health <= DEATH_THRESHOLD;

        public void ClampHealth() => Mathf.Clamp(health, DEATH_THRESHOLD, maximumHealth);

        public void Attack(Combatant victim, int damage)
        {
            var eventData = new OnDamageEvent { Attacker = this, Victim = victim };
            eventData.HealthBefore = victim.health;
            victim.health -= Mathf.Abs(damage);
            eventData.HealthAfter = victim.health;

            victim.OnDamageTaken(eventData);
            OnDamageDealt(eventData);

            if (victim.isDead())
            {
                victim.OnDeath(new OnDeathEvent { Attacker = this, Victim = victim });
            }
        }

        public bool ApplyStatus(StatusArchetype archetype)
        {
            //TODO add new statuses and increment stacks for existing ones

            Status newStatus = new Status(archetype);
            if (activeStatuses.Contains(newStatus))
            {
                
            }

            return false;
        }

        private void OnDamageTaken(OnDamageEvent eventData)
        {
            //TODO call status hooks

            onDamageTaken?.Invoke(eventData);
        }

        private void OnDamageDealt(OnDamageEvent eventData)
        {
            //TODO call status hooks

            onDamageDealt?.Invoke(eventData);
        }

        private void OnDeath(OnDeathEvent eventData)
        {
            //TODO call status hooks

            onDeath?.Invoke(eventData);
        }
    }
}
