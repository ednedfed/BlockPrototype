using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostRotationUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var ghostBlockData in SystemAPI.Query<RefRW<GhostBlockDataComponent>>())
        {
            if (Input.GetKeyDown(KeyCode.Q))
                ghostBlockData.ValueRW.direction++;

            if (Input.GetKeyDown(KeyCode.E))
                ghostBlockData.ValueRW.direction--;

            ghostBlockData.ValueRW.direction += BlockGameConstants.GhostBlock.NumDirections;
            ghostBlockData.ValueRW.direction %= BlockGameConstants.GhostBlock.NumDirections;
        }
    }
}
