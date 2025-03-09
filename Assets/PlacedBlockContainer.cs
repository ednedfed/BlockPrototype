using System.Collections.Generic;
using UnityEngine;

public class PlacedBlockContainer
{
    public class PlacedBlockData
    {
        public int id;
        public uint blockType;
        public Vector3 position;
        public Quaternion rotation;
    }

    Dictionary<int, PlacedBlockData> _placedCubes = new Dictionary<int, PlacedBlockData>();

    //todo segregate game object management until it can be replaced
    Dictionary<GameObject, int> _idPerGameObject = new Dictionary<GameObject, int>();
    Dictionary<int, GameObject> _gameObjectPerId = new Dictionary<int, GameObject>();
    BlockTypes _blockTypes;

    public PlacedBlockContainer(BlockTypes blockTypes)
    {
        _blockTypes = blockTypes;
    }

    public void Add(int blockId, uint blockType, Vector3 position, Quaternion rotation)
    {
        _placedCubes.Add(blockId,
            new PlacedBlockData
            {
                id = blockId,
                blockType = blockType,
                position = position,
                rotation = rotation
            }
        );

        GameObject gameObject = GameObject.Instantiate(_blockTypes.blockPrefabs[blockType], position, rotation);

        _gameObjectPerId.Add(blockId, gameObject);
        _idPerGameObject.Add(gameObject, blockId);
    }

    public void Remove(int blockId)
    {
        GameObject gaameObject = _gameObjectPerId[blockId];

        GameObject.Destroy(gaameObject);

        _gameObjectPerId.Remove(blockId);
        _idPerGameObject.Remove(gaameObject);

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

    public void Clear()
    {
        foreach (var cube in _gameObjectPerId)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        _idPerGameObject.Clear();
        _gameObjectPerId.Clear();
        _placedCubes.Clear();
    }

    //this is not clean yet?
    public Dictionary<int, PlacedBlockData>.ValueCollection GetVales()
    {
        return _placedCubes.Values;
    }
}
