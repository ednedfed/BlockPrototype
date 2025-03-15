using System.Collections.Generic;
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

    public class PosTracker
    {
        public void Update(float3 position, float deltaTime, Color color)
        {
            _posHistory.Add(position);
            if (_posHistory.Count > _max)
                _posHistory.RemoveAt(0);

            for (int i = 0; i<_posHistory.Count-1; ++i)
            {
                Debug.DrawLine(_posHistory[i], _posHistory[i + 1], color, deltaTime);
            }
        }

        int _max = 1_000;
        List<float3> _posHistory = new List<float3>();
    }
}
