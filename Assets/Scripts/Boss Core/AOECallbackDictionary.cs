using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static AOE;

public class AOECallbackDictionary {
    public static AOECallbackDelegate NOTHING = (self) => { };

    public static AOECallbackDelegate DONT_DESTROY_OOB = (self) => { self.data.shouldDestroyOnOutOfBounds = false; };
}
