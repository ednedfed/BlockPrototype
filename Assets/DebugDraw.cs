using Unity.Mathematics;
using UnityEngine;

public static class DebugDraw
{
    public static void DrawTransformFrame(float3 position, quaternion rotation, float deltaTime)
    {
        UnityEngine.Debug.DrawRay(position, math.mul(rotation, new float3(1f, 0f, 0f)), Color.red, deltaTime);
        UnityEngine.Debug.DrawRay(position, math.mul(rotation, new float3(0f, 1f, 0f)), Color.green, deltaTime);
        UnityEngine.Debug.DrawRay(position, math.mul(rotation, new float3(0f, 0f, 1f)), Color.blue, deltaTime);
    }
}
