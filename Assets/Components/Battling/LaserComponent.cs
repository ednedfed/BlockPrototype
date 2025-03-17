using System;
using Unity.Entities;
using Unity.Mathematics;

public struct LaserComponent : IComponentData
{
    public float3 aimPoint;
    public float damage;
    public TimeSpan fireInterval;
    public DateTime nextFireTime;
}
