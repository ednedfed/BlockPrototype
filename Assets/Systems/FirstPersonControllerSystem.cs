using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
partial class FirstPersonControllerSystem : SystemBase
{
    Rigidbody _rb;
    Transform _charTransform;

    public FirstPersonControllerSystem(Rigidbody rb, Transform charTransform)
    {
        _rb = rb;
        _charTransform = charTransform;
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

        UpdateRotation();
    }

    private void UpdateTranslation(float deltaTime)
    {
        var velocity = float3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            velocity.z += 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.z -= 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x -= 1;
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += 1;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y += 1;
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            velocity.y -= 1;
        }

        _rb.linearVelocity = _charTransform.rotation * velocity * BlockGameConstants.Drone.MoveSpeed * deltaTime;
    }

    void UpdateRotation()
    {
        var mouseDelta = Input.mousePositionDelta * BlockGameConstants.Drone.RotateSpeed;

        var delta3dx = new float3(0f, mouseDelta.x, 0f);

        _charTransform.Rotate(delta3dx, Space.World);

        var delta3dy = new float3(-mouseDelta.y, 0f, 0f);

        _charTransform.Rotate(delta3dy, Space.Self);
    }
}
