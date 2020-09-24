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

        public event OnAbilityCast.Delegate onCast;
        public event OnAbilityFinish.Delegate onFinish;

        public bool Cast(GameObject blackboard)
        {
            if (behavior == null)
            {
                behavior = ArcheType?.GetBehaviorInstance();
                behavior.Initialize(blackboard, this);
            }

            if (!IsRunning && IsReady && (behavior?.Start() ?? false))
            {
                currentExecution = behavior.Update();
                onCast?.Invoke(new OnAbilityCast { Ability = this });
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
                    ResetCooldown();
                    behavior.Finish(AbilityBehavior.Result.FINISHED);
                    currentExecution = null;

                    OnFinish(AbilityBehavior.Result.FINISHED);
                }
            }
            else if (Cooldown > 0f && Charges < (ArcheType?.BaseMaxCharges ?? 1))
            {
                Cooldown -= Time.deltaTime;
                if (Cooldown <= 0f)
                {
                    if (ArcheType?.FillAllChargesOnCooldown ?? false)
                    {
                        Charges = ArcheType.BaseMaxCharges;
                    }
                    else
                    {
                        Charges += ArcheType?.BaseChargeFillRate ?? 1;
                    }
                    Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
                }
            }
            else
            {
                Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                ResetCooldown();
                behavior.Finish(AbilityBehavior.Result.STOPPED);
                currentExecution = null;

                OnFinish(AbilityBehavior.Result.STOPPED);
            }
        }

        public void Interrupt()
        {
            if (ArcheType?.IsInterruptable ?? true && IsRunning)
            {
                ResetCooldown();
                behavior.Finish(AbilityBehavior.Result.INTERRUPTED);
                currentExecution = null;

                OnFinish(AbilityBehavior.Result.INTERRUPTED);
            }
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

        private void ResetCooldown()
        {
            if (Charges > 0)
            {
                Charges--;
            }
            else
            {
                Cooldown = ArcheType?.BaseMaxCooldown ?? 0f;
            }
        }

        private void OnFinish(AbilityBehavior.Result result)
        {
            onFinish?.Invoke(new OnAbilityFinish { Ability = this, Result = result });
        }
    }
}
