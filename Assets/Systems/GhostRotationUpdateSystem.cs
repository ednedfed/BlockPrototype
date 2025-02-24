using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostRotationUpdateSystem : SystemBase
{
    GhostBlockData _ghostBlockData;

    public GhostRotationUpdateSystem(GhostBlockData ghostBlockData)
    {
        _ghostBlockData = ghostBlockData;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            _ghostBlockData.direction++;

        if(Input.GetKeyDown(KeyCode.E))
            _ghostBlockData.direction--;

        _ghostBlockData.direction += BlockGameConstants.GhostBlock.NumDirections;
        _ghostBlockData.direction %= BlockGameConstants.GhostBlock.NumDirections;
    }
}
