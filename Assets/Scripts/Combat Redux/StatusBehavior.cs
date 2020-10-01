using UnityEngine;

namespace Combat
{
    public abstract class StatusBehavior
    {
        public StatusArchetype Archetype { get; set; }

        private int stackCount;
        public int StackCount
        {
            get => stackCount;
            set
            {
                if (value != stackCount)
                {
                    OnStacksChanged(stackCount, value);
                    stackCount = value;
                }
            }
        }

        protected GameObject Blackboard { get; private set; }

        protected Combatant Source { get; private set; }

        public float RemainingDuration { get; set; }

        public void Initialize(GameObject blackboard, Combatant source, int initialStacks)
        {
            Blackboard = blackboard;
            Source = source;
            stackCount = initialStacks;
        }

        public virtual void OnApply() { }

        public virtual void OnRemove() { }

        public virtual void OnStacksChanged(int stacksBefore, int stacksAfter) { }

        public virtual void OnUpdate() { }

        public virtual void OnDamageTaken(OnDamageEvent eventData) { }

        public virtual void OnDamageDealt(OnDamageEvent eventData) { }

        public virtual void OnDeath(OnDeathEvent eventData) { }

        public virtual void OnAbilityCast(OnAbilityCast eventData) { }

        public virtual void OnAbilityFinish(OnAbilityFinish eventData) { }

        public override bool Equals(object obj) => obj.GetType().Equals(GetType());

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
