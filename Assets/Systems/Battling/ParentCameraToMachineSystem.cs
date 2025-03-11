using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//todo: use linked entity for this?
[DisableAutoCreation]
partial class ParentCameraToMachineSystem : SystemBase
{
    float3 offset = new float3(0f, 3f, -4f);

    protected override void OnUpdate()
    {
        foreach (var (machineTag, localTransform) in SystemAPI.Query<MachineTagComponent, LocalTransform>())
        {
            //is there really a way to use camera in dots?
            UnityEngine.Camera.main.transform.position = localTransform.Position + offset;//todo: add camera distance once entity exists
        }
    }
}
