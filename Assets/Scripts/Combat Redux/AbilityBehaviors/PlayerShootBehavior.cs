using System;
using System.Collections;
using System.Collections.Generic;
using Projectiles;
using UnityEngine;

namespace Combat.AbilityBehaviors 
{

    public class PlayerShootBehavior : AbilityBehavior
    {
        private Func<bool> ShootTriggerPressed;

        private static readonly ProjectileData SHOTGUN_PELLET = new ProjectileData()
        {
            Size = Size.TINY,
            Speed = Constants.Speed.LIGHTNING,
            Start = Constants.Positions.PLAYER_POSITION,
            Target = Constants.Positions.BOSS_POSITION,
            Faction = CombatCore.Entity.Faction.player
        };

        // Called on trigger, to check any other conditions before activating.
        public override bool Start(GameObject blackboard, Ability ability)
        {
            ShootTriggerPressed = ability.Trigger;
            return true;
        }

        // Called each update while the ability is active.
        public override IEnumerator Update()
        {
            float duration = 2f;

            // While the trigger is held, decrement duration and yield return
            while (ShootTriggerPressed())
            {
                duration -= Time.deltaTime;
                yield return null;
            }

            // Trigger was released. Check duration to see what we should shoot.
            if (duration <= 0f) 
            {
                // Full power
                Debug.Log("Fully charged");
            }
            else
            {
                // Partial power. Convert to a float in [0, 1] and do something.
                float percent = (2f - duration) / 2f;
                Debug.Log("Partially charged, percent: " + percent);

                SHOTGUN_PELLET.Clone().Create();

            }

        }

        // Called when the ability ends.
        public override void Finish(Result result)
        {
            Debug.Log("ah");
        }
    }
}
