using Unity.Entities;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class DeleteBlockSystem : SystemBase
{
    BlockFactory _blockFactory;
    InputAction _deleteInput;

    public DeleteBlockSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;

        _deleteInput = InputSystem.actions.FindAction("DeleteBlock");
    }

    protected override void OnUpdate()
    {
        foreach (var hitObject in SystemAPI.Query<HitObjectComponent>())
        {
            if (_deleteInput.WasPressedThisFrame() && hitObject.isCube)
            {
                _blockFactory.RemoveBlock(hitObject.hitBlockId);
            }
        }
    }
}
