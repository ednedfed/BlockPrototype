using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;

//common block building
[DisableAutoCreation]
public partial class BlockEntityBuilder : SystemBase, IBlockFactoryListenerWithCategory
{
    Dictionary<int, Entity> _entityPerBlockId = new Dictionary<int, Entity>();

    public virtual BlockCategory blockCategory => BlockCategory.Primitive;

    //note: eventually no game objects
    public BlockEntityBuilder(Dictionary<int, Entity> entityPerBlockId)
    {
        _entityPerBlockId = entityPerBlockId;
    }

    public virtual void OnBuild(in Entity newEntity, in PlacedBlockData blockData) { }

    public void OnAdd(PlacedBlockData blockData)
    {
        if (blockData.blockCategory != blockCategory)
            throw new System.Exception("built wrong block");
            
       //UnityEngine.Debug.Log($"built {blockCategory}");

        var newEntity = EntityManager.CreateEntity();
        EntityManager.SetName(newEntity, $"Block_{blockCategory}_{blockData.blockId}");

        EntityManager.AddComponentData(newEntity, new BlockIdComponent() { blockId = blockData.blockId });
        EntityManager.AddComponentData(newEntity, new LocalTransform() { Position = blockData.position, Rotation = blockData.rotation, Scale = 1f });

        //set up for parenting
        EntityManager.AddComponentData(newEntity, new Parent());
        EntityManager.AddComponentData(newEntity, new LocalToWorld() { Value = Unity.Mathematics.float4x4.identity});

        //chunk per machine for filtering
        EntityManager.AddSharedComponentManaged(newEntity, new MachineIdComponent() { machineId = blockData.machineId });

        OnBuild(newEntity, blockData);

        _entityPerBlockId.Add(blockData.blockId, newEntity);
    }

    public void OnRemove(int blockId)
    {
        if (_entityPerBlockId.ContainsKey(blockId))
        {
            EntityManager.DestroyEntity(_entityPerBlockId[blockId]);

            _entityPerBlockId.Remove(blockId);
        }
    }

    public void OnClear(int machineId)
    {
        //todo: filter per machine
    }

    protected override void OnUpdate() { }

    public void OnClear()
    {
        foreach (var entity in _entityPerBlockId.Values)
        {
            EntityManager.DestroyEntity(entity);
        }

        _entityPerBlockId.Clear();
    }
}
