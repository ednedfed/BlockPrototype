using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[DisableAutoCreation]
[UpdateAfter(typeof(ParentGameObjectToMachineSystem))]
partial class LaserShootingSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;

    public LaserShootingSystem(BlockGameObjectContainer blockGameObjectContainer)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
    }

    protected override void OnUpdate()
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(EntityManager.WorldUnmanaged);

        //store this on a camera entity? decide how to map weapons to machine, can shared component be used?
        foreach (var (playerInput, machineTag) in SystemAPI.Query<PlayerInputComponent, MachineTagComponent>())
        {
            foreach (var (laserComponent, blockWorld) in SystemAPI.Query<LaserComponent, LocalToWorld>()
                .WithSharedComponentFilterManaged(new MachineIdComponent { machineId = machineTag.machineId }))
            {
                if (playerInput.fire == true)
                {
                    var collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

                    RaycastInput input = new RaycastInput()
                    {
                        Start = blockWorld.Position,
                        End = laserComponent.aimPoint,
                        Filter = new CollisionFilter()
                        {
                            BelongsTo = ~0u,
                            CollidesWith = ~0u,
                            GroupIndex = machineTag.collisionGroupIndex
                        }
                    };

                    bool haveHit = collisionWorld.CastRay(input, out var hit);
                    if (haveHit)
                    {
                        UnityEngine.Debug.DrawLine(hit.Position, blockWorld.Position, UnityEngine.Color.yellow, SystemAPI.Time.DeltaTime);

                        var machineEntity = hit.Entity;
                        var colliderKey = hit.ColliderKey;

                        var physicsCollider = EntityManager.GetComponentData<PhysicsCollider>(hit.Entity);
                        physicsCollider.Value.Value.GetChild(ref colliderKey, out var child);

                        //get child to get block
                        var hitBlockEntity = child.Entity;

                        if (hitBlockEntity != Entity.Null && EntityManager.HasComponent<MachineTagComponent>(machineEntity))
                        {
                            DeparentBlock(hitBlockEntity, EntityManager, ecb);

                            //todo: now we have to deactivate the child collider
                            //however this is more complex as the body can be split
                            //physicsCollider.Value.Value.SetCollisionFilter(CollisionFilter.Zero, colliderKey);
                        }
                    }
                }
            }
        }
    }

    void DeparentBlock(Entity hitBlockEntity, EntityManager entityManager, EntityCommandBuffer entityCommandBuffer)
    {
        //cause damage, do vfx
        var blockId = entityManager.GetComponentData<BlockIdComponent>(hitBlockEntity);

        //when would this happen? unity's dbugging is so unstable it's been hard to tell
        if (entityManager.HasComponent<Parent>(hitBlockEntity) == false)
            return;

        //deparent, possibly make a new RB?
        var blockParent = entityManager.GetComponentData<Parent>(hitBlockEntity);
        blockParent.Value = Entity.Null;
        entityManager.SetComponentData(hitBlockEntity, blockParent);

        var blockGameObject = _blockGameObjectContainer.GetGameObject(blockId.blockId);
        var collider = blockGameObject.GetComponentInChildren<UnityEngine.Collider>();

        UnityEngine.BoxCollider boxCollider = (UnityEngine.BoxCollider)collider;
        entityCommandBuffer.AddComponent(hitBlockEntity, new PhysicsCollider
        {
            Value = Unity.Physics.BoxCollider.Create(new BoxGeometry()
            {
                Center = boxCollider.center,
                Size = (float3)boxCollider.size * (float3)collider.transform.lossyScale,
                Orientation = quaternion.identity
            })
        });

        // 3. PhysicsMass - Defines mass and inertia (automatic calculation for dynamic bodies)
        var physicsMass = PhysicsMass.CreateDynamic(MassProperties.CreateBox(boxCollider.bounds.size), 1f);
        physicsMass.CenterOfMass = boxCollider.center;
        entityCommandBuffer.AddComponent(hitBlockEntity, physicsMass);

        var random = UnityEngine.Random.insideUnitCircle;

        entityCommandBuffer.AddComponent(hitBlockEntity, new PhysicsVelocity
        {
            Linear = new float3(random.x, 10f, random.y),
            Angular = UnityEngine.Random.insideUnitSphere
        });

        //not final but allows it to visually work
        entityCommandBuffer.AddComponent(hitBlockEntity, new LocalTransform
        {
            Position = blockGameObject.transform.position,
            Rotation = blockGameObject.transform.rotation,
        });

        entityCommandBuffer.AddComponent(hitBlockEntity, new PhysicsDamping
        {
            Linear = 0.01f,
            Angular = 0.05f
        });

        entityCommandBuffer.AddSharedComponent(hitBlockEntity, new PhysicsWorldIndex());
    }
}
