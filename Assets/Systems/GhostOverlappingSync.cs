using UnityEngine;

class GhostOverlappingSync
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

    public void FixedUpdate()
    {
        //todo: extract this functionality
        if (_hitObject.isOverlapping)
        {
            _ghost.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.InvalidGhostColor;
            _cursor.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.InvalidCursorColor;
        }
        else
        {
            _ghost.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.ValidGhostColor;
            _cursor.GetComponentInChildren<Renderer>().material.color = BlockGameConstants.GhostBlock.ValidCursorColor;
        }
    }
}
