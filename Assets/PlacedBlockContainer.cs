using System.Collections.Generic;
using UnityEngine;

public class PlacedBlockContainer
{
    public class PlacedBlockData
    {
        public int id;
        public uint blockType;
        public GameObject gameObject;
    }

    Dictionary<int, PlacedBlockData> _placedCubes = new Dictionary<int, PlacedBlockData>();
    Dictionary<GameObject, int> _idPerGameObject = new Dictionary<GameObject, int>();

    public void Add(int blockId, GameObject blockGameObject, uint blockType)
    {
        _placedCubes.Add(blockId,
            new PlacedBlockData
            {
                id = blockId,
                gameObject = blockGameObject,
                blockType = blockType,
            }
        );

        _idPerGameObject.Add(blockGameObject, blockId);
    }

    public void Remove(int blockId)
    {
        _idPerGameObject.Remove(_placedCubes[blockId].gameObject);
        _placedCubes.Remove(blockId);
    }

    public uint GetBlockType(int blockId)
    {
        return _placedCubes[blockId].blockType;
    }

    public int GetBlockId(GameObject root)
    {
        return _idPerGameObject[root];
    }

    public GameObject GetBlockGameObject(int blockId)
    {
        return _placedCubes[blockId].gameObject;
    }

    public void Clear()
    {
        foreach (var cube in _placedCubes)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        _placedCubes.Clear();
    }

    //this is not clean yet?
    public Dictionary<int, PlacedBlockData>.ValueCollection GetVales()
    {
        return _placedCubes.Values;
    }
}
