using System;
using System.Collections;
using UnityEngine;

namespace Combat.AbilityBehaviors
{
    public class Example : AbilityBehavior
    {
        public override bool Start()
        {
            Debug.Log($"Starting {nameof(Example)} behavior for {Ability.Archetype?.Name} on {Blackboard.name}");
            return true;
        }

        public override IEnumerator Update()
        {
            float duration = 2f;
            while (Ability.Trigger() && (duration -= Time.deltaTime) > 0f)
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
