using UnityEngine;

public class BlockFactory
{
    readonly BlockTypes _blockTypes;
    readonly PlacedBlockContainer _saveData;

    int _idGen;

    public BlockFactory(BlockTypes blockTypes, PlacedBlockContainer saveData)
    {
        _blockTypes = blockTypes;
        _saveData = saveData;
    }

    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        //todo: replace prefabs with pure data
        if (blockType >= _blockTypes.blockDatas.Length || _blockTypes.blockDatas[blockType].buildPrefab == null)
            return;

        //todo: this will become entity collection?
        _saveData.Add(_idGen++, blockType, position, rotation);
    }

    public void RemoveBlock(int blockId)
    {
        _saveData.Remove(blockId);
    }

    internal void ResetIdGen()
    {
        _idGen = 0;
    }
}
