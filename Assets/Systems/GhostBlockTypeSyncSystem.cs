using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostBlockTypeSyncSystem : SystemBase
{
    GameObject _ghost;
    
    GhostBlockData ghostBlockData;
    BlockTypes blockTypes;

    public GhostBlockTypeSyncSystem(GameObject ghost, GhostBlockData ghostBlockData, BlockTypes blockTypes)
    {
        _ghost = ghost;
        this.ghostBlockData = ghostBlockData;
        this.blockTypes = blockTypes;
    }

    protected override void OnUpdate()
    {
        uint desiredBlockType = ghostBlockData.blockType;
        for (int i = 0; i <= BlockGameConstants.GhostBlock.BlockTypeCount; ++i)
        {
            if (Input.GetKeyDown((KeyCode)(i + KeyCode.Alpha0)))
            {
                desiredBlockType = (uint)i-1;
                break;
            }
        }

        if (desiredBlockType >= blockTypes.ghostBlockPrefabs.Length)
            return;

        //sync block type
        if (desiredBlockType != ghostBlockData.blockType)
        {
            ghostBlockData.blockType = desiredBlockType;

            //update ghost
            UpdateGhostPrefab(desiredBlockType);
        }
    }

    void UpdateGhostPrefab(uint desiredBlockType)
    {
        if (_ghost.transform.childCount > 0)
        {
            var child = _ghost.transform.GetChild(0);

            GameObject.Destroy(child.gameObject);
        }

        if (blockTypes.ghostBlockPrefabs[desiredBlockType] != null)
        {
            //parent type to ghost
            GameObject.Instantiate(blockTypes.ghostBlockPrefabs[desiredBlockType], _ghost.transform);
        }
    }
}
