using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    //todo: make a factory
    public int idGen;
    public Dictionary<int, GameObject> placedCubes = new Dictionary<int, GameObject>();

    internal void Clear()
    {
        foreach (var cube in placedCubes)
        {
            GameObject.Destroy(cube.Value);
        }

        placedCubes.Clear();

        idGen = 0;
    }
}
