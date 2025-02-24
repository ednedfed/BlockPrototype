using Unity.Mathematics;
using UnityEngine;

class FirstPersonController
{
    public Rigidbody rb;
    public Transform charTransform;

    public void Start()
    {
        //don't have pointer waving around
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    public void FixedUpdate()
    {
        UpdateTranslation();

        UpdateRotation();
    }

    private void UpdateTranslation()
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

        rb.linearVelocity = charTransform.rotation * velocity * BlockGameConstants.Drone.MoveSpeed * Time.deltaTime;
    }

    void UpdateRotation()
    {
        var mouseDelta = Input.mousePositionDelta * BlockGameConstants.Drone.RotateSpeed;

        var delta3dx = new float3(0f, mouseDelta.x, 0f);

        charTransform.Rotate(delta3dx, Space.World);

        var delta3dy = new float3(-mouseDelta.y, 0f, 0f);

        charTransform.Rotate(delta3dy, Space.Self);
    }
}
