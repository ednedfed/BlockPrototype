using System.Collections.Generic;
using UnityEngine;

public struct PlacedBlockData
{
    public int id;
    public uint blockType;
    public BlockCategory blockCategory;
    public Vector3 position;
    public Quaternion rotation;
}

public interface IBlockFactoryListenerWithCategory : IBlockFactoryListener
{
    BlockCategory blockCategory { get; }
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
    List<IBlockFactoryListener>[] _listenersPerCategory;

    readonly BlockTypes _blockTypes;

    int _idGen;

    public BlockFactory(BlockTypes blockTypes)
    {
        _blockTypes = blockTypes;

        _listeners = new List<IBlockFactoryListener>();
        _listenersPerCategory = new List<IBlockFactoryListener>[_blockTypes.blockDatas.Length];

        for (int i = 0; i < _listenersPerCategory.Length; ++i)
        { 
            _listenersPerCategory[i] = new List<IBlockFactoryListener>();
        }
    }
    public void RegisterBlockListener(IBlockFactoryListener listener)
    {
        _listeners.Add(listener);
    }

    public void RegisterBlockListenerWithCategory(IBlockFactoryListenerWithCategory listener)
    {
        _listenersPerCategory[(int)listener.blockCategory].Add(listener);
    }

    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        //todo: replace prefabs with pure data
        if (blockType >= _blockTypes.blockDatas.Length || _blockTypes.blockDatas[blockType].buildPrefab == null)
            return;

        var blockCategory = _blockTypes.blockDatas[blockType].blockCategory;

        var blockId = _idGen++;

        var blockData = new PlacedBlockData
        {
            id = blockId,
            blockCategory = blockCategory,
            blockType = blockType,
            position = position,
            rotation = rotation
        };

        foreach (var listener in _listeners)
        {
            listener.OnAdd(blockData);
        }

        foreach (var listener in _listenersPerCategory[(int)blockCategory])
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
        foreach (var listeners in _listenersPerCategory)
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
        foreach (var listeners in _listenersPerCategory)
        {
            foreach (var listener in listeners)
            {
                listener.OnClear();
            }
        }
    }
}
