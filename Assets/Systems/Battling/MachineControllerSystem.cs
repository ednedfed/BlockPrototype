using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class MachineControllerSystem : SystemBase
{
    InputAction _moveAction;

    public MachineControllerSystem()
    {
        _moveAction = InputSystem.actions.FindAction("Move");
    }

    protected override void OnCreate()
    {
        //don't have pointer waving around
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var playerInput in SystemAPI.Query<RefRW<PlayerInputComponent>>())
        {
            playerInput.ValueRW.moveVector = _moveAction.ReadValue<Vector2>();
        }
    }
}
