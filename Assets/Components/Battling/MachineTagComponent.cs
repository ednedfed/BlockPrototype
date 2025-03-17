using Unity.Entities;

public struct MachineTagComponent : ISharedComponentData
{
    public int collisionGroupIndex;
    public int machineId;
}
