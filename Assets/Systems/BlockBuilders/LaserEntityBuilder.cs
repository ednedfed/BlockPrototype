using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//this is likely to represent one machine
[DisableAutoCreation]
public partial class LaserEntityBuilder : BlockEntityBuilder
{
    Dictionary<int, Entity> _laserEntities = new Dictionary<int, Entity>();

    public override BlockCategory blockCategory => BlockCategory.Laser;

    public override void OnBuild(in Entity newEntity, in PlacedBlockData blockData)
    {
        EntityManager.AddComponentData(newEntity, new LaserComponent());
    }
}
