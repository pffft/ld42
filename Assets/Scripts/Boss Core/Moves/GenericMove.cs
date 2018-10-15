using System.Collections;
using System.Collections.Generic;
using AI;
using UnityEngine;

namespace Moves
{
    public class GenericMove : Move
    {
        private readonly AISequence sequence;

        public GenericMove(AISequence sequence) {
            this.sequence = sequence;
        }

        public override float GetDifficulty()
        {
            return -1f;
        }

        public override AISequence GetSequence()
        {
            return sequence;
        }
    }
}
