using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Combat
{
    public class AbilityCaster : MonoBehaviour
    {
        [Serializable]
        public class BoolRef
        {
            public bool Value { get; set; }

            public static implicit operator bool(BoolRef br)
            {
                return br != null ? br.Value : false;
            }

            public static implicit operator BoolRef(bool br)
            {
                return new BoolRef { Value = br };
            }
        }

        [Serializable]
        public class TriggerEvent : UnityEvent<BoolRef> { }

        [Serializable]
        public struct ConfigurationSlot
        {
            public AbilityArchetype archetype;
            public TriggerEvent trigger;
        }

        [SerializeField]
        private List<ConfigurationSlot> configuration = new List<ConfigurationSlot>();

        private List<Ability> abilities;

        public void Awake()
        {
            abilities = new List<Ability>();
            foreach (ConfigurationSlot slot in configuration)
            {
                abilities.Add(new Ability
                {
                    ArcheType = slot.archetype,
                    Trigger = () => {
                        BoolRef status = new BoolRef();
                        slot.trigger?.Invoke(status);
                        return status.Value;
                    }
                });
            }
        }

        public void Update()
        {
            foreach (Ability slot in abilities)
            {
                slot.Update();
                if (slot.Trigger?.Invoke() ?? false)
                {
                    slot.Cast(blackboard: gameObject);
                }
            }
        }
    }
}
