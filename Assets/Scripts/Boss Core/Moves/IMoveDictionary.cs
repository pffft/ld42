using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moves
{
    public interface IMoveDictionary
    {

        /*
         * Loads this Move file's moves and initializes their sequences.
         */
        void Load();
    }
}
