using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
partial class MouseLookRotationSystem : SystemBase
{
    Transform _cameraRoot;

    public MouseLookRotationSystem(Transform cameraRoot)
    {
        _cameraRoot = cameraRoot;
    }

    protected override void OnUpdate()
    {
        var mouseDelta = Input.mousePositionDelta * BlockGameConstants.Camera.RotateSpeed;

        var delta3dx = new float3(0f, mouseDelta.x, 0f);
        var delta3dy = new float3(-mouseDelta.y, 0f, 0f);

        _cameraRoot.Rotate(delta3dx, Space.World);
        _cameraRoot.Rotate(delta3dy, Space.Self);
    }
}
