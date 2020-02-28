using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AI;
using Projectiles;
using Moves.Basic;

namespace Moves.Test
{
    public class SpinReverse : Move
    {
        public SpinReverse(int count = 50)
        {
            Sequence =
                ForConcurrent(count, i =>
                    new Shoot1(
                        new ProjectileCurving(187, false)
                        {
                            Start = (Constants.Positions.PLAYER_POSITION + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.forward)),
                            Target = (Constants.Positions.PLAYER_POSITION + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.forward) + Quaternion.AngleAxis(i * (360f / count), Vector3.up) * (10 * Vector3.right)),
                            Speed = (Constants.Speed.FAST),
                            MaxTime = (0.75f),
                            OnDestroyTimeout = self =>
                                new Shoot1(
                                    new ProjectileReverse()
                                    {
                                        Start = (self.transform.position),
                                        Target = (Constants.Positions.PLAYER_POSITION),
                                        Speed = (Constants.Speed.SNIPE),
                                        MaxTime = (0.75f)
                                    }
                               )
                        }
                   )
             );
        }
    }
}
