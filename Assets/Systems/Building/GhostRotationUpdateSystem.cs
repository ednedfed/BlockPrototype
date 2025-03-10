using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class GhostRotationUpdateSystem : SystemBase
{
    InputAction _rotateInput;

    public GhostRotationUpdateSystem()
    {
        _rotateInput = InputSystem.actions.FindAction("Rotate");
    }

    protected override void OnUpdate()
    {
        foreach (var ghostBlockData in SystemAPI.Query<RefRW<GhostBlockDataComponent>>())
        {
            ghostBlockData.ValueRW.direction += Mathf.RoundToInt(_rotateInput.ReadValue<float>());

            ghostBlockData.ValueRW.direction += BlockGameConstants.GhostBlock.NumDirections;
            ghostBlockData.ValueRW.direction %= BlockGameConstants.GhostBlock.NumDirections;
        }
    }
}
