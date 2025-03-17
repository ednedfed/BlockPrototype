using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInputComponent : IComponentData
{
    public float2 moveVector;
    public bool fire;
}
