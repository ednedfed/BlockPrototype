using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor;
using UnityEngine;

[DisableAutoCreation]
partial class CreateCompositeCollisionSystem : SystemBase, IDisposable
{
    BlockGameObjectContainer _blockGameObjectContainer;
    PlacedBlockContainer _placedBlockContainer;
    RigidbodyEntityFactory _rigidbodyEntityFactory;
    
    bool _created;

    //these are allocated so must clean after
    List<BlobAssetReference<Unity.Physics.Collider>> _collidersToDestroy = new List<BlobAssetReference<Unity.Physics.Collider>>();

    public CreateCompositeCollisionSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer, RigidbodyEntityFactory rigidbodyEntityFactory)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
        _rigidbodyEntityFactory = rigidbodyEntityFactory;

        _created = false;
    }

    protected override void OnUpdate()
    {
        //todo: eventually use pure entities
        var blocks = _placedBlockContainer.GetValues();
        if (_created == false && blocks.Count > 0)
        {
            CreateMachineCompositeCollider(blocks);

            _created = true;
        }
    }

    void CreateMachineCompositeCollider(Dictionary<int, PlacedBlockData>.ValueCollection blocks)
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

        //todo: when do we deal with this, which unity identifies as a leak?
        _collidersToDestroy.Add(compoundCollider);

        foreach (var child in children)
        {
            child.Collider.Dispose();
        }

        children.Dispose();

        centreOfMass /= blocks.Count;

        Entity machineEntity = _rigidbodyEntityFactory.CreateRigidbodyEntity(EntityManager, compoundCollider, centreOfMass, "MachineRigidbody");

        //todo: per block? per machine? decide
        EntityManager.AddSharedComponent(machineEntity, new MachineTagComponent());
        EntityManager.AddComponentData(machineEntity, new PlayerInputComponent());
        EntityManager.AddComponentData(machineEntity, new CameraRaycastComponent());

        //try to get dots to do the parenting
        foreach (var (blockId, localTransform, parent) in SystemAPI.Query<BlockIdComponent, RefRW<LocalTransform>, RefRW<Parent>>())
        {
            parent.ValueRW.Value = machineEntity;
        }
    }
}
