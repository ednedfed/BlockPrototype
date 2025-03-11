using Unity.Entities;

public struct GhostBlockDataComponent : IComponentData
{
    public uint blockType;
    public int direction;//todo: split?
}
