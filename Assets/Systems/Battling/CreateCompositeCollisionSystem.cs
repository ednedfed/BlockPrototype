using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

[DisableAutoCreation]
partial class CreateCompositeCollisionSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;
    PlacedBlockContainer _placedBlockContainer;
    RigidbodyEntityFactory _rigidbodyEntityFactory;

    bool _created;

    public CreateCompositeCollisionSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer, RigidbodyEntityFactory rigidbodyEntityFactory)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
        _rigidbodyEntityFactory = rigidbodyEntityFactory;

        _created = false;
    }

    protected override void OnUpdate()
    {
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
            var gameObject = _blockGameObjectContainer.GetGameObject(block.id);
            var collider = gameObject.GetComponentInChildren<UnityEngine.Collider>();
            BlobAssetReference<Unity.Physics.Collider> boxColliderEcs = default;

            //todo: once blocks are entities, use correct collider type
            if (block.blockType == 2)
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

        Entity entity = _rigidbodyEntityFactory.CreateRigidbodyEntity(EntityManager, compoundCollider, centreOfMass, "MachineRigidbody");

        EntityManager.AddSharedComponent(entity, new MachineTagComponent());

        //todo: per block?
        EntityManager.AddComponentData(entity, new PlayerInputComponent());
    }
}
