using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisableAutoCreation]
partial class GhostPositionUpdateSystem : SystemBase
{
    GameObject _character;
    GameObject _cursor;
    GameObject _ghost;
    GhostBlockData _ghostBlockData;

    HitObject _hitObject;

    public GhostPositionUpdateSystem(GameObject character, GameObject cursor, GameObject ghost, GhostBlockData ghostBlockData, HitObject hitObject)
    {
        _character = character;
        _cursor = cursor;
        _ghost = ghost;
        _ghostBlockData = ghostBlockData;
        _hitObject = hitObject;
    }

    protected override void OnUpdate()
    {
        Physics.Raycast(_character.transform.position + _character.transform.forward * BlockGameConstants.GhostBlock.StartRaycastDistance,
            _character.transform.forward, out var hitInfo,
            BlockGameConstants.GhostBlock.RaycastDistance, BlockGameConstants.GameLayers.InverseGhostLayerMask);

        _hitObject.raycastHit = hitInfo;
        _hitObject.isCube = hitInfo.collider != null
            && hitInfo.collider.gameObject.layer == BlockGameConstants.GameLayers.BlockLayer;

        if (_hitObject.raycastHit.collider != null)
        {
            //todo: determine why these are the offsets required
            var targetGhostPosition = (float3)_hitObject.raycastHit.point +
                BlockGameConstants.BlockProperties.CubeCentre +
                (float3)_hitObject.raycastHit.normal * BlockGameConstants.BlockProperties.HalfCubeSize;

            //snap on surface, don't snap y
            _ghost.transform.position = math.floor(targetGhostPosition) +
                new float3(BlockGameConstants.BlockProperties.HalfCubeSize, 0f, BlockGameConstants.BlockProperties.HalfCubeSize);

            _ghost.transform.up = _hitObject.raycastHit.normal;

            _ghost.transform.rotation *= Quaternion.AngleAxis(
                _ghostBlockData.direction * BlockGameConstants.GhostBlock.DegreesPerTurn,
                Vector3.up);

            //cursor is parented to ghost
            _cursor.transform.position = _ghost.transform.position;
            _cursor.transform.rotation = _ghost.transform.rotation;
        }

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
    }

    Collider[] _resultsNoAlloc = new Collider[1];
}
