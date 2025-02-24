using Unity.Entities;
using Unity.Transforms;

[DisableAutoCreation]
partial class GhostOverlappingSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //todo: dots rendering must get implemented
        foreach (var (ghostTags, hitObject, ghostLocalTransforms) in SystemAPI.Query<GhostTagComponent, HitObjectComponent, LocalTransform>())
        {
            /*
            var ghostRenderer = _ghost.GetComponentInChildren<Renderer>();
            if (ghostRenderer != null)
            {
                if (_hitObject.isOverlapping)
                {
                    ghostRenderer.material.color = BlockGameConstants.GhostBlock.InvalidGhostColor;
                }
                else
                {
                    ghostRenderer.material.color = BlockGameConstants.GhostBlock.ValidGhostColor;
                }
            }
            */
        }
        /*
        var cursorRenderer = _cursor.GetComponentInChildren<Renderer>();
        if (cursorRenderer != null)
        {
            if (_hitObject.isOverlapping)
            {
                cursorRenderer.material.color = BlockGameConstants.GhostBlock.InvalidCursorColor;
            }
            else
            {
                cursorRenderer.material.color = BlockGameConstants.GhostBlock.ValidCursorColor;
            }
        }
        */
    }
}
