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

    DebugDraw.PosTracker _posTracker = new DebugDraw.PosTracker();

    protected override void OnUpdate()
    {
        foreach (var (machineTag, machineLocalTransform) in SystemAPI.Query<MachineTagComponent, LocalTransform>())
        {
            bool debug = false;

            //add a child offset, currently wheels, but might become generic
            foreach (var (blockWorld, blockIdComponent) in SystemAPI.Query<LocalToWorld, BlockIdComponent>())
            {
                var blockGameObject = _blockGameObjectContainer.GetGameObject(blockIdComponent.blockId);
                blockGameObject.transform.position = blockWorld.Position;
                blockGameObject.transform.rotation = blockWorld.Rotation;

                if (debug == false)
                {
                    _posTracker.Update(blockGameObject.transform.position, SystemAPI.Time.DeltaTime, UnityEngine.Color.white);
                    debug = true;
                }
            }

            //add a child offset, currently wheels, but might become generic
            foreach (var (wheelComponent, blockLocal, blockIdComponent) in SystemAPI.Query<WheelComponent, LocalTransform, BlockIdComponent>())
            {
                //local offset here
                var upLocal = math.mul(math.inverse(blockLocal.Rotation), machineLocalTransform.Up());
                var additionalRotationWorldspace = quaternion.AxisAngle(upLocal, math.radians(wheelComponent.currentSteerAngle));

                var instance = _blockGameObjectContainer.GetGameObject(blockIdComponent.blockId);
                instance.transform.rotation *= additionalRotationWorldspace;

                var placeholderTyreRot = quaternion.AxisAngle(math.up(), wheelComponent.axisRotation);
                instance.transform.rotation *= placeholderTyreRot;
            }

            //add a child offset, currently wheels, but might become generic
            foreach (var (laserComponent, blockLocal, blockIdComponent) in SystemAPI.Query<LaserComponent, RefRW<LocalTransform>, BlockIdComponent>())
            {
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
