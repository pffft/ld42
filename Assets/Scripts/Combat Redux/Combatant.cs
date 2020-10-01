using System;
using System.Collections.Generic;
using UnityEngine;

namespace Combat
{
    [AddComponentMenu("Combat/Combatant")]
    public class Combatant : MonoBehaviour
    {
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

        private Dictionary<(string, Combatant), StatusBehavior> appliedStatuses = new Dictionary<(string, Combatant), StatusBehavior>();

        public event OnDamageEvent.Delegate onDamageDealt;
        public event OnDamageEvent.Delegate onDamageTaken;
        public event OnDeathEvent.Delegate onDeath;

        public void Update()
        {
            ClampHealth();

            Queue<(string, Combatant)> expiredStatuses = new Queue<(string, Combatant)>();

            foreach (KeyValuePair<(string, Combatant), StatusBehavior> entry in appliedStatuses)
            {
                var key = entry.Key;
                var behavior = entry.Value;

                behavior.OnUpdate();
                behavior.RemainingDuration -= Time.deltaTime;
                if (behavior.RemainingDuration <= 0f)
                {
                    if (behavior.Archetype?.DecayAllStacks ?? false)
                    {
                        behavior.StackCount = 0;
                    }
                    else
                    {
                        behavior.StackCount -= Mathf.Min(behavior.Archetype?.BaseStackDecayRate ?? 1, behavior.StackCount);
                    }

                    if (behavior.StackCount <= 0)
                    {
                        expiredStatuses.Enqueue(key);
                    }
                }
            }

            foreach ((string, Combatant) entry in expiredStatuses)
            {
                appliedStatuses.Remove(entry);
            }
            expiredStatuses.Clear();
        }

        public bool isAlive() => health > 0;

        public bool isDead() => health <= 0;

        public void ClampHealth() => Mathf.Clamp(health, 0, maximumHealth);

        public void Attack(Combatant victim, int damage)
        {
            var eventData = new OnDamageEvent { Attacker = this, Victim = victim };
            eventData.HealthBefore = victim.health;
            if (damage > victim.health)
            {
                victim.health = 0;
            }
            else
            {
                victim.health -= Mathf.Abs(damage);
            }
            eventData.HealthAfter = victim.health;

            victim.OnDamageTaken(eventData);
            OnDamageDealt(eventData);

            if (victim.isDead())
            {
                victim.OnDeath(new OnDeathEvent { Attacker = this, Victim = victim });
            }
        }

        public bool ApplyStatusTo(Combatant victim, StatusArchetype archetype, int stacks)
        {
            var key = (archetype.Name, this);
            if (victim.appliedStatuses.TryGetValue(key, out StatusBehavior existing))
            {
                var baseMaxStacks = archetype?.BaseMaxStacks ?? 1;
                if (existing.StackCount < baseMaxStacks)
                {
                    existing.StackCount += Mathf.Min(stacks, baseMaxStacks - stacks);
                    return true;
                }
                return false;
            }

            var newStatus = archetype.GetInstance();
            newStatus.Initialize(gameObject, source: this, initialStacks: stacks);

            newStatus.OnApply();

            victim.appliedStatuses.Add(key, newStatus);

            return true;
        }

        private void OnDamageTaken(OnDamageEvent eventData)
        {
            foreach (StatusBehavior behavior in appliedStatuses.Values)
            {
                behavior.OnDamageTaken(eventData);
            }

            onDamageTaken?.Invoke(eventData);
        }

        private void OnDamageDealt(OnDamageEvent eventData)
        {
            foreach (StatusBehavior behavior in appliedStatuses.Values)
            {
                behavior.OnDamageDealt(eventData);
            }

            onDamageDealt?.Invoke(eventData);
        }

        private void OnDeath(OnDeathEvent eventData)
        {
            foreach (StatusBehavior behavior in appliedStatuses.Values)
            {
                behavior.OnDeath(eventData);
            }

            onDeath?.Invoke(eventData);
        }

        public void OnAbilityCast(OnAbilityCast eventData)
        {
            foreach (StatusBehavior behavior in appliedStatuses.Values)
            {
                behavior.OnAbilityCast(eventData);
            }
        }

        public void OnAbilityFinish(OnAbilityFinish eventData)
        {
            foreach (StatusBehavior behavior in appliedStatuses.Values)
            {
                behavior.OnAbilityFinish(eventData);
            }
        }

        public override bool Equals(object other)
        {
            if (other != null && other.GetType().IsSubclassOf(typeof(Combatant)))
            {
                var otherCombatant = other as Combatant;
                return otherCombatant.name.Equals(name);
            }
            return false;
        }

        public override int GetHashCode() => name.GetHashCode();
    }
}
