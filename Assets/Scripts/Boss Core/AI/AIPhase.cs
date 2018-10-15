using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Moves;

// TODO: make a list of weight/AISequence pairs that can be constructed
// make a list of scripted events that happen once in X times
// randomly choose the next sequence to do, or check if player interrupts
namespace AI
{
    public partial class AIPhase
    {
        // What's the amount of health we have this phase?
        public int maxHealth = 100;

        // How big is the arena this phase?
        public float maxArenaRadius = 50f;

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

        public AIPhase SetMaxHealth(int health) {
            this.maxHealth = health;
            return this;
        }

        public AIPhase SetMaxArenaRadius(float width) {
            this.maxArenaRadius = width;
            return this;
        }

        public AIPhase AddMove(int weight, Move move) {
            phaseSequences.Add(new AIPhaseComponent(weight, move));
            totalWeight += weight;
            return this;
        }

        public AIPhase AddScriptedMove(int when, Move move) {
            scriptedSequences.Add(new AIPhaseScriptedComponent(when, move));
            return this;
        }

        public AIPhase AddRepeatingScriptedMove(int everyX, Move move) {
            repeatingScriptedSequences.Add(new AIPhaseScriptedComponent(everyX, move));
            return this;
        }

        /*
         * TODO: make this use the difficulty to consider weights, rather than custom weights.
         * Then make the difficulty an AnimationCurve.
         */
        public Move GetNext()
        {
            count++;

            // First we check for scripted events.
            // We'll always take the first scripted event that matches, so if there
            // are two, the tie is broken by taking the first one added.
            foreach (AIPhaseScriptedComponent component in scriptedSequences) {
                // Only run if the event is scheduled to run.
                if (count == component.everyX) {
                    return component.move;
                }
            }

            // Then we do repeating scripted events.
            foreach (AIPhaseScriptedComponent component in repeatingScriptedSequences)
            {
                if (count == 0 && component.everyX == 0)
                {
                    return component.move;
                }

                if (count % component.everyX == 0)
                {
                    return component.move;
                }
            }

            // If we haven't got a scripted event, we take a random non-scripted event.
            int targetWeight = Random.Range(0, totalWeight);
            int currentWeight = 0;
            foreach (AIPhaseComponent component in phaseSequences) {
                if (targetWeight < currentWeight + component.weight) {
                    return component.move;
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
            public Move move;

            public AIPhaseComponent(int weight, Move move) {
                this.weight = weight;
                this.move = move;
            }
        }

        private class AIPhaseScriptedComponent {
            public int everyX;
            public Move move;

            public AIPhaseScriptedComponent(int everyX, Move move) {
                this.everyX = everyX;
                this.move = move;
            }
        }
    }
}