using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
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

        UpdateTranslation(deltaTime);
    }

    private void UpdateTranslation(float deltaTime)
    {
        var velocity = float3.zero;
        var moveVector = _moveAction.ReadValue<Vector2>();

        velocity.z += moveVector.y;


        var angularVelocity = float3.zero;
        angularVelocity.y += moveVector.x;

        //todo: get wheels and apply force

        foreach (var (velocityComponent, physicsMass, machineTag) in SystemAPI.Query<RefRW<PhysicsVelocity>, PhysicsMass, MachineTagComponent>())
        {
            PhysicsComponentExtensions.ApplyLinearImpulse(ref velocityComponent.ValueRW, physicsMass, velocity);

            PhysicsComponentExtensions.ApplyAngularImpulse(ref velocityComponent.ValueRW, physicsMass, angularVelocity);
        }
    }
}
