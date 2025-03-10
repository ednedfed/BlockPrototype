using Unity.Entities;
using Unity.Transforms;

//todo: use linked entity for this?
[DisableAutoCreation]
[UpdateAfter(typeof(GhostPositionUpdateSystem))]
partial class ParentCursorToGhostSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (cursorTags, cursorLocalTranforms) in SystemAPI.Query<CursorTagComponent, RefRW<LocalTransform>>())
        {
            foreach (var (ghostTags, ghostLocalTransforms) in SystemAPI.Query<GhostTagComponent, LocalTransform>())
            {
                //cursor is parented to ghost
                cursorLocalTranforms.ValueRW.Position = ghostLocalTransforms.Position;
                cursorLocalTranforms.ValueRW.Rotation = ghostLocalTransforms.Rotation;
            }
        }
    }
}
