using System;
using UnityEngine;

public class BlockFactory
{
    readonly BlockTypes _blockTypes;
    readonly SaveData _saveData;

    int _idGen;

    public BlockFactory(BlockTypes blockTypes, SaveData saveData)
    {
        _blockTypes = blockTypes;
        _saveData = saveData;
    }

    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        if (blockType >= _blockTypes.blockPrefabs.Length || _blockTypes.blockPrefabs[blockType] == null)
            return;

        //for now use all one material
        GameObject placedBlock = GameObject.Instantiate(_blockTypes.blockPrefabs[blockType], position, rotation);
        var blockId = placedBlock.AddComponent<BlockIdComponent>();
        blockId.blockId = _idGen++;

        //todo: this will become entity collection?
        _saveData.placedCubes.Add(blockId.blockId,
            new SaveData.PlacedBlockData
            {
                id = blockId.blockId,
                gameObject = placedBlock,
                blockType = blockType,
            }
        );
    }

    public void RemoveBlock(BlockIdComponent blockId)
    {
        var placedBlock = _saveData.placedCubes[blockId.blockId];
        _saveData.placedCubes.Remove(blockId.blockId);

        GameObject.Destroy(placedBlock.gameObject);
    }

    internal void ResetIdGen()
    {
        _idGen = 0;
    }
}
