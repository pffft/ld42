using System;
using System.Collections;
using UnityEngine;

namespace Combat
{
    public sealed class Ability
    {
        private AbilityBehavior behavior = null;
        private IEnumerator currentExecution = null;

        public AbilityArchetype ArcheType { get; set; }

        public float Cooldown { get; set; }

        public int Charges { get; set; }

        public bool IsAvailable { get; set; }

        public Func<bool> Trigger { get; set; }

        public bool IsReady => Cooldown <= 0f || Charges > 0;

        public bool IsRunning => currentExecution != null;

        public bool Cast(GameObject blackboard)
        {
            if (behavior == null)
            {
                behavior = ArcheType?.GetBehaviorInstance();
            }

            if (!IsRunning && IsReady && (behavior?.Start(blackboard, this) ?? false))
            {
                currentExecution = behavior.Update();
                if (Charges > 0) 
                {
                    Charges--;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update()
        {
            if (IsRunning)
            {
                bool finished = !currentExecution.MoveNext();
                if (finished)
                {
                    Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
                    behavior.Finish(AbilityBehavior.Result.FINISHED);
                    currentExecution = null;
                }
            }
            else if (Cooldown > 0f)
            {
                Cooldown -= Time.deltaTime;
                if (Cooldown <= 0f)
                {
                    if (Charges < (ArcheType?.BaseMaxCharges ?? 0))
                    {
                        Charges++;
                        Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
                    }
                    else
                    {
                        Cooldown = 0;
                    }
                }
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
                behavior.Finish(AbilityBehavior.Result.STOPPED);
            }
            
            currentExecution = null;
        }

        public void Interrupt()
        {
            if (IsRunning)
            {
                Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
                behavior.Finish(AbilityBehavior.Result.INTERRUPTED);
            }

            currentExecution = null;
        }

        public void Reset()
        {
            Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
            Charges = 0;
            IsAvailable = true;
            behavior = null;
            currentExecution = null;
        }

        public void SetArchetypeAndReset(AbilityArchetype archetype)
        {
            ArcheType = archetype;
            Reset();
        }
    }
}
