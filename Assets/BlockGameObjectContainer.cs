using System.Collections.Generic;
using UnityEngine;

//this is likely to represent one machine
public class BlockGameObjectContainer : IBlockFactoryListener
{
    //todo segregate game object management until it can be replaced
    Dictionary<GameObject, int> _idPerGameObject = new Dictionary<GameObject, int>();
    Dictionary<int, GameObject> _gameObjectPerId = new Dictionary<int, GameObject>();
    BlockTypes _blockTypes;

    public BlockGameObjectContainer(BlockTypes blockTypes)
    {
        _blockTypes = blockTypes;
    }

    public void OnAdd(PlacedBlockData blockData)
    {
        GameObject gameObject = GameObject.Instantiate(_blockTypes.blockDatas[blockData.blockType].buildPrefab, blockData.position, blockData.rotation);

        _gameObjectPerId.Add(blockData.id, gameObject);
        _idPerGameObject.Add(gameObject, blockData.id);
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

    public void OnClear()
    {
        foreach (var cube in _gameObjectPerId)
        {
            GameObject.Destroy(cube.Value.gameObject);
        }

        _idPerGameObject.Clear();
        _gameObjectPerId.Clear();
    }
}
