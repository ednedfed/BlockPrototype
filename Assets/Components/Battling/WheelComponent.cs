using Unity.Entities;
using Unity.Mathematics;

public struct WheelComponent : IComponentData
{
    public float currentSteerAngle;
    public float maxSteerAngle;
}
