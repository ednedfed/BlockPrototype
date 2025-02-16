using Unity.Mathematics;
using UnityEngine;

public class GhostPosition : MonoBehaviour
{
    public GameObject ghost;

    public HitObject hitObject;

    void FixedUpdate()
    {
        int layerMask = 1 << 6;
        layerMask = ~layerMask;

        Physics.Raycast(transform.position + transform.forward * 0.5f, transform.forward, out var hitInfo, 100f, layerMask);

        hitObject.raycastHit = hitInfo;
        hitObject.isCube = hitInfo.collider != null && hitInfo.collider.gameObject.layer == 3;

        if (hitObject.raycastHit.collider != null)
        {
            var snappedPos = (float3)hitObject.raycastHit.point;
            ghost.transform.position = (float3)snappedPos;

            ghost.transform.up = hitObject.raycastHit.normal;
        }

        var ghostcollider = ghost.GetComponentInChildren<Collider>();
        var overlapCount = Physics.OverlapBoxNonAlloc(ghostcollider.transform.position, new float3(0.49f), resultsNoAlloc, ghostcollider.transform.rotation, layerMask);

        hitObject.isOverlapping = overlapCount > 0;
    }

    Collider[] resultsNoAlloc = new Collider[1];
}
