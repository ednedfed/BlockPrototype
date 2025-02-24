using UnityEngine;

class PlaceBlock
{
    GameObject _ghost;

    HitObject _hitObject;
    GhostBlockData _ghostBlockData;
    BlockFactory _blockFactory;

    public PlaceBlock(GameObject ghost, HitObject hitObject, GhostBlockData ghostBlockData, BlockFactory blockFactory)
    {
        _ghost = ghost;
        _hitObject = hitObject;
        _ghostBlockData = ghostBlockData;
        _blockFactory = blockFactory;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _hitObject.isOverlapping == false)
        {
            _blockFactory.InstantiateBlock(_ghostBlockData.blockType, _ghost.transform.position, _ghost.transform.rotation);
        }
    }
}
