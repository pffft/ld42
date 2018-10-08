using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AOEs.AOE;

namespace AOEs
{
    public class AOECallbackDictionary
    {
        public static AOECallbackDelegate NOTHING = (self) => { Debug.Log("AOE callback nothing"); };

        public static AOECallbackDelegate DONT_DESTROY_OOB = (self) => { self.data.shouldDestroyOnOutOfBounds = false; };
    }
}
