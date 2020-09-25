using System;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "DefaultArchetype", menuName = "Combat/Ability Archetype")]
    public class AbilityArchetype : ScriptableObject
    {
        [SerializeField]
        [Tooltip("A unique identifier for this ability type")]
        private string abilityName = "Default";

        [SerializeField]
        [TextArea]
        [Tooltip("A short description of what this ability type does")]
        private string description = "A default ability archetype";

        [SerializeField]
        [Tooltip("The icon used to represent this ability in UI")]
        private Sprite icon = null;

        [SerializeField]
        [Tooltip("The delay (in seconds) from a cast to the next allowed cast")]
        private float baseMaxCooldown = 1f;

        [SerializeField]
        [Tooltip("Maximum number of casts that can be cached for this ability")]
        private int baseMaxCharges = 1;

        [SerializeField]
        [Tooltip("Number of charges stored after every cooldown completion")]
        private int baseChargeFillRate = 1;

        [SerializeField]
        [Tooltip("Same as $baseMaxCharges charge fill rate")]
        private bool fillAllChargesOnCooldown = false;

        [SerializeField]
        [Tooltip("Can casts of this ability can be interrupted?")]
        private bool interruptable = true;

        [SerializeField]
        [Tooltip("The fully-qualified class name of the AbilityBehavior associated with this type")]
        private string behaviourName = "Example";

        public string Name => abilityName;

        public string Description => description;

        public Sprite Icon => icon;

        public float BaseMaxCooldown => baseMaxCooldown;

        public int BaseMaxCharges => baseMaxCharges;

        public int BaseChargeFillRate => baseChargeFillRate;

        public bool FillAllChargesOnCooldown => fillAllChargesOnCooldown;

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
