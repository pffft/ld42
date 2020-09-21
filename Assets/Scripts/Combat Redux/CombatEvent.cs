
namespace Combat
{
    public abstract class CombatEvent<T>
    {
        public delegate void Delegate(T data);
    }

    public class OnDamageTakenEvent : CombatEvent<OnDamageTakenEvent>
    {
        public float 
    }
}
