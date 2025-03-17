using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

//this is likely to represent one machine
public class BlockGameObjectContainer : IBlockFactoryListener
{
    //todo segregate game object management until it can be replaced
    Dictionary<GameObject, int> _idPerGameObject = new Dictionary<GameObject, int>();
    Dictionary<int, GameObject> _gameObjectPerId = new Dictionary<int, GameObject>();
    BlockTypes _blockTypes;
    bool _activeOnAdd;

    public BlockGameObjectContainer(BlockTypes blockTypes, bool activeOnAdd)
    {
        _blockTypes = blockTypes;
        _activeOnAdd = activeOnAdd;//true for the whole gamemode
    }

    public void OnAdd(PlacedBlockData blockData)
    {
        GameObject gameObject = GameObject.Instantiate(_blockTypes.blockDatas[blockData.blockType].buildPrefab, blockData.position, blockData.rotation);
        gameObject.SetActive(_activeOnAdd);

        _gameObjectPerId.Add(blockData.blockId, gameObject);
        _idPerGameObject.Add(gameObject, blockData.blockId);
    }

    public void OnRemove(int blockId)
    {
        GameObject gaameObject = _gameObjectPerId[blockId];

        GameObject.Destroy(gaameObject);

        _gameObjectPerId.Remove(blockId);
        _idPerGameObject.Remove(gaameObject);
    }

    public int GetBlockId(GameObject root)
    {
        return _idPerGameObject[root];
    }

    public void OnClear(int machineId)
    {
        //todo: per machine game object management?
    }

    public void OnClear()
    {
        foreach (var cube in _gameObjectPerId)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        _idPerGameObject.Clear();
        _gameObjectPerId.Clear();
    }

    //todo: deprecate this
    public GameObject GetGameObject(int blockId)
    {
        return _gameObjectPerId[blockId];
    }

    public void SetPositionAndRotation(int blockId, float3 position, quaternion rotation)
    {
        var blockGameObject = _gameObjectPerId[blockId];
        blockGameObject.transform.position = position;
        blockGameObject.transform.rotation = rotation;
    }

}
