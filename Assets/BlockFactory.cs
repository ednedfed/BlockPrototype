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
    List<IBlockFactoryListener>[] _listenersPerType;

    readonly BlockTypes _blockTypes;

    int _idGen;

    public BlockFactory(BlockTypes blockTypes)
    {
        _blockTypes = blockTypes;

        _listeners = new List<IBlockFactoryListener>();
        _listenersPerType = new List<IBlockFactoryListener>[_blockTypes.blockDatas.Length];

        for (int i = 0; i < _listenersPerType.Length; ++i)
        { 
            _listenersPerType[i] = new List<IBlockFactoryListener>();
        }
    }
    public void RegisterBlockListener(IBlockFactoryListener listener)
    {
        _listeners.Add(listener);
    }

    public void RegisterBlockListener(uint blockType, IBlockFactoryListener listener)
    {
        _listenersPerType[blockType].Add(listener);
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

        foreach (var listener in _listenersPerType[blockType])
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

        //todo: don't know types
        foreach (var listeners in _listenersPerType)
        {
            foreach (var listener in listeners)
            {
                listener.OnRemove(blockId);
            }
        }
    }

    public void Clear()
    {
        _idGen = 0;

        foreach (var listener in _listeners)
        {
            listener.OnClear();
        }

        //all types
        foreach (var listeners in _listenersPerType)
        {
            foreach (var listener in listeners)
            {
                listener.OnClear();
            }
        }
    }
}
