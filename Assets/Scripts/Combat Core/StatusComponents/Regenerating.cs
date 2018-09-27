using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatCore.StatusComponents
{
    public class Regenerating : StatusComponent
    {
        public override void OnUpdate(Entity subject, float time)
        {
            // regenerate here?
        }

        public override void OnShieldsDown(Entity subject)
        {
            // make the shield go poof
        }

        public override void OnRevert(Entity subject)
        {

            subject.movespeed.Unlock();
        }
    }
}