using System.Collections.Generic;
using UnityEngine;

public struct PlacedBlockData
{
    public int id;
    public uint blockType;
    public Vector3 position;
    public Quaternion rotation;
}

public interface IBlockFactoryListener
{
    void OnAdd(PlacedBlockData blockData);
    void OnRemove(int blockId);
    void OnClear();
}

public class BlockFactory
{
    List<IBlockFactoryListener> _listeners;

    readonly BlockTypes _blockTypes;

    int _idGen;

    public BlockFactory(BlockTypes blockTypes)
    {
        _listeners = new List<IBlockFactoryListener>();

        _blockTypes = blockTypes;
    }
    public void RegisterBlockListener(IBlockFactoryListener listener)
    {
        _listeners.Add(listener);
    }

    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        //todo: replace prefabs with pure data
        if (blockType >= _blockTypes.blockDatas.Length || _blockTypes.blockDatas[blockType].buildPrefab == null)
            return;

        var blockId = _idGen++;

        var blockData = new PlacedBlockData
        {
            id = blockId,
            blockType = blockType,
            position = position,
            rotation = rotation
        };

        foreach (var listener in _listeners)
        {
            listener.OnAdd(blockData);
        }
    }

    public void RemoveBlock(int blockId)
    {
        foreach (var listener in _listeners)
        {
            listener.OnRemove(blockId);
        }
    }

    public void Clear()
    {
        _idGen = 0;

        foreach (var listener in _listeners)
        {
            listener.OnClear();
        }
    }
}
