using System;
using System.Collections;
using UnityEngine;

namespace Combat.AbilityBehaviors
{
    public class Example : AbilityBehavior
    {
        Func<bool> trigger;
        public override bool Start(GameObject blackboard, Ability ability)
        {
            Debug.Log($"Starting {nameof(Example)} behavior for {ability.ArcheType?.Name} on {blackboard.name}");
            trigger = ability.Trigger;
            return true;
        }

        public override IEnumerator Update()
        {
            float duration = 2f;
            while (trigger() && (duration -= Time.deltaTime) > 0f)
            {
                Debug.Log($"Running {nameof(Example)}; {duration}s remaining");
                yield return null;
            }

            if (duration <= 0f)
            {
                Debug.Log("Full CHARGE!");
            }
            else
            {
                Debug.Log($"Charged to {(2f - duration) * 50f}%");
            }
        }

        public override void Finish(Result result)
        {
            switch(result)
            {
            case Result.FINISHED:
                Debug.Log($"Finished {nameof(Example)}");
                break;
            case Result.INTERRUPTED:
                Debug.Log($"Interrupted {nameof(Example)}");
                break;
            case Result.STOPPED:
                Debug.Log($"Stopped {nameof(Example)}");
                break;
            }
        }
    }
}
