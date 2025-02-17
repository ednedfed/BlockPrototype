using Unity.Mathematics;
using UnityEngine;

public class GhostPosition : MonoBehaviour
{
    public GameObject cursor;
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
            ghost.transform.up = hitObject.raycastHit.normal;
            cursor.transform.up = hitObject.raycastHit.normal;

            var snappedPos = (float3)hitObject.raycastHit.point + new float3(0f, 0.5f, 0f);

            var snap3 = snappedPos + (float3)hitObject.raycastHit.normal * 0.5f;

            //snap3 -= new float3(0.5f, 0f, 0.5f);

            var snappedPos2 = (float3)math.floor(snap3) + new float3(0.5f, 0f, 0.5f);
            var snappedPos4 = new float3(snappedPos2.x, snap3.y, snappedPos2.z);

            cursor.transform.position = snappedPos2;
            ghost.transform.position = snappedPos2;
        }

        var ghostcollider = ghost.GetComponentInChildren<Collider>();
        var overlapCount = Physics.OverlapBoxNonAlloc(ghostcollider.transform.position, new float3(0.49f), resultsNoAlloc, ghostcollider.transform.rotation, layerMask);

        hitObject.isOverlapping = overlapCount > 0;

        if (hitObject.isOverlapping)
        {
            ghost.GetComponentInChildren<Renderer>().material.color = new Color { r = 1f, a = 0.5f };
            cursor.GetComponentInChildren<Renderer>().material.color = new Color { r = 1f, a = 0.5f };
        }
        else
        {
            ghost.GetComponentInChildren<Renderer>().material.color = new Color { r = 1f, g = 1f, b = 1f, a = 0.5f };
            cursor.GetComponentInChildren<Renderer>().material.color = new Color { g = 1f, a = 0.5f };
        }


    }

    Collider[] resultsNoAlloc = new Collider[1];
}
