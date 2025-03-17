using System.Collections.Generic;

public class PlacedBlockContainer : IBlockFactoryListener
{
    //store separate per machine internally to allow us to filter
    Dictionary<int, int> _machinePerBlock = new Dictionary<int, int>();//remember machine id
    Dictionary<int, Dictionary<int, PlacedBlockData>> _placedCubesPerMachine = new Dictionary<int, Dictionary<int, PlacedBlockData>>();

    //structural changes are more complex so that regular queries can be simpler
    public void OnAdd(PlacedBlockData blockData)
    {
        if (_placedCubesPerMachine.TryGetValue(blockData.machineId, out var machineBlocks) == false)
        {
            _placedCubesPerMachine[blockData.machineId] = new Dictionary<int, PlacedBlockData>();
            machineBlocks = _placedCubesPerMachine[blockData.machineId];
        }

        machineBlocks.Add(blockData.blockId, blockData);
        _machinePerBlock.Add(blockData.blockId, blockData.machineId);
    }

    public void OnRemove(int blockId)
    {
        var machineId = _machinePerBlock[blockId];
        _placedCubesPerMachine[machineId].Remove(blockId);
        _machinePerBlock.Remove(blockId);
    }

    public uint GetBlockType(int blockId)
    {
        var machineId = _machinePerBlock[blockId];
        return _placedCubesPerMachine[machineId][blockId].blockType;
    }

    public BlockCategory GetBlockCategory(int blockId)
    {
        var machineId = _machinePerBlock[blockId];
        return _placedCubesPerMachine[machineId][blockId].blockCategory;
    }

    public void OnClear(int machineId)
    {
        //allow clear even if value didn't exist yet
        if (_placedCubesPerMachine.TryGetValue(machineId, out var cubesPerMachine))
        {
            foreach (var block in cubesPerMachine)
            {
                _machinePerBlock.Remove(block.Key);
            }

            _placedCubesPerMachine.Remove(machineId);
        }
    }

    public void OnClear()
    {
        _machinePerBlock.Clear();
        _placedCubesPerMachine.Clear();
    }

    public Dictionary<int, PlacedBlockData>.ValueCollection GetValues(int machineId)
    {
        return _placedCubesPerMachine[machineId].Values;
    }
}
