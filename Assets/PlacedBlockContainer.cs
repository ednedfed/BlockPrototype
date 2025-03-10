using System.Collections.Generic;

//this is likely to represent one machine, could become entities eventually
public class PlacedBlockContainer : IBlockFactoryListener
{
    Dictionary<int, PlacedBlockData> _placedCubes = new Dictionary<int, PlacedBlockData>();

    public void OnAdd(PlacedBlockData blockData)
    {
        _placedCubes.Add(blockData.id, blockData);
    }

    public void OnRemove(int blockId)
    {
        _placedCubes.Remove(blockId);
    }

    public uint GetBlockType(int blockId)
    {
        return _placedCubes[blockId].blockType;
    }

    public void OnClear()
    {
        _placedCubes.Clear();
    }

    public Dictionary<int, PlacedBlockData>.ValueCollection GetVales()
    {
        return _placedCubes.Values;
    }
}
