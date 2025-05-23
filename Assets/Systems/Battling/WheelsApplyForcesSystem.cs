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

        foreach (var (machinePhysicsVelocity, machinePhysicsMass, machineTransform, machineTag, playerInput) in SystemAPI.Query<RefRW<PhysicsVelocity>, PhysicsMass, LocalTransform, MachineTagComponent, PlayerInputComponent>())
        {
            foreach (var (wheelComponent, wheelTransform) in SystemAPI.Query<RefRW<WheelComponent>, LocalTransform>()
                .WithSharedComponentFilter(new MachineIdComponent { machineId = machineTag.machineId }))
            {
                var velocity = float3.zero;
                var angularVelocity = float3.zero;

                //forward back
                velocity.z += playerInput.moveVector.y * deltaTime * 10f;

                //un-steering when input not pressed
                if (math.abs(playerInput.moveVector.x) < 0.001f)
                    wheelComponent.ValueRW.currentSteerAngle *= 0.7f;

                //left right, todo: work out how to move to on add
                bool isInfront = wheelTransform.Position.z >= machinePhysicsMass.CenterOfMass.z;

                if(isInfront)
                    wheelComponent.ValueRW.currentSteerAngle += playerInput.moveVector.x * deltaTime * 200f;
                else
                    wheelComponent.ValueRW.currentSteerAngle -= playerInput.moveVector.x * deltaTime * 200f;

                wheelComponent.ValueRW.currentSteerAngle = math.clamp(wheelComponent.ValueRW.currentSteerAngle, -wheelComponent.ValueRW.maxSteerAngle, wheelComponent.ValueRW.maxSteerAngle);

                var wheelWorldRotation = math.mul(machineTransform.Rotation, wheelTransform.Rotation);
                var wheelWorldPosition = math.mul(machineTransform.Rotation, wheelTransform.Position) + machineTransform.Position;

                velocity = math.mul(wheelWorldRotation, velocity);

                quaternion steerRotation = quaternion.AxisAngle(machineTransform.Up(), math.radians(wheelComponent.ValueRO.currentSteerAngle));

                velocity = math.mul(steerRotation, velocity);

                var wheelForward = math.cross(machineTransform.Up(), math.mul(wheelWorldRotation, math.up()));

                var invRadius = 1f / wheelComponent.ValueRW.radius;
                wheelComponent.ValueRW.axisRotation -= math.dot(machinePhysicsVelocity.ValueRO.Linear, wheelForward) * invRadius * deltaTime;

                //todo: friction etx

                //note: this point is worldspace
                machinePhysicsVelocity.ValueRW.ApplyImpulse(machinePhysicsMass, machineTransform.Position, machineTransform.Rotation, velocity, wheelWorldPosition);

                UnityEngine.Debug.DrawRay(wheelWorldPosition, velocity, UnityEngine.Color.yellow, deltaTime);

                DebugDraw.DrawTransformFrame(wheelWorldPosition, wheelWorldRotation, deltaTime);
            }
        }
    }
}
