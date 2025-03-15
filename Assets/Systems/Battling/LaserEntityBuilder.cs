using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//this is likely to represent one machine
[DisableAutoCreation]
public partial class LaserEntityBuilder : SystemBase, IBlockFactoryListenerWithCategory
{
    Dictionary<int, Entity> _laserEntities = new Dictionary<int, Entity>();

    public BlockCategory blockCategory => BlockCategory.Laser;

    public void OnAdd(PlacedBlockData blockData)
    {
        if (blockData.blockCategory != blockCategory)
            throw new System.Exception("built wrong block");
            
        UnityEngine.Debug.Log($"built {blockCategory}");

        var newEntity = EntityManager.CreateEntity();

        //todo: machine id component?
        EntityManager.AddComponentData(newEntity, new BlockIdComponent() { blockId = blockData.id });
        EntityManager.AddComponentData(newEntity, new LaserComponent()
        {
        });
        EntityManager.AddComponentData(newEntity, new LocalTransform() { Position = blockData.position, Rotation = blockData.rotation, Scale = 1f });

        _laserEntities.Add(blockData.id, newEntity);
    }

    public void OnRemove(int blockId)
    {
        if (_laserEntities.ContainsKey(blockId))
        {
            EntityManager.DestroyEntity(_laserEntities[blockId]);

            _laserEntities.Remove(blockId);
        }
    }

    public void OnClear()
    {
        foreach (var entity in _laserEntities.Values)
        {
            EntityManager.DestroyEntity(entity);
        }

        _laserEntities.Clear();
    }

    protected override void OnUpdate() { }
}
