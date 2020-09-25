
namespace Combat
{
    public abstract class StatusComponent
    {
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

        public virtual void OnStacksChanged(int stacksBefore, int stacksAfter)
        {

        }

        public virtual void OnUpdate()
        {

        }

        public virtual void OnDamageTaken(OnDamageEvent eventData)
        {

        }

        public virtual void OnDamageDealt(OnDamageEvent eventData)
        {

        }  

        public virtual void OnDeath(OnDeathEvent eventData)
        {

        }

        public override bool Equals(object obj) => obj.GetType().Equals(GetType());

        public override int GetHashCode() => GetType().GetHashCode();
    }
}
