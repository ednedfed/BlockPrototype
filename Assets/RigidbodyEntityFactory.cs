﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class RigidbodyEntityFactory
{
    //ensure all components are added that are needed
    public Entity CreateRigidbodyEntity(EntityManager entityManager, BlobAssetReference<Unity.Physics.Collider> collider, Vector3 centreOfMass, FixedString64Bytes entityName,
        float3 position, quaternion rotation)
    {
        // Create a new entity with the combined collider
        Entity rigidbodyEntity = entityManager.CreateEntity();
        entityManager.SetName(rigidbodyEntity, entityName);

        entityManager.AddComponentData(rigidbodyEntity, new PhysicsCollider { Value = collider });
        entityManager.AddComponentData(rigidbodyEntity, new LocalTransform { Position = position, Rotation = rotation, Scale = 1f });
        entityManager.AddComponentData(rigidbodyEntity, new LocalToWorld() { Value = Unity.Mathematics.float4x4.identity });

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
