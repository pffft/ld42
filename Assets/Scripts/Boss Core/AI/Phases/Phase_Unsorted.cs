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
                    if (System.Attribute.GetCustomAttribute(type, typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute)) != null) 
                    {
                        Debug.Log("Skipping compiler type: " + type);
                        continue;
                    }

                    try
                    {
                        Debug.Log("Trying to add type: " + type);
                        object rawObj = System.Activator.CreateInstance(type);
                        Debug.Log("Instantiated type parent type: " + rawObj.GetType().BaseType);
                        AISequence instantiatedSequence = (AISequence) rawObj;
                        if (instantiatedSequence == null) 
                        {
                            Debug.LogError("Instantiation failure! Got back null.");
                        }
                        AddSequence(10, instantiatedSequence);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Failed to add type: " + type);
                        Debug.LogError(e);
                    }
                }
            }
        }
    }
}