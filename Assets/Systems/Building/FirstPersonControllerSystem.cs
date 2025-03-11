using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[DisableAutoCreation]
partial class FirstPersonControllerSystem : SystemBase
{
    Rigidbody _rb;
    Transform _charTransform;
    InputAction _moveAction;
    InputAction _jumpAction;
    InputAction _crouchAction;

    public FirstPersonControllerSystem(Rigidbody rb, Transform charTransform)
    {
        _rb = rb;
        _charTransform = charTransform;

        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _crouchAction = InputSystem.actions.FindAction("Crouch");
    }

    protected override void OnCreate()
    {
        //don't have pointer waving around
        Cursor.lockState = CursorLockMode.Locked;
    }

    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        var velocity = float3.zero;
        var moveVector = _moveAction.ReadValue<Vector2>();

        velocity.z += moveVector.y;
        velocity.x += moveVector.x;

        if (_jumpAction.IsPressed())
        {
            velocity.y += 1;
        }
        if (_crouchAction.IsPressed())
        {
            velocity.y -= 1;
        }

        _rb.linearVelocity = _charTransform.rotation * velocity * BlockGameConstants.Drone.MoveSpeed * deltaTime;
    }
}
