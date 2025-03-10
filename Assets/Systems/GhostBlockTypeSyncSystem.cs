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
                var action = InputSystem.actions.FindAction($"BlockType{i}");//todo: extract this if we keep action per slot
                if (action != null && action.WasPressedThisFrame())
                {
                    desiredBlockType = (uint)i;
                    break;
                }
            }

            if (desiredBlockType >= _blockTypes.blockDatas.Length)
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
            if (ghostBlockPrefab.gameObject != null)
            {
                ghostBlockPrefab.gameObject.transform.position = ghostLocalTransforms.Position;
                ghostBlockPrefab.gameObject.transform.rotation = ghostLocalTransforms.Rotation;
            }
        }
    }

    void UpdateGhostPrefab(uint desiredBlockType, LocalTransform ghostLocalTransforms, GhostBlockPrefabComponent ghostBlockPrefab)
    {
        if (ghostBlockPrefab.gameObject != null)
        {
            GameObject.Destroy(ghostBlockPrefab.gameObject);
            ghostBlockPrefab.gameObject = null;
        }

        if (_blockTypes.blockDatas[desiredBlockType].ghostPrefab != null)
        {
            ghostBlockPrefab.gameObject = GameObject.Instantiate(_blockTypes.blockDatas[desiredBlockType].ghostPrefab);
        }
    }
}
