using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

//todo: use linked entity for this?
[DisableAutoCreation]
partial class ParentCameraToMachineSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (machineTag, localTransform, physicsMass) in SystemAPI.Query<MachineTagComponent, LocalTransform, PhysicsMass>())
        {
            //is there a way to use camera in dots?
            var worldSpaceCom = localTransform.Position + math.mul(localTransform.Rotation, physicsMass.CenterOfMass);

            UnityEngine.Camera.main.transform.position = worldSpaceCom + BlockGameConstants.Camera.MachineOffset;
        }
    }
}
