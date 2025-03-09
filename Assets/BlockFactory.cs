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
        if (blockType >= _blockTypes.blockPrefabs.Length || _blockTypes.blockPrefabs[blockType] == null)
            return;

        GameObject placedBlock = GameObject.Instantiate(_blockTypes.blockPrefabs[blockType], position, rotation);

        //todo: this will become entity collection?
        _saveData.Add(_idGen++, placedBlock, blockType);
    }

    public void RemoveBlock(int blockId)
    {
        //think about where to store game object reference
        GameObject.Destroy(_saveData.GetBlockGameObject(blockId));

        _saveData.Remove(blockId);
    }

    internal void ResetIdGen()
    {
        _idGen = 0;
    }
}
