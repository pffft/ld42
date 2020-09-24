using UnityEngine;

namespace Combat
{
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

        public event OnDamageEvent.Delegate onDamageDealt;
        public event OnDamageEvent.Delegate onDamageTaken;
        public event OnDeathEvent.Delegate onDeath;

        public void Update()
        {
            ClampHealth();
        }

        public bool isAlive() => health > DEATH_THRESHOLD;

        public bool isDead() => health <= DEATH_THRESHOLD;

        public void ClampHealth() => Mathf.Clamp(health, DEATH_THRESHOLD, maximumHealth);

        public void DealDamageTo(Combatant victim, int damage)
        {
            var eventData = new OnDamageEvent { Attacker = this, Victim = victim };
            eventData.HealthBefore = health;
            health -= Mathf.Abs(damage);
            eventData.HealthAfter = health;

            victim.onDamageTaken?.Invoke(eventData);
            onDamageDealt?.Invoke(eventData);

            if (isDead())
            {
                onDeath?.Invoke(new OnDeathEvent { Attacker = this, Victim = victim });
            }
        }
    }
}
