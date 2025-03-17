using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

[DisableAutoCreation]
partial class CreateCompositeCollisionSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;
    PlacedBlockContainer _placedBlockContainer;
    RigidbodyEntityFactory _rigidbodyEntityFactory;
    
    bool _created;

    //these are allocated so must clean after
    List<Entity> _entitisBuilt = new List<Entity>();

    public CreateCompositeCollisionSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer, RigidbodyEntityFactory rigidbodyEntityFactory)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
        _rigidbodyEntityFactory = rigidbodyEntityFactory;

        //todo: support multiple instances
        _created = false;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

#if UNITY_EDITOR
    //later this can be more legit, i.e. have a context shutdown method
    private void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        if (change == PlayModeStateChange.ExitingPlayMode)
        {
            for (int i = 0; i < _entitisBuilt.Count; ++i)
            {
                Entity entity = _entitisBuilt[i];

                var collider = EntityManager.GetComponentData<PhysicsCollider>(entity);

                collider.Value.Dispose();

                EntityManager.DestroyEntity(entity);
            }

            _entitisBuilt.Clear();
        }
    }
#endif

    protected override void OnUpdate()
    {
        //todo: eventually use pure entities but for now it's ok to use collider from game objects
        var blocks = _placedBlockContainer.GetValues();
        if (_created == false && blocks.Count > 0)
        {
            CreateMachineCompositeCollider(blocks, 1);

            _created = true;
        }
    }

    void CreateMachineCompositeCollider(Dictionary<int, PlacedBlockData>.ValueCollection blocks, int machineId)
    {
        Vector3 centreOfMass = Vector3.zero;

        NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance> children = new NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance>(blocks.Count, Allocator.Temp);

        foreach (var block in blocks)
        {
            //todo: remove gameobject
            var gameObject = _blockGameObjectContainer.GetGameObject(block.id);
            var collider = gameObject.GetComponentInChildren<UnityEngine.Collider>();
            BlobAssetReference<Unity.Physics.Collider> boxColliderEcs = default;

            //todo: once blocks are entities, use correct collider type
            if (block.blockCategory == BlockCategory.Wheel)
            {
                UnityEngine.BoxCollider boxCollider = (UnityEngine.BoxCollider)collider;
                boxColliderEcs = Unity.Physics.SphereCollider.Create(new Unity.Physics.SphereGeometry()
                {
                    Center = boxCollider.center,
                    Radius = boxCollider.size.x * collider.transform.lossyScale.x * 0.5f,
                });

                boxColliderEcs.Value.SetFriction(5f);
            }
            else if (collider is UnityEngine.BoxCollider)
            {
                //todo: add primitive shape varieties
                UnityEngine.BoxCollider boxCollider = (UnityEngine.BoxCollider)collider;
                boxColliderEcs = Unity.Physics.BoxCollider.Create(new Unity.Physics.BoxGeometry()
                {
                    Center = boxCollider.center,
                    Size = (float3)boxCollider.size * (float3)collider.transform.lossyScale,
                    Orientation = Unity.Mathematics.quaternion.identity
                });
            }

            children.Add(
                new Unity.Physics.CompoundCollider.ColliderBlobInstance
                {
                    Collider = boxColliderEcs,
                    CompoundFromChild = new Unity.Mathematics.RigidTransform(gameObject.transform.rotation, gameObject.transform.position),
                }
            );

            centreOfMass += gameObject.transform.position;
        }

        var compoundCollider = Unity.Physics.CompoundCollider.Create(children.AsArray());

        foreach (var child in children)
        {
            child.Collider.Dispose();
        }

        children.Dispose();

        centreOfMass /= blocks.Count;

        var filter = compoundCollider.Value.GetCollisionFilter();
        filter.GroupIndex = -machineId;
        compoundCollider.Value.SetCollisionFilter(filter);

        Entity machineEntity = _rigidbodyEntityFactory.CreateRigidbodyEntity(EntityManager, compoundCollider, centreOfMass, "MachineRigidbody");


        //todo: when do we deal with this, which unity identifies as a leak?
        _entitisBuilt.Add(machineEntity);

        //todo: per block? per machine? decide
        EntityManager.AddSharedComponent(machineEntity, new MachineTagComponent() { collisionGroupIndex = machineId });
        EntityManager.AddComponentData(machineEntity, new PlayerInputComponent());
        EntityManager.AddComponentData(machineEntity, new CameraRaycastComponent());

        //try to get dots to do the parenting
        foreach (var (blockId, localTransform, parent) in SystemAPI.Query<BlockIdComponent, RefRW<LocalTransform>, RefRW<Parent>>())
        {
            parent.ValueRW.Value = machineEntity;
        }
    }
}
