using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Reflection;

public class MoveLoader {

    public virtual void Load() {

        Profiler.BeginSample("Loading " + GetType().Namespace);
        Assembly thisAssembly = Assembly.GetExecutingAssembly();

        // Grab all the fields within this namespace
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Static);

        for (int i = 0; i < fields.Length; i++)
        {
            // If the field is already instantiated, then we can skip it without reporting errors.
            if (fields[i].GetValue(null) != null) {
                continue;
            }

            // Try seeing what the type of the field is first- if it's custom, try to load that in.
            System.Type type = fields[i].FieldType;
            if (type == null || type.Name.Equals("Move"))
            {
                // Failing that, try to look up the class name by the field's name.
                string fullName = GetType().Namespace + "." + fields[i].Name;
                type = thisAssembly.GetType(fullName);
                if (type == null)
                {
                    Debug.LogError("Failed to locate class with type \"" + fullName + "\".");
                    continue;
                }
            }

            // Try to instantiate the type via a parameterless constructor.
            try 
            {
                Moves.Move move = System.Activator.CreateInstance(type) as Moves.Move;

                Debug.Log("Is move null?: " + (move == null));
                move.Initialize();
                fields[i].SetValue(null, move);
            } 
            catch (System.MissingMethodException) 
            {
                // We can optionally try harder to find other constructors here.
                // Maybe check for optional types; feed in defaults for value types, etc..

                Debug.LogError("Failed to find parameterless constructor for type \"" + type.FullName + "\".");
                continue;
            }
        }
        Profiler.EndSample();
    }
}
