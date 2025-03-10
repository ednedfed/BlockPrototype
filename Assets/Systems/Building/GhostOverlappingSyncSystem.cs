using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

[DisableAutoCreation]
partial class GhostOverlappingSyncSystem : SystemBase
{
    protected override void OnUpdate()
    {
        //todo: dots rendering must get implemented
        foreach (var (ghostBlockPrefab, hitObject) in SystemAPI.Query<GhostBlockPrefabComponent, HitObjectComponent>())
        {
            if (ghostBlockPrefab.gameObject != null)
            {
                var ghostRenderer = ghostBlockPrefab.gameObject.GetComponentInChildren<Renderer>();
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

        foreach (var hitObject in SystemAPI.Query<HitObjectComponent>())
        {
            foreach (var (_, baseColor) in SystemAPI.Query<CursorMeshTagComponent, RefRW<URPMaterialPropertyBaseColor>>())
            {
                if (hitObject.isOverlapping)
                {
                    baseColor.ValueRW.Value = (Vector4)BlockGameConstants.GhostBlock.InvalidCursorColor;
                }
                else
                {
                    baseColor.ValueRW.Value = (Vector4)BlockGameConstants.GhostBlock.ValidCursorColor;
                }
            }
        }
    }
}
