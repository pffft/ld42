using System;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "DefaultArchetype", menuName = "AbilityArchetype")]
    public class AbilityArchetype : ScriptableObject
    {
        [SerializeField]
        private string abilityName = "Default";

        [SerializeField]
        [TextArea]
        private string description = "A default ability archetype";

        [SerializeField]
        private Sprite icon = null;

        [SerializeField]
        private float baseMaxCooldown = 1f;

        [SerializeField]
        private int baseMaxCharges = 0;

        [SerializeField]
        private bool interruptable = true;

        [SerializeField]
        private string behaviourName = "Example";

        public string Name => name;

        public string Description => description;

        public Sprite Icon => icon;

        public float BaseMaxCooldown => baseMaxCooldown;

        public float BaseMaxCharges => baseMaxCharges;

        public bool IsInterruptable => interruptable;

        public AbilityBehavior GetBehaviorInstance()
        {
            Type t = Type.GetType(behaviourName, throwOnError: true, ignoreCase: false);
            if (t.IsSubclassOf(typeof(AbilityBehavior)))
            {
                return Activator.CreateInstance(t, nonPublic: true) as AbilityBehavior;
            }
            else
            {
#if DEBUG
                Debug.Log($"{t.Name} is not an {nameof(AbilityBehavior)}");
#endif
                return null;
            }
        }
    }
}
