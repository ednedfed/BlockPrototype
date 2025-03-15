using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//this is likely to represent one machine
[DisableAutoCreation]
public partial class WheelEntityBuilder : BlockEntityBuilder
{
    public override BlockCategory blockCategory => BlockCategory.Wheel;

    public override void OnBuild(in Entity newEntity, in PlacedBlockData blockData)
    {
        EntityManager.AddComponentData(newEntity, new WheelComponent()
        {
            maxSteerAngle = 30f,
            radius = 0.75f
        });
    }
}
