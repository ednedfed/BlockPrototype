using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public class PlacedBlockData
    {
        public int id;
        public uint blockType;
        public GameObject gameObject;
    }

    public Dictionary<int, PlacedBlockData> placedCubes = new Dictionary<int, PlacedBlockData>();

    public void Clear()
    {
        foreach (var cube in placedCubes)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        placedCubes.Clear();
    }
}
