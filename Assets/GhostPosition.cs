using Unity.Mathematics;
using UnityEngine;

public class GhostPosition : MonoBehaviour
{
    public GameObject cursor;
    public GameObject ghost;
    public GhostRotation ghostRotation;

    public HitObject hitObject;

    void FixedUpdate()
    {
        Physics.Raycast(transform.position + transform.forward * BlockGameConstants.GhostBlock.StartRaycastDistance,
            transform.forward, out var hitInfo,
            BlockGameConstants.GhostBlock.RaycastDistance, BlockGameConstants.GameLayers.InverseGhostLayerMask);

        hitObject.raycastHit = hitInfo;
        hitObject.isCube = hitInfo.collider != null
            && hitInfo.collider.gameObject.layer == BlockGameConstants.GameLayers.BlockLayer;

        if (hitObject.raycastHit.collider != null)
        {
            //todo: determine why these are the offsets required
            var targetGhostPosition = (float3)hitObject.raycastHit.point +
                BlockGameConstants.BlockProperties.CubeCentre +
                (float3)hitObject.raycastHit.normal * BlockGameConstants.BlockProperties.HalfCubeSize;

            //snap on surface, don't snap y
            ghost.transform.position = math.floor(targetGhostPosition) +
                new float3(BlockGameConstants.BlockProperties.HalfCubeSize, 0f, BlockGameConstants.BlockProperties.HalfCubeSize);

            ghost.transform.up = hitObject.raycastHit.normal;

            ghost.transform.rotation *= Quaternion.AngleAxis(
                ghostRotation.direction * BlockGameConstants.GhostBlock.DegreesPerTurn,
                Vector3.up);

            //cursor is parented to ghost
            cursor.transform.position = ghost.transform.position;
            cursor.transform.rotation = ghost.transform.rotation;
        }

        var ghostcollider = ghost.GetComponentInChildren<Collider>();
        if (ghostcollider == null)
        {
            return;
        }

        var overlapCount = Physics.OverlapBoxNonAlloc(
            ghostcollider.transform.position,
            BlockGameConstants.BlockProperties.CubeHalfExtent,
            resultsNoAlloc,
            ghostcollider.transform.rotation,
            BlockGameConstants.GameLayers.InverseGhostLayerMask);

        hitObject.isOverlapping = overlapCount > 0;

        //todo: extract this functionality
        if (hitObject.isOverlapping)
        {
            ghost.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.InvalidGhostColor;
            cursor.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.InvalidCursorColor;
        }
        else
        {
            ghost.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.ValidGhostColor;
            cursor.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.ValidCursorColor;
        }
    }

    Collider[] resultsNoAlloc = new Collider[1];
}
