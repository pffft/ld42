using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using static BossController;

using Moves.Basic;

namespace Moves.Tutorial2
{
    public class Force_Block : AISequence
    {
        public Force_Block() : base
        (
            new Teleport().Wait(0.25f),
            new PlayerLock(true),
            new ShootArc(100, -90, 90, new Projectile { Size = Size.TINY }).Wait(0.1f).Times(10),
            new PlayerLock(false),
            new Pause(4f)
        )
        {
            Description = "Fires 10 sets of tiny projectiles in a 180 degree arc, too dense to dash through.";
            Difficulty = 5f;
        }
    }
}
