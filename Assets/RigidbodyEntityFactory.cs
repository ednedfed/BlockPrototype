using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

public class RigidbodyEntityFactory
{
    //ensure all components are added that are needed
    public Entity CreateRigidbodyEntity(EntityManager entityManager, BlobAssetReference<Unity.Physics.Collider> collider, Vector3 centreOfMass, FixedString64Bytes entityName)
    {
        // Create a new entity with the combined collider
        Entity rigidbodyEntity = entityManager.CreateEntity();
        entityManager.SetName(rigidbodyEntity, entityName);

        entityManager.AddComponentData(rigidbodyEntity, new PhysicsCollider { Value = collider });
        entityManager.AddComponentData(rigidbodyEntity, new LocalTransform { Position = float3.zero, Rotation = quaternion.identity, Scale = 1f });
        entityManager.AddComponentData(rigidbodyEntity, new LocalToWorld() );

        // 3. PhysicsMass - Defines mass and inertia (automatic calculation for dynamic bodies)
        var physicsMass = PhysicsMass.CreateDynamic(MassProperties.CreateBox(collider.Value.CalculateAabb().Extents), 1f);
        physicsMass.CenterOfMass = centreOfMass;
        entityManager.AddComponentData(rigidbodyEntity, physicsMass);

        // 4. PhysicsVelocity - Controls motion (set to zero initially)
        entityManager.AddComponentData(rigidbodyEntity, new PhysicsVelocity
        {
            Linear = float3.zero,
            Angular = float3.zero
        });

        // 5. PhysicsDamping - Controls friction-like behavior (optional but recommended)
        entityManager.AddComponentData(rigidbodyEntity, new PhysicsDamping
        {
            Linear = 0.01f,   // Slows down linear motion
            Angular = 0.05f   // Slows down rotational motion
        });

        entityManager.AddSharedComponent(rigidbodyEntity, new PhysicsWorldIndex());

        return rigidbodyEntity;
    }
}
