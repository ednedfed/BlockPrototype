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
            damage = 0.1f
        });
    }
}
