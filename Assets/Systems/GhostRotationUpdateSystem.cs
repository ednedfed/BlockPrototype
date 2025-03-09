using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostRotationUpdateSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var ghostBlockData in SystemAPI.Query<RefRW<GhostBlockDataComponent>>())
        {
            //todo: split input
            if (Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y > 0)
                ghostBlockData.ValueRW.direction++;

            if (Input.GetKeyDown(KeyCode.E) || Input.mouseScrollDelta.y < 0)
                ghostBlockData.ValueRW.direction--;

            ghostBlockData.ValueRW.direction += BlockGameConstants.GhostBlock.NumDirections;
            ghostBlockData.ValueRW.direction %= BlockGameConstants.GhostBlock.NumDirections;
        }
    }
}
