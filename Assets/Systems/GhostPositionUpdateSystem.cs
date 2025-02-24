using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[DisableAutoCreation]
partial class GhostPositionUpdateSystem : SystemBase
{
    GameObject _character;

    public GhostPositionUpdateSystem(GameObject character)
    {
        _character = character;

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


            //TODO
            /*
            var ghostcollider = _ghost.GetComponentInChildren<Collider>();
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

            _hitObject.isOverlapping = overlapCount > 0;
            */
        }
    }

    Collider[] _resultsNoAlloc = new Collider[1];
    readonly Transform _ghostRotationTransform;
}
