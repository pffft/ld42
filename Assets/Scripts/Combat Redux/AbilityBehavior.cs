using System.Collections;
using UnityEngine;

namespace Combat
{
    public abstract class AbilityBehavior
    {
        public enum Result { FINISHED, STOPPED, INTERRUPTED }

        public GameObject Blackboard { get; private set; }
        public Ability Ability { get; private set; }

        public void Initialize(GameObject blackboard, Ability ability)
        {
            Blackboard = blackboard;
            Ability = ability;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract bool Start();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerator Update();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public abstract void Finish(Result result);
    }
}
