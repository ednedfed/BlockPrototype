using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
partial class GhostPositionUpdateSystem : SystemBase
{
    GameObject _character;
    PlacedBlockContainer _placedBlockContainer;

    public GhostPositionUpdateSystem(GameObject character, PlacedBlockContainer placedBlockContainer)
    {
        _character = character;
        _placedBlockContainer = placedBlockContainer;

        GameObject gameObject = new GameObject("tmpGhostForCalculation");
        _ghostRotationTransform = gameObject.transform;
    }

    protected override void OnUpdate()
    {
        Physics.Raycast(_character.transform.position + _character.transform.forward * BlockGameConstants.GhostBlock.StartRaycastDistance,
            _character.transform.forward, out var hitInfo,
            BlockGameConstants.GhostBlock.RaycastDistance, BlockGameConstants.GameLayers.InverseGhostLayerMask);

        foreach (var (ghostBlockData, hitObject, ghostLocalTransforms) in SystemAPI.Query<GhostBlockDataComponent, RefRW<HitObjectComponent>, RefRW<LocalTransform>>())
        {
            hitObject.ValueRW.raycastHit = hitInfo;
            hitObject.ValueRW.isCube = hitInfo.collider != null
                && hitInfo.collider.gameObject.layer == BlockGameConstants.GameLayers.BlockLayer;

            if (hitObject.ValueRW.isCube)
            {
                hitObject.ValueRW.hitBlockId = _placedBlockContainer.GetBlockId(hitInfo.collider.transform.root.gameObject);
            }

            if (hitObject.ValueRW.raycastHit.collider != null)
            {
                //todo: determine why these are the offsets required
                var targetGhostPosition = (float3)hitObject.ValueRW.raycastHit.point +
                    BlockGameConstants.BlockProperties.CubeCentre +
                    (float3)hitObject.ValueRW.raycastHit.normal * BlockGameConstants.BlockProperties.HalfCubeSize;

                //snap on surface, don't snap y
                ghostLocalTransforms.ValueRW.Position = math.floor(targetGhostPosition) +
                    new float3(BlockGameConstants.BlockProperties.HalfCubeSize, 0f, BlockGameConstants.BlockProperties.HalfCubeSize);

                //this does calculation we need for now to get other axes from assigning up
                _ghostRotationTransform.rotation = ghostLocalTransforms.ValueRW.Rotation;
                _ghostRotationTransform.up = hitObject.ValueRW.raycastHit.normal;
                _ghostRotationTransform.rotation *= Quaternion.AngleAxis(
                    ghostBlockData.direction * BlockGameConstants.GhostBlock.DegreesPerTurn,
                    Vector3.up);

                ghostLocalTransforms.ValueRW.Rotation = _ghostRotationTransform.rotation;
            }
        }

        foreach (var (ghostBlockPrefab, hitObject) in SystemAPI.Query<GhostBlockPrefabComponent, RefRW<HitObjectComponent>>())
        {
            if (ghostBlockPrefab.gameObject != null)
            {
                var ghostcollider = ghostBlockPrefab.gameObject.GetComponentInChildren<Collider>();
                if (ghostcollider == null)
                {
                    return;
                }

                var overlapCount = Physics.OverlapBoxNonAlloc(
                    ghostcollider.transform.position,
                    BlockGameConstants.BlockProperties.CubeHalfExtent,
                    _resultsNoAlloc,
                    ghostcollider.transform.rotation,
                    BlockGameConstants.GameLayers.InverseGhostLayerMask);

                hitObject.ValueRW.isOverlapping = overlapCount > 0;
            }
        }
    }

    Collider[] _resultsNoAlloc = new Collider[1];
    readonly Transform _ghostRotationTransform;
}
