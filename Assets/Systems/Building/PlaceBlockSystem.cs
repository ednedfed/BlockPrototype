using Unity.Entities;
using Unity.Transforms;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class PlaceBlockSystem : SystemBase
{
    BlockFactory _blockFactory;
    InputAction _placeBlockAction;

    public PlaceBlockSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;

        _placeBlockAction = InputSystem.actions.FindAction("PlaceBlock");
    }

    protected override void OnUpdate()
    {
        foreach (var hitObject in SystemAPI.Query<HitObjectComponent>())
        {
            if (_placeBlockAction.WasPressedThisFrame() && hitObject.isOverlapping == false)
            {
                foreach (var (ghostBlockData, localTranforms) in SystemAPI.Query<GhostBlockDataComponent, LocalTransform>())
                {
                    _blockFactory.AddBlock(ghostBlockData.blockType, localTranforms.Position, localTranforms.Rotation, BlockGameConstants.SaveGame.BuildMachineId);
                }
            }
        }
    }
}
