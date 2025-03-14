using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//this is likely to represent one machine
[DisableAutoCreation]
public partial class WheelEntityBuilder : SystemBase, IBlockFactoryListenerWithCategory
{
    Dictionary<int, Entity> _wheelEntities = new Dictionary<int, Entity>();

    public BlockCategory blockCategory => BlockCategory.Wheel;

    public void OnAdd(PlacedBlockData blockData)
    {
        //todo: make a builder per type
        if (blockData.blockCategory != BlockCategory.Wheel)
            throw new System.Exception("built wrong block");
            
        UnityEngine.Debug.Log("built wheel");

        var newEntity = EntityManager.CreateEntity();

        //todo: machine id component?
        EntityManager.AddComponentData(newEntity, new BlockIdComponent() { blockId = blockData.id });
        EntityManager.AddComponentData(newEntity, new WheelComponent()
        {
            maxSteerAngle = 30f,
        });
        EntityManager.AddComponentData(newEntity, new LocalTransform() { Position = blockData.position, Rotation = blockData.rotation, Scale = 1f });

        _wheelEntities.Add(blockData.id, newEntity);
    }

    public void OnRemove(int blockId)
    {
        if (_wheelEntities.ContainsKey(blockId))
        {
            EntityManager.DestroyEntity(_wheelEntities[blockId]);

            _wheelEntities.Remove(blockId);
        }
    }

    public void OnClear()
    {
        foreach (var entity in _wheelEntities.Values)
        {
            EntityManager.DestroyEntity(entity);
        }

        _wheelEntities.Clear();
    }

    protected override void OnUpdate() { }
}
