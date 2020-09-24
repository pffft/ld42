
namespace Combat
{
    public abstract class CombatEvent<T>
    {
        public delegate void Delegate(T data);
    }

    public class OnDamageEvent : CombatEvent<OnDamageEvent>
    {
        public Combatant Attacker { get; set; }
        public Combatant Victim { get; set; }

        public int HealthBefore { get; set; }
        public int HealthAfter { get; set; }
        public int Damage => HealthBefore - HealthAfter;
    }

    public class OnDeathEvent : CombatEvent<OnDeathEvent>
    {
        public Combatant Attacker { get; set; }
        public Combatant Victim { get; set; }
    }

    public class OnAbilityCast : CombatEvent<OnAbilityCast>
    {
        public Ability Ability { get; set; }
    }

    public class OnAbilityFinish : CombatEvent<OnAbilityFinish>
    {
        public Ability Ability { get; set; }
        public AbilityBehavior.Result Result { get; set; }
    }
}
