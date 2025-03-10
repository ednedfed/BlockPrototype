using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine;
using Unity.Physics.Authoring;

[DisableAutoCreation]
partial class CreateCompositeCollisionSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;
    PlacedBlockContainer _placedBlockContainer;

    bool _created;

    public CreateCompositeCollisionSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
        _created = false;
    }

    protected override void OnUpdate()
    {
        var blocks = _placedBlockContainer.GetVales();
        if (_created == false && blocks.Count > 0)
        {
            NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance> children = new NativeList<Unity.Physics.CompoundCollider.ColliderBlobInstance>(blocks.Count, Allocator.Temp);

            foreach (var block in blocks)
            {
                var gameObject = _blockGameObjectContainer.GetGameObject(block.id);
                var collider = gameObject.GetComponentInChildren<UnityEngine.Collider>();

                if (collider is UnityEngine.BoxCollider)
                {
                    UnityEngine.BoxCollider boxCollider = (UnityEngine.BoxCollider)collider;

                    var boxColliderEcs = Unity.Physics.BoxCollider.Create(new Unity.Physics.BoxGeometry()
                    {
                        Center = boxCollider.center,
                        Size = boxCollider.size,
                        Orientation = Unity.Mathematics.quaternion.identity
                    });

                    children.Add(
                        new Unity.Physics.CompoundCollider.ColliderBlobInstance
                        {
                            Collider = boxColliderEcs,
                            CompoundFromChild = new Unity.Mathematics.RigidTransform(gameObject.transform.rotation, gameObject.transform.position)
                        }
                    );
                }
            }

            var compoundCollider = Unity.Physics.CompoundCollider.Create(children.AsArray());

            foreach (var child in children)
            {
                child.Collider.Dispose();
            }

            children.Dispose();

            CreateRigidbodyEntity(EntityManager, compoundCollider);

            _created = true;
        }
    }

    //ensure all components are added that are needed
    static void CreateRigidbodyEntity(EntityManager entityManager, BlobAssetReference<Unity.Physics.Collider> collider)
    {
        // Create a new entity with the combined collider
        Entity combinedEntity = entityManager.CreateEntity();
        entityManager.SetName(combinedEntity, "MachineRigidbody");

        entityManager.AddComponentData(combinedEntity, new PhysicsCollider { Value = collider });
        entityManager.AddComponentData(combinedEntity, new PhysicsMass { InverseMass = 1.0f });
        entityManager.AddComponentData(combinedEntity, new LocalTransform { Position = float3.zero, Rotation = quaternion.identity, Scale = 1 });

        // 3. PhysicsMass - Defines mass and inertia (automatic calculation for dynamic bodies)
        entityManager.AddComponentData(combinedEntity, PhysicsMass.CreateDynamic(MassProperties.UnitSphere, 1f));

        // 4. PhysicsVelocity - Controls motion (set to zero initially)
        entityManager.AddComponentData(combinedEntity, new PhysicsVelocity
        {
            Linear = float3.zero,
            Angular = float3.zero
        });

        // 5. PhysicsDamping - Controls friction-like behavior (optional but recommended)
        entityManager.AddComponentData(combinedEntity, new PhysicsDamping
        {
            Linear = 0.01f,   // Slows down linear motion
            Angular = 0.05f   // Slows down rotational motion
        });

        entityManager.AddSharedComponent(combinedEntity, new PhysicsWorldIndex());
    }
}
