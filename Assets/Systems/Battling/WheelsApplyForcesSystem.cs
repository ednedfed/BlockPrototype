using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

[DisableAutoCreation]
[UpdateAfter(typeof(MachineControllerSystem))]
partial class WheelsApplyForcesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = SystemAPI.Time.DeltaTime;

        //todo: get wheels and apply force
        foreach (var playerInput in SystemAPI.Query<PlayerInputComponent>())
        {
            foreach (var (velocityComponent, physicsMass, machineTag) in SystemAPI.Query<RefRW<PhysicsVelocity>, PhysicsMass, MachineTagComponent>())
            {
                var velocity = float3.zero;
                velocity.z += playerInput.moveVector.y;

                var angularVelocity = float3.zero;
                angularVelocity.y += playerInput.moveVector.x;

                PhysicsComponentExtensions.ApplyLinearImpulse(ref velocityComponent.ValueRW, physicsMass, velocity);

                PhysicsComponentExtensions.ApplyAngularImpulse(ref velocityComponent.ValueRW, physicsMass, angularVelocity);
            }
        }
    }
}
