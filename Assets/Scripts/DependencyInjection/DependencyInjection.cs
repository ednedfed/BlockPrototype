using System.Reflection;
using UnityEngine;

public static class DependencyInjection
{
    public static void InjectIntoMonobehaviours<T>(T instance) where T : class
    {
        // Get all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        foreach (GameObject obj in allObjects)
        {
            // Get all MonoBehaviours on this GameObject
            MonoBehaviour[] components = obj.GetComponents<InjectableBehaviour>();

            foreach (InjectableBehaviour comp in components)
            {
                if (comp == null) continue;

                // Get all fields in the MonoBehaviour
                FieldInfo[] fields = comp.GetType()
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                foreach (FieldInfo field in fields)
                {
                    // Check if the field type matches the instance type
                    if (field.FieldType == typeof(T))
                    {
                        Debug.Log($"Assigning {typeof(T).Name} to {comp.name}'s {field.Name}");

                        // Assign the instance to the field
                        field.SetValue(comp, instance);
                    }
                }
            }
        }
    }
}
