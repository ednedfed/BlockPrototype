using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public BlockTypes blockTypes;
    public SaveData saveData;

    //todo: register block types
    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        if (blockType >= blockTypes.blockPrefabs.Length)
            return;

        //for now use all one material
        GameObject placedBlock = GameObject.Instantiate(blockTypes.blockPrefabs[blockType], position, rotation);
        var blockId = placedBlock.AddComponent<BlockIdComponent>();
        blockId.blockId = saveData.idGen++;

        //todo: this will become entity collection?
        saveData.placedCubes.Add(blockId.blockId,
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
        var placedBlock = saveData.placedCubes[blockId.blockId];
        saveData.placedCubes.Remove(blockId.blockId);

        GameObject.Destroy(placedBlock.gameObject);
    }
}
