using System.Reflection;
using UnityEngine;

public class MainGameContext : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //todo: also load static data?

        BlockTypes blockTypes = GameObject.FindFirstObjectByType<BlockTypes>();

        HitObject hitObject = new HitObject();
        GhostBlockData ghostBlockData = new GhostBlockData();
        SaveData saveData = new SaveData();
        BlockFactory blockFactory = new BlockFactory(blockTypes, saveData);

        //inject, but later use ECS world so don't write an entire DI system yet
        InjectIntoMonobehaviours(blockTypes);
        InjectIntoMonobehaviours(hitObject);
        InjectIntoMonobehaviours(ghostBlockData);
        InjectIntoMonobehaviours(saveData);
        InjectIntoMonobehaviours(blockFactory);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InjectIntoMonobehaviours<T>(T instance) where T : class
    {
        // Get all GameObjects in the scene
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

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
