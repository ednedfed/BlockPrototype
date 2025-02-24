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
        foreach (var (ghostBlockData, ghostBlockPrefab, ghostLocalTransforms) in SystemAPI.Query<RefRW<GhostBlockDataComponent>, GhostBlockPrefabComponent, LocalTransform>())
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
                UpdateGhostPrefab(desiredBlockType, ghostLocalTransforms, ghostBlockPrefab);
            }
        }

        //todo: figure out parenting
        foreach (var (ghostBlockPrefab, ghostLocalTransforms) in SystemAPI.Query<GhostBlockPrefabComponent, LocalTransform>())
        {
            if (ghostBlockPrefab.ghostBlockPrefab != null)
            {
                ghostBlockPrefab.ghostBlockPrefab.transform.position = ghostLocalTransforms.Position;
                ghostBlockPrefab.ghostBlockPrefab.transform.rotation = ghostLocalTransforms.Rotation;
            }
        }
    }

    void UpdateGhostPrefab(uint desiredBlockType, LocalTransform ghostLocalTransforms, GhostBlockPrefabComponent ghostBlockPrefab)
    {
        if (ghostBlockPrefab.ghostBlockPrefab != null)
        {
            GameObject.Destroy(ghostBlockPrefab.ghostBlockPrefab);
            ghostBlockPrefab.ghostBlockPrefab = null;
        }

        if (_blockTypes.ghostBlockPrefabs[desiredBlockType] != null)
        {
            ghostBlockPrefab.ghostBlockPrefab = GameObject.Instantiate(_blockTypes.ghostBlockPrefabs[desiredBlockType]);
        }
    }
}
