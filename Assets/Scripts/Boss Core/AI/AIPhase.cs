using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: make a list of weight/AISequence pairs that can be constructed
// make a list of scripted events that happen once in X times
// randomly choose the next sequence to do, or check if player interrupts
namespace AI
{
    public partial class AIPhase
    {
        /// <summary>
        /// What's the amount of health we have this phase?
        /// </summary>
        /// <value>The max health.</value>
        public int MaxHealth { get; set; } = 100;

        /// <summary>
        /// How big is the arena this phase?
        /// </summary>
        /// <value>The max arena radius.</value>
        public float MaxArenaRadius { get; set; } = 50f;

        /// <summary>
        /// If this value is true, we adjust the probability weights of the moves to be
        /// closer to the global difficulty parameter. This parameter is based on how
        /// aggressive the player is + how much damage they've taken recently (indicating
        /// their proficiency at this difficulty). 
        /// </summary>
        /// <value><c>true</c> if dynamic difficulty; otherwise, <c>false</c>.</value>
        public bool DynamicDifficulty { get; set; } = false;

        private List<AIPhaseComponent> phaseSequences;
        private List<AIPhaseScriptedComponent> scriptedSequences;
        private List<AIPhaseScriptedComponent> repeatingScriptedSequences;

        private int totalWeight;
        private int count = -1;

        public AIPhase()
        {
            phaseSequences = new List<AIPhaseComponent>();
            scriptedSequences = new List<AIPhaseScriptedComponent>();
            repeatingScriptedSequences = new List<AIPhaseScriptedComponent>();
        }

        public AIPhase AddSequence(int weight, AISequence sequence) {
            phaseSequences.Add(new AIPhaseComponent(weight, sequence));
            totalWeight += weight;
            return this;
        }

        public AIPhase AddScriptedSequence(int when, AISequence sequence) {
            scriptedSequences.Add(new AIPhaseScriptedComponent(when, sequence));
            return this;
        }

        public AIPhase AddRepeatingScriptedSequence(int everyX, AISequence sequence) {
            repeatingScriptedSequences.Add(new AIPhaseScriptedComponent(everyX, sequence));
            return this;
        }

        /*
         * TODO: make this use the difficulty to consider weights, rather than custom weights.
         * Then make the difficulty an AnimationCurve.
         */
        public AISequence GetNext()
        {
            count++;

            // First we check for scripted events.
            // We'll always take the first scripted event that matches, so if there
            // are two, the tie is broken by taking the first one added.
            foreach (AIPhaseScriptedComponent component in scriptedSequences) {
                // Only run if the event is scheduled to run.
                if (count == component.everyX) {
                    Debug.Log("Returned scripted ai sequence");
                    return component.sequence;
                }
            }

            // Then we do repeating scripted events.
            foreach (AIPhaseScriptedComponent component in repeatingScriptedSequences)
            {
                if (count == 0 && component.everyX == 0)
                {
                    Debug.Log("Returned scheduled scripted ai sequence");
                    return component.sequence;
                }

                if (count % component.everyX == 0)
                {
                    Debug.Log("Returned scheduled scripted ai sequence");
                    return component.sequence;
                }
            }

            // If we haven't got a scripted event, we take a random non-scripted event.
            int targetWeight = Random.Range(0, totalWeight);
            int currentWeight = 0;
            foreach (AIPhaseComponent component in phaseSequences) {
                if (targetWeight < currentWeight + component.weight)
                {
                    Debug.Log("Returned random ai sequence");
                    return component.sequence;
                }
                currentWeight += component.weight;
            }

            if (phaseSequences.Count == 0)
            {
                Debug.LogError("Failed to get next sequence for phase. Dictionary is empty. Add some sequences!");
                return null;
            }

            // This shouldn't ever trigger.
            Debug.LogError("Failed to get next sequence for phase.");
            return null;
        }

        private class AIPhaseComponent {
            public int weight;
            public AISequence sequence;

            public AIPhaseComponent(int weight, AISequence sequence) {
                this.weight = weight;
                this.sequence = sequence;
            }
        }

        private class AIPhaseScriptedComponent {
            public int everyX;
            public AISequence sequence;

            public AIPhaseScriptedComponent(int everyX, AISequence sequence) {
                this.everyX = everyX;
                this.sequence = sequence;
            }
        }
    }
}