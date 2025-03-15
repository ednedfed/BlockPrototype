using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
partial class WeaponAimRaycastSystem : SystemBase
{
    Transform _cameraTransform;

    public WeaponAimRaycastSystem(Transform cameraTransform)
    {
        _cameraTransform = cameraTransform;
    }

    protected override void OnUpdate()
    {
        //store this on a camera entity? decide how to map weapons to machine, can shared component be used?
        foreach (var cameraRaycast in SystemAPI.Query<RefRW<CameraRaycastComponent>>())
        {
            //todo: mask own machine, use camera per machine?
            Physics.Raycast(_cameraTransform.position,
                _cameraTransform.transform.forward, out var hitInfo,
                BlockGameConstants.WeaponAim.RaycastDistance, BlockGameConstants.GameLayers.InverseGhostLayerMask);

            cameraRaycast.ValueRW.hitInfo = hitInfo;

            UnityEngine.Debug.DrawLine(_cameraTransform.position, _cameraTransform.position + _cameraTransform.transform.forward * BlockGameConstants.WeaponAim.RaycastDistance, UnityEngine.Color.yellow, SystemAPI.Time.DeltaTime);
            UnityEngine.Debug.DrawLine(_cameraTransform.position, hitInfo.point, UnityEngine.Color.white, SystemAPI.Time.DeltaTime);

            foreach (var laser in SystemAPI.Query<RefRW<LaserComponent>>())
            {
                laser.ValueRW.aimPoint = (float3)cameraRaycast.ValueRW.hitInfo.point;
            }
        }
    }
}
