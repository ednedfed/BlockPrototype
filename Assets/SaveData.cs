using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public class PlacedBlockData
    {
        public int id;
        public uint blockType;
        public GameObject gameObject;
    }

    //todo: make a factory
    public int idGen;
    public Dictionary<int, PlacedBlockData> placedCubes = new Dictionary<int, PlacedBlockData>();

    internal void Clear()
    {
        foreach (var cube in placedCubes)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        placedCubes.Clear();

        idGen = 0;
    }
}
