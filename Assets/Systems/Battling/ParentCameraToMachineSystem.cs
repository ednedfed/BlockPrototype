using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

//todo: use linked entity for this?
[DisableAutoCreation]
partial class ParentCameraToMachineSystem : SystemBase
{
    Transform _cameraAttachment;

    public ParentCameraToMachineSystem(Transform cameraAttachment)
    {
        _cameraAttachment = cameraAttachment;
    }

    protected override void OnUpdate()
    {
        foreach (var (machineTag, localTransform, physicsMass) in SystemAPI.Query<MachineTagComponent, LocalTransform, PhysicsMass>())
        {
            //is there a way to use camera in dots?
            var worldSpaceCom = localTransform.Position + math.mul(localTransform.Rotation, physicsMass.CenterOfMass);

            _cameraAttachment.position = worldSpaceCom + math.rotate(localTransform.Rotation, BlockGameConstants.Camera.MachineOffset);
            _cameraAttachment.rotation = localTransform.Rotation;
        }
    }
}
