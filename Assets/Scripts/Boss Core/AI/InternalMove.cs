using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    /// <summary>
    /// A subclass of AISequence that allows for more low-level control over the internal status of the game.
    /// 
    /// If you need to work with things like moving the camera, instantiating projectiles, or
    /// messing with the arena size, you need an InternalMove.
    /// </summary>
    public class InternalMove : AISequence
    {
        // Pass-through constructor from AISequence.
        internal InternalMove(params AISequence[] sequences) : base(sequences) {}

        /*
         * Creates a new singleton AISequence from the given Action.
         * This has no delay after its event.
         */
        internal InternalMove(AIEvent.Action a)
        {
            this.Events = new AIEvent[] { new AIEvent(0f, a) };
            this.Children = () => { return null; };
        }

        // Used uniquely by Pause() to generate an AIEvent with a nonzero duration.
        internal InternalMove(AIEvent a) 
        {
            this.Events = new AIEvent[] { a };
            this.Children = () => { return null; };
        }

        /*
         * Some delegates used to generate a sequence of events that need more dynamic
         * information. This allows for for loops over sequences and events.
         */
        internal delegate AISequence[] GenerateSequences();
        internal delegate AISequence GenerateSequence();

        /*
         * Keeps track of a function that can "explode" into a list of AISequences.
         * When this is added to the event queue, this function is called.
         */
        internal InternalMove(GenerateSequences genFunction)
        {
            this.Events = null;
            this.Children = () => genFunction();
            this.Description = ShouldTryExpandFunctions ? null : "Some sequences were generated from a function.";
        }

        /*
         * Keeps track of a function that can "explode" into a single AISequence.
         * When this is added to the event queue, this function is called.
         */
        internal InternalMove(GenerateSequence genFunction)
        {
            this.Events = null;
            this.Children = () => new AISequence[] { genFunction() };
            this.Description = ShouldTryExpandFunctions ? null : "A sequence was generated from a function.";
        }


        private static readonly string[] validNames = new string[] {
            "Invincible",
            "MoveCamera",
            "Pause",
            "PlayerLock",
            "Shoot1",
            "Shoot3",
            "ShootAOE",
            "ShootArc",
            "ShootHomingStrafe",
            "ShootLine",
            "ShootWall",
            "Strafe",
            "Teleport"
        };

        /// <summary>
        /// Returns true if the internal move is valid.
        /// 
        /// This checks against the whitelist of known internal moves to see if the provided move matches.
        /// This helps to prevent users from creating invalid moves and adding them to the queue.
        /// </summary>
        /// <returns><c>true</c> if the move is valid, <c>false</c> otherwise.</returns>
        /// <param name="move">The move to check.</param>
        public static bool IsValid(InternalMove move) 
        {
            string name = move.GetType().Name;
            bool isNameValid = false;
            for (int i = 0; i < validNames.Length; i++) {
                if (validNames[i].Equals(name)) {
                    isNameValid = true;
                    break;
                }
            }

            if (!isNameValid) {
                return false;
            }

            if (!"Moves.Basic".Equals(move.GetType().Namespace)) {
                return false;
            }

            return true;
        }
    }
}
