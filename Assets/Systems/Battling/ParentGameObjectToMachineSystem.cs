using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEditor.Rendering;
using UnityEngine.UIElements;

[DisableAutoCreation]
partial class ParentGameObjectToMachineSystem : SystemBase
{
    BlockGameObjectContainer _blockGameObjectContainer;
    PlacedBlockContainer _placedBlockContainer;

    public ParentGameObjectToMachineSystem(BlockGameObjectContainer blockGameObjectContainer, PlacedBlockContainer placedBlockContainer)
    {
        _blockGameObjectContainer = blockGameObjectContainer;
        _placedBlockContainer = placedBlockContainer;
    }

    protected override void OnUpdate()
    {
        foreach (var (machineTag, machineLocalTransform) in SystemAPI.Query<MachineTagComponent, LocalTransform>())
        {
            var allBlocks = _placedBlockContainer.GetValues();
            foreach (var block in allBlocks)
            {
                //todo: parent block entities, then sync gameobject later on

                //todo: use instancing or dots rendering and this becomes redundant
                var instance = _blockGameObjectContainer.GetGameObject(block.id);
                instance.transform.position = math.mul(machineLocalTransform.Rotation, block.position) + machineLocalTransform.Position;
                instance.transform.rotation = math.mul(machineLocalTransform.Rotation, block.rotation);
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
