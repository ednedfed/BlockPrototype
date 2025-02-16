using Unity.Mathematics;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public Rigidbody rb;

    float moveSpeed = 500f;
    float rotateSpeed = 0.3f;

    private void Start()
    {
        //don't have pointer waving around
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
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

        rb.linearVelocity = transform.rotation * velocity * moveSpeed * Time.deltaTime;
    }

    void UpdateRotation()
    {
        var mouseDelta = Input.mousePositionDelta * rotateSpeed;

        var delta3dx = new float3(0f, mouseDelta.x, 0f);

        transform.Rotate(delta3dx, Space.World);

        var delta3dy = new float3(-mouseDelta.y, 0f, 0f);

        transform.Rotate(delta3dy, Space.Self);
    }
}
