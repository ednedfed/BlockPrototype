using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
[UpdateAfter(typeof(GhostPositionUpdateSystem))]
partial class ParentCursorToGhostSystem : SystemBase
{
    GameObject _ghost;

    public ParentCursorToGhostSystem(GameObject ghost)
    {
        _ghost = ghost;
    }

    protected override void OnUpdate()
    {
        foreach (var (cursorTags, localTranforms) in SystemAPI.Query<CursorTagComponent, RefRW<LocalTransform>>())
        {
            //cursor is parented to ghost
            localTranforms.ValueRW.Position = _ghost.transform.position;
            localTranforms.ValueRW.Rotation = _ghost.transform.rotation;
        }
    }
}
