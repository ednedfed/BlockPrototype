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
        foreach (var (playerInput, machineTag) in SystemAPI.Query<RefRW<PlayerInputComponent>, MachineTagComponent>())
        {
            //for now only control one machine
            if (machineTag.machineId != 0)
                continue;

            playerInput.ValueRW.moveVector = _moveAction.ReadValue<Vector2>();
        }
    }
}
