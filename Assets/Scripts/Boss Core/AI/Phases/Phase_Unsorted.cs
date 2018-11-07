using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Reflection;

using AI;

namespace Phases
{
    public class Phase_Unsorted : AIPhase
    {
        public Phase_Unsorted()
        {
            MaxHealth = 100;

            System.Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (System.Type type in types)
            {
                if (type.Namespace != null && type.Namespace.Equals("Moves.Unsorted"))
                {
                    try
                    {
                        Debug.Log("Adding type: " + type);
                        AddSequence(10, System.Activator.CreateInstance(type) as AISequence);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("Failed to add type: " + type);
                    }
                }
            }
        }
    }
}