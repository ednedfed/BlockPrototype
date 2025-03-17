using Unity.Entities;
using Unity.Physics;
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
        foreach (var (cameraRaycast, machineTagComponent) in SystemAPI.Query<RefRW<CameraRaycastComponent>, MachineTagComponent>())
        {
            //for now only control one machine
            if (machineTagComponent.machineId != 0)
                continue;

            var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            RaycastInput input = new RaycastInput()
            {
                Start = _cameraTransform.position,
                End = _cameraTransform.position + _cameraTransform.transform.forward * BlockGameConstants.WeaponAim.RaycastDistance,
                Filter = new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u,
                    GroupIndex = machineTagComponent.collisionGroupIndex
                }
            };

            bool haveHit = collisionWorld.CastRay(input, out var hit);
            cameraRaycast.ValueRW.hit = hit;

            UnityEngine.Debug.DrawLine(_cameraTransform.position, _cameraTransform.position + _cameraTransform.transform.forward * BlockGameConstants.WeaponAim.RaycastDistance, UnityEngine.Color.yellow, SystemAPI.Time.DeltaTime);
            UnityEngine.Debug.DrawLine(_cameraTransform.position, hit.Position, UnityEngine.Color.white, SystemAPI.Time.DeltaTime);

            foreach (var laser in SystemAPI.Query<RefRW<LaserComponent>>()
                .WithSharedComponentFilter(new MachineIdComponent { machineId = machineTagComponent.machineId }))
            {
                laser.ValueRW.aimPoint = cameraRaycast.ValueRW.hit.Position;
            }
        }
    }
}
