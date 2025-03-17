using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//common block building
[DisableAutoCreation]
public partial class BlockEntityBuilder : SystemBase, IBlockFactoryListenerWithCategory
{
    private Dictionary<int, Entity> _builtEntities = new Dictionary<int, Entity>();

    public virtual BlockCategory blockCategory => BlockCategory.Primitive;

    public virtual void OnBuild(in Entity newEntity, in PlacedBlockData blockData) { }

    public void OnAdd(PlacedBlockData blockData)
    {
        if (blockData.blockCategory != blockCategory)
            throw new System.Exception("built wrong block");
            
       //UnityEngine.Debug.Log($"built {blockCategory}");

        var newEntity = EntityManager.CreateEntity();
        EntityManager.SetName(newEntity, $"Block_{blockCategory}_{blockData.blockId}");

        EntityManager.AddComponentData(newEntity, new BlockIdComponent() { blockId = blockData.blockId });
        EntityManager.AddComponentData(newEntity, new MachineIdComponent() { machineId = blockData.machineId });

        EntityManager.AddComponentData(newEntity, new LocalTransform() { Position = blockData.position, Rotation = blockData.rotation, Scale = 1f });

        //set up for parenting
        EntityManager.AddComponentData(newEntity, new Parent());
        EntityManager.AddComponentData(newEntity, new LocalToWorld());

        OnBuild(newEntity, blockData);

        _builtEntities.Add(blockData.blockId, newEntity);
    }

    public void OnRemove(int blockId)
    {
        if (_builtEntities.ContainsKey(blockId))
        {
            EntityManager.DestroyEntity(_builtEntities[blockId]);

            _builtEntities.Remove(blockId);
        }
    }

    public void OnClear(int machineId)
    {
        //todo: filter per machine
    }

    protected override void OnUpdate() { }

    public void OnClear()
    {
        foreach (var entity in _builtEntities.Values)
        {
            EntityManager.DestroyEntity(entity);
        }

        _builtEntities.Clear();
    }
}
