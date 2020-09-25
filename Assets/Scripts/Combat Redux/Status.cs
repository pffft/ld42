using JetBrains.Annotations;

namespace Combat
{
    public struct Status
    {
        public StatusArchetype Archetype { get; set; }

        public float RemainingDuration { get; set; }

        public Status([NotNull] StatusArchetype archetype)
        {
            Archetype = archetype;
            RemainingDuration = Archetype.BaseMaxDuration;
        }

        public override bool Equals(object obj)
        {
            if (GetType().Equals(obj?.GetType()))
            {
                Status other = (Status) obj;
                return other.Archetype?.Name == Archetype?.Name;
            }
            return false;
        }

        public override int GetHashCode() => Archetype?.Name.GetHashCode() ?? 0;
    }
}
