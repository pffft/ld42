using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CombatCore;
using AI;

namespace Moves.Basic
{
    public class Invincible : InternalMove
    {

        // A reference to the BossController's Entity.
        private static Entity self;

        public Invincible(bool to) : base
        (
            () => 
            {
                if (self == null) {
                    self = GameManager.Boss.GetComponent<Entity>();
                }
                self.SetInvincible(to);
            }
        ) 
        {
            Description = "Makes the boss " + (to ? "" : "not") + " invincible.";
        }
    }
}
