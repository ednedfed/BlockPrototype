using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

//todo: use linked entity for this?
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
                //todo: use instancing or dots rendering and this becomes redundant
                var instance = _blockGameObjectContainer.GetGameObject(block.id);
                instance.transform.position = math.mul(machineLocalTransform.Rotation, block.position) + machineLocalTransform.Position;
                instance.transform.rotation = math.mul(machineLocalTransform.Rotation, block.rotation);
            }
        }
    }
}
