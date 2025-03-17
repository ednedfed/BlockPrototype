using System;
using System.Collections.Generic;
using Unity.Entities;

//this is likely to represent one machine
[DisableAutoCreation]
public partial class LaserEntityBuilder : BlockEntityBuilder
{
    public LaserEntityBuilder(Dictionary<int, Entity> entityPerBlockId) : base(entityPerBlockId)
    {
    }

    public override BlockCategory blockCategory => BlockCategory.Laser;

    public override void OnBuild(in Entity newEntity, in PlacedBlockData blockData)
    {
        EntityManager.AddComponentData(newEntity, new LaserComponent()
        { 
            damage = 0.5f,
            fireInterval = TimeSpan.FromSeconds(1.0),
            nextFireTime = DateTime.UtcNow
        });
    }
}
