using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
partial class PlaceBlockSystem : SystemBase
{
    BlockFactory _blockFactory;

    public PlaceBlockSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        foreach (var hitObject in SystemAPI.Query<HitObjectComponent>())
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && hitObject.isOverlapping == false)
            {
                foreach (var (ghostBlockData, localTranforms) in SystemAPI.Query<GhostBlockDataComponent, LocalTransform>())
                {
                    _blockFactory.InstantiateBlock(ghostBlockData.blockType, localTranforms.Position, localTranforms.Rotation);
                }
            }
        }
    }
}
