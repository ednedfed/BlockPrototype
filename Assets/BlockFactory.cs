using System.Collections.Generic;
using UnityEngine;

public struct PlacedBlockData
{
    public readonly int machineId;
    public readonly int blockId;
    public readonly uint blockType;
    public readonly BlockCategory blockCategory;
    public readonly Vector3 position;
    public readonly Quaternion rotation;

    public PlacedBlockData(int machineId, int blockId, uint blockType, BlockCategory blockCategory, Vector3 position, Quaternion rotation)
    {
        this.machineId = machineId;
        this.blockId = blockId;
        this.blockType = blockType;
        this.blockCategory = blockCategory;
        this.position = position;
        this.rotation = rotation;
    }
}

public interface IBlockFactoryListenerWithCategory : IBlockFactoryListener
{
    BlockCategory blockCategory { get; }
}

public interface IBlockFactoryListener
{
    void OnAdd(PlacedBlockData blockData);
    void OnRemove(int blockId);
    void OnClear(int machineId);//clear slot only
    void OnClear();//clear everything
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

    public void AddBlock(uint blockType, Vector3 position, Quaternion rotation, int machineId)
    {
        //todo: replace prefabs with pure data
        if (blockType >= _blockTypes.blockDatas.Length || _blockTypes.blockDatas[blockType].buildPrefab == null)
            return;

        var blockCategory = _blockTypes.blockDatas[blockType].blockCategory;

        var blockId = _idGen++;

        var blockData = new PlacedBlockData(
            machineId,
            blockId,
            blockType,
            blockCategory,
            position,
            rotation
        );

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

    //do not reset idgen when it's per machine
    public void Clear(int machineId)
    {
        foreach (var listener in _listeners)
        {
            listener.OnClear(machineId);
        }

        //all types
        foreach (var listeners in _listenersPerCategory)
        {
            foreach (var listener in listeners)
            {
                listener.OnClear(machineId);
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
