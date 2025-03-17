using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//do this after parent group or it can be out of sync, e.g frame behind?
[DisableAutoCreation]
[UpdateAfter(typeof(TransformSystemGroup))]
partial class ParentGameObjectToMachineSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;

    public ParentGameObjectToMachineSystem(BlockGameObjectContainer blockGameObjectContainer)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
    }

    protected override void OnUpdate()
    {
        //blocks are usually machine agnostic
        foreach (var (blockWorld, blockIdComponent) in SystemAPI.Query<LocalToWorld, BlockIdComponent>())
        {
            _blockGameObjectContainer.SetPositionAndRotation(blockIdComponent.blockId, blockWorld.Position, blockWorld.Rotation);
        }

        //specialised offsets are machine-specific (currently)
        foreach (var (machineTag, machineLocalTransform) in SystemAPI.Query<MachineTagComponent, LocalTransform>())
        {
            //add a child offset, currently wheels, but might become generic
            foreach (var (wheelComponent, blockLocal, blockIdComponent, machineIdComponent) in SystemAPI.Query<WheelComponent, LocalTransform, BlockIdComponent, MachineIdComponent>())
            {
                if (machineIdComponent.machineId != machineTag.machineId)
                    continue;

                //local offset here
                var upLocal = math.mul(math.inverse(blockLocal.Rotation), machineLocalTransform.Up());
                var additionalRotationWorldspace = quaternion.AxisAngle(upLocal, math.radians(wheelComponent.currentSteerAngle));

                var instance = _blockGameObjectContainer.GetGameObject(blockIdComponent.blockId);
                instance.transform.rotation *= additionalRotationWorldspace;

                var placeholderTyreRot = quaternion.AxisAngle(math.up(), wheelComponent.axisRotation);
                instance.transform.rotation *= placeholderTyreRot;
            }

            //add a child offset, currently wheels, but might become generic
            foreach (var (laserComponent, blockLocal, blockIdComponent, machineIdCompnent) in SystemAPI.Query<LaserComponent, RefRW<LocalTransform>, BlockIdComponent, MachineIdComponent>())
            {
                if (machineIdCompnent.machineId != machineTag.machineId)
                    continue;

                var blockWorldPos = math.mul(machineLocalTransform.Rotation, blockLocal.ValueRO.Position) + machineLocalTransform.Position;

                //local offset here
                var toTarget = laserComponent.aimPoint - blockWorldPos;
                var upLocal = machineLocalTransform.Up();
                var newRight = math.cross(toTarget, upLocal);
                var newUp = math.cross(newRight, toTarget);

                var rotationWorldSpace = quaternion.LookRotation(toTarget, newUp);

                UnityEngine.Debug.DrawLine(laserComponent.aimPoint, blockWorldPos, UnityEngine.Color.magenta, SystemAPI.Time.DeltaTime);

                var instance = _blockGameObjectContainer.GetGameObject(blockIdComponent.blockId);
                instance.transform.rotation = rotationWorldSpace;
            }
        }
    }
}
