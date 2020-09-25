using System;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(fileName = "DefaultArchetype", menuName = "Combat/Status Archetype")]
    public class StatusArchetype : ScriptableObject
    {
        [SerializeField]
        private string statusName = "Default";

        [SerializeField]
        private string description = "A default status archetype";

        [SerializeField]
        private Sprite icon = null;

        [SerializeField]
        private float baseMaxDuration = 1f;

        [SerializeField]
        private int baseMaxStacks = 1;

        [SerializeField]
        private int baseStackFillRate = 1;

        [SerializeField]
        private bool fillAllStacks = false;

        [SerializeField]
        private int baseStackDecayRate = 1;

        [SerializeField]
        private bool decayAllStacks = false;

        [SerializeField]
        private string[] componentNames = new string[0];

        public string Name => statusName;

        public string Description => description;

        public Sprite Icon => icon;

        public float BaseMaxDuration => baseMaxDuration;

        public int BaseMaxStacks => baseMaxStacks;

        public int BaseStackFillRate => baseStackFillRate;

        public bool FillAllStacks => fillAllStacks;

        public int BaseStackDecayRate => baseStackDecayRate;

        public bool DecayAllStacks => decayAllStacks;

        public StatusComponent[] GetComponents()
        {
            StatusComponent[] components = new StatusComponent[componentNames.Length];
            for (int i = 0; i < components.Length; i++)
            {
                Type t = Type.GetType(name, throwOnError: true, ignoreCase: false);

                if (t.IsSubclassOf(typeof(StatusComponent)))
                {
                    components[i] = Activator.CreateInstance(t) as StatusComponent;
                }
            }

            return components;
        }
    }
}
