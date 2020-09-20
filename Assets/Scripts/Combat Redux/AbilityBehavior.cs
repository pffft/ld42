using System.Collections;
using UnityEngine;

namespace Combat
{
    public abstract class AbilityBehavior
    {
        public enum Result { FINISHED, STOPPED, INTERRUPTED }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blackboard"></param>
        /// <param name="ability"></param>
        /// <returns></returns>
        public abstract bool Start(GameObject blackboard, Ability ability);

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
