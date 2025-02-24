using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
partial class GhostBlockTypeSyncSystem : SystemBase
{
    BlockTypes _blockTypes;

    public GhostBlockTypeSyncSystem(BlockTypes blockTypes)
    {
        _blockTypes = blockTypes;
    }

    protected override void OnUpdate()
    {
        foreach (var (ghostBlockData, ghostLocalTransforms) in SystemAPI.Query<RefRW<GhostBlockDataComponent>, LocalTransform>())
        {
            uint desiredBlockType = ghostBlockData.ValueRO.blockType;

            for (int i = 0; i <= BlockGameConstants.GhostBlock.BlockTypeCount; ++i)
            {
                if (Input.GetKeyDown((KeyCode)(i + KeyCode.Alpha0)))
                {
                    desiredBlockType = (uint)i - 1;
                    break;
                }
            }

            if (desiredBlockType >= _blockTypes.ghostBlockPrefabs.Length)
                return;

            //sync block type
            if (desiredBlockType != ghostBlockData.ValueRO.blockType)
            {
                ghostBlockData.ValueRW.blockType = desiredBlockType;

                //update ghost
                UpdateGhostPrefab(desiredBlockType, ghostLocalTransforms);
            }
        }
    }

    void UpdateGhostPrefab(uint desiredBlockType, LocalTransform ghostLocalTransforms)
    {
        //todo: figure out parenting mesh
        /*
        if (_ghost.transform.childCount > 0)
        {
            var child = _ghost.transform.GetChild(0);

            GameObject.Destroy(child.gameObject);
        }
        */

        if (_blockTypes.ghostBlockPrefabs[desiredBlockType] != null)
        {
            //parent type to ghost
            // GameObject.Instantiate(blockTypes.ghostBlockPrefabs[desiredBlockType], _ghost.transform);
        }
    }
}
