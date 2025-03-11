using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;

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
            foreach (var (machinePhysicsVelocity, machinePhysicsMass, machineTransform, machineTag) in SystemAPI.Query<RefRW<PhysicsVelocity>, PhysicsMass, LocalTransform, MachineTagComponent>())
            {
                foreach (var (wheelComponent, wheelTransform) in SystemAPI.Query<WheelComponent, LocalTransform>())
                {
                    var velocity = float3.zero;
                    var angularVelocity = float3.zero;

                    velocity.z += playerInput.moveVector.y;
                    velocity.x += playerInput.moveVector.x;

                    machinePhysicsVelocity.ValueRW.ApplyImpulse(machinePhysicsMass, machineTransform.Position, machineTransform.Rotation, velocity, wheelTransform.Position);

                    PhysicsComponentExtensions.ApplyAngularImpulse(ref machinePhysicsVelocity.ValueRW, machinePhysicsMass, angularVelocity);
                }
            }
        }
    }
}
