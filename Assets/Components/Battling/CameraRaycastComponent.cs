using Unity.Entities;

public struct CameraRaycastComponent : IComponentData
{
    public Unity.Physics.RaycastHit hit;
}
