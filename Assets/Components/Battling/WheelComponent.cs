using Unity.Entities;

public struct WheelComponent : IComponentData
{
    public float currentSteerAngle;
    public float maxSteerAngle;
    public float axisRotation;//actually rotating the wheel
    internal float radius;
}
