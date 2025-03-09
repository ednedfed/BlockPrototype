using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class GhostBlockTypeSyncSystem : SystemBase
{
    BlockTypes _blockTypes;
    PlacedBlockContainer _placedBlockContainer;
    InputAction _pickBlockAction;

    public GhostBlockTypeSyncSystem(BlockTypes blockTypes, PlacedBlockContainer placedBlockContainer)
    {
        _blockTypes = blockTypes;
        _placedBlockContainer = placedBlockContainer;

        _pickBlockAction = InputSystem.actions.FindAction("PickBlock");
    }

    protected override void OnUpdate()
    {
        foreach (var (ghostBlockData, ghostHit, ghostBlockPrefab, ghostLocalTransforms) in SystemAPI.Query<RefRW<GhostBlockDataComponent>, HitObjectComponent, GhostBlockPrefabComponent, LocalTransform>())
        {
            uint desiredBlockType = ghostBlockData.ValueRO.blockType;

            if (_pickBlockAction.WasPressedThisFrame() && ghostHit.isCube)
            {
                desiredBlockType = _placedBlockContainer.GetBlockType(ghostHit.hitBlockId);
            }

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
