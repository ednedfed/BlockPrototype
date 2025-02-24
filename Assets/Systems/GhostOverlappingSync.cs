using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostOverlappingSync : SystemBase
{
    GameObject _cursor;
    GameObject _ghost;
    HitObject _hitObject;

    public GhostOverlappingSync(GameObject cursor, GameObject ghost, HitObject hitObject)
    {
        _cursor = cursor;
        _ghost = ghost;
        _hitObject = hitObject;
    }

    protected override void OnUpdate()
    {
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
    }
}
