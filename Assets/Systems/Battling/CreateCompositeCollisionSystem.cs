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
    SpawnPointComponent[] _spawnPoints;

    //these are allocated so must clean after
    List<Entity> _entitiesBuilt = new List<Entity>();

    public CreateCompositeCollisionSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer,
        RigidbodyEntityFactory rigidbodyEntityFactory, SpawnPointComponent[] spawnPoints)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
        _rigidbodyEntityFactory = rigidbodyEntityFactory;

        _spawnPoints = spawnPoints;

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
            for (int i = 0; i < _entitiesBuilt.Count; ++i)
            {
                Entity entity = _entitiesBuilt[i];

                var collider = EntityManager.GetComponentData<PhysicsCollider>(entity);

                collider.Value.Dispose();

                EntityManager.DestroyEntity(entity);
            }

            _entitiesBuilt.Clear();
        }
    }
#endif

    protected override void OnUpdate()
    {
        for (int i = 0; i < _spawnPoints.Length; ++i)
        {
            var spawnPoint = _spawnPoints[i];

            if(spawnPoint.isLoaded == false)
                continue;

            //todo: eventually use pure entities but for now it's ok to use collider from game objects
            var blocks = _placedBlockContainer.GetValues(i);
            if (blocks.Count > 0 && spawnPoint.isSpawned == false)
            {
                CreateMachine(blocks, i, spawnPoint.transform.position, spawnPoint.transform.rotation);

                //even if we don't store it only in the spawn point we at least don't want use the spawn point for more than one player
                spawnPoint.isSpawned = true;
            }
        }
    }

    void CreateMachine(Dictionary<int, PlacedBlockData>.ValueCollection blocks, int machineIndex, float3 spawnPosition, quaternion spawnRotation)
    {
        Vector3 centreOfMass = Vector3.zero;

        NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance> childColliders = new NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance>(blocks.Count, Allocator.Temp);

        foreach (var block in blocks)
        {
            //todo: remove gameobject
            var blockGameObject = _blockGameObjectContainer.GetGameObject(block.blockId);

            //set active when ready
            blockGameObject.SetActive(true);

            var collider = blockGameObject.GetComponentInChildren<UnityEngine.Collider>();
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

            childColliders.Add(
                new Unity.Physics.CompoundCollider.ColliderBlobInstance
                {
                    Collider = boxColliderEcs,
                    CompoundFromChild = new Unity.Mathematics.RigidTransform(blockGameObject.transform.rotation, blockGameObject.transform.position),
                }
            );

            centreOfMass += blockGameObject.transform.position;
        }

        var compoundCollider = Unity.Physics.CompoundCollider.Create(childColliders.AsArray());

        foreach (var child in childColliders)
        {
            child.Collider.Dispose();
        }

        childColliders.Dispose();

        centreOfMass /= blocks.Count;

        var collisionGroup = -(machineIndex + 1);

        var filter = compoundCollider.Value.GetCollisionFilter();
        filter.GroupIndex = collisionGroup;
        compoundCollider.Value.SetCollisionFilter(filter);

        Entity machineEntity = _rigidbodyEntityFactory.CreateRigidbodyEntity(
            EntityManager, compoundCollider, centreOfMass, $"MachineRigidbody_{machineIndex}",
            spawnPosition, spawnRotation);

        

        //todo: when do we deal with this, which unity identifies as a leak?
        _entitiesBuilt.Add(machineEntity);

        EntityManager.AddSharedComponent(machineEntity, new MachineTagComponent() { collisionGroupIndex = collisionGroup, machineId = machineIndex });
        EntityManager.AddComponentData(machineEntity, new PlayerInputComponent());
        EntityManager.AddComponentData(machineEntity, new CameraRaycastComponent());

        //todo: use shared component
        foreach (var (blockId, machineId, localTransform, parent) in SystemAPI.Query<BlockIdComponent, MachineIdComponent, RefRW<LocalTransform>, RefRW<Parent>>())
        {
            if (machineId.machineId == machineIndex)
            {
                parent.ValueRW.Value = machineEntity;
            }
        }
    }
}
