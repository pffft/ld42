using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BossCore;

namespace AI {
    public class AOEAISequence : AISequence
    {
        private AOE aoe;

        public AOEAISequence(AOE.AOEStructure aoeStructure) : this(-1, aoeStructure) { }

        public AOEAISequence(float difficulty, AOE.AOEStructure aoeStructure) {
            AISequence seq = new AISequence(() => {
                aoe = aoeStructure.Create();
            });
            this.difficulty = difficulty;
            this.events = seq.events;
        }

        public new AOEAISequence Wait(float seconds) {
            return (AOEAISequence)base.Wait(seconds);
        }

        public AOEAISequence SetSpeed(Speed speed) {
            return (AOEAISequence)Then(new AISequence(() =>
            {
                aoe.data.Speed(speed);
            }));
        }
    }
}
