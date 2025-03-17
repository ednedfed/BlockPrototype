using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[UpdateAfter(typeof(TransformSystemGroup))]
partial class ParentCameraToMachineSystem : SystemBase
{
    Transform _cameraAttachment;

    public ParentCameraToMachineSystem(Transform cameraAttachment)
    {
        _cameraAttachment = cameraAttachment;
    }

    DebugDraw.PosTracker _posTracker = new DebugDraw.PosTracker();

    protected override void OnUpdate()
    {
        foreach (var (machineTag, localTransform, physicsMass) in SystemAPI.Query<MachineTagComponent, LocalToWorld, PhysicsMass>())
        {
            //for now only control one machine
            if (machineTag.machineId != 0)
                continue;

            //is there a way to use camera in dots?
            var worldSpaceCom = localTransform.Position + math.mul(localTransform.Rotation, physicsMass.CenterOfMass);

            _cameraAttachment.position = worldSpaceCom + math.rotate(localTransform.Rotation, BlockGameConstants.Camera.MachineOffset);
            _cameraAttachment.rotation = localTransform.Rotation;


            _posTracker.Update(_cameraAttachment.position, SystemAPI.Time.DeltaTime, Color.red);
        }
    }
}
