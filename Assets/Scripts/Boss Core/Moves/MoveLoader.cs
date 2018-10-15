using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Reflection;

public class MoveLoader {

    public void Load() {

        Assembly thisAssembly = Assembly.GetExecutingAssembly();

        Debug.Log("Loading in namespace: " + GetType().Namespace);
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);

        for (int i = 0; i < fields.Length; i++)
        {
            string fullName = GetType().Namespace + "." + fields[i].Name;

            System.Type type = thisAssembly.GetType(fullName);
            if (type == null)
            {
                Debug.LogError("Failed to locate class with type \"" + fullName + "\".");
                continue;
            }

            try 
            {
                fields[i].SetValue(null, System.Activator.CreateInstance(type));
            } 
            catch (System.MissingMethodException m) 
            {
                // We can optionally try harder to find other constructors here.
                // Maybe check for optional types; feed in defaults for value types, etc..

                Debug.LogError("Failed to find parameterless constructor for type \"" + fullName + "\".");
                continue;
            }

            Debug.Log("Successfully loaded Move " + fields[i].Name);
        }

        /*
        Profiler.BeginSample("Loading in moves via reflection");
        System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        foreach (System.Type type in assembly.GetTypes())
        {
            if (type.Namespace != null && type.Namespace.Equals(GetType().Namespace))
            {
                if (type.Name.Equals("Definitions"))
                {
                    continue;
                }

                //(System.Activator.CreateInstance(type) as Moves.IMoveDictionary).Load();

            }
        }
        Profiler.EndSample();
        */
    }
}
