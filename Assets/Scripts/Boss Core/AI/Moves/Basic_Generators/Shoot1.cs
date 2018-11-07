﻿using AI;
using Projectiles;
using static BossController;

namespace Moves.Basic
{
    public class Shoot1 : InternalMove
    {
        public Shoot1(Projectile skeleton = null) : base
        (
            () =>
            {
                GameManager.Boss.Glare();

                Projectile newStruc = skeleton != null ? skeleton.Clone() : new Projectile();
                newStruc.Create();
            }
        )
        {
            Description = "Shot one " + (skeleton == null ? "default projectile at the player." : skeleton + ".");
        }
    }
}
