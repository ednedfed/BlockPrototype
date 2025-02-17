using UnityEngine;

public class BlockFactory : MonoBehaviour
{
    public SaveData saveData;
    public GameObject[] blockToInstantiate;

    //todo: register block types
    public void InstantiateBlock(uint blockType, Vector3 position, Quaternion rotation)
    {
        //for now use all one material
        GameObject placedBlock = GameObject.Instantiate(blockToInstantiate[blockType], position, rotation);
        var blockId = placedBlock.AddComponent<BlockId>();
        blockId.blockId = saveData.idGen++;

        //todo: this will become entity collection?
        saveData.placedCubes.Add(blockId.blockId, placedBlock);
    }

    public void RemoveBlock(BlockId blockId)
    {
        var gameObject = saveData.placedCubes[blockId.blockId];
        saveData.placedCubes.Remove(blockId.blockId);

        GameObject.Destroy(gameObject);
    }
}
