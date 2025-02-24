using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class GhostOverlappingSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //todo: dots rendering must get implemented
        foreach (var (ghostBlockPrefab, hitObject) in SystemAPI.Query<GhostBlockPrefabComponent, HitObjectComponent>())
        {
            if (ghostBlockPrefab.ghostBlockPrefab != null)
            {
                var ghostRenderer = ghostBlockPrefab.ghostBlockPrefab.GetComponentInChildren<Renderer>();
                if (ghostRenderer != null)
                {
                    if (hitObject.isOverlapping)
                    {
                        ghostRenderer.material.color = BlockGameConstants.GhostBlock.InvalidGhostColor;
                    }
                    else
                    {
                        ghostRenderer.material.color = BlockGameConstants.GhostBlock.ValidGhostColor;
                    }
                }
            }
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
