using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class PlaceBlockSystem : SystemBase
{
    GameObject _ghost;

    HitObject _hitObject;
    GhostBlockData _ghostBlockData;
    BlockFactory _blockFactory;

    public PlaceBlockSystem(GameObject ghost, HitObject hitObject, GhostBlockData ghostBlockData, BlockFactory blockFactory)
    {
        _ghost = ghost;
        _hitObject = hitObject;
        _ghostBlockData = ghostBlockData;
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _hitObject.isOverlapping == false)
        {
            _blockFactory.InstantiateBlock(_ghostBlockData.blockType, _ghost.transform.position, _ghost.transform.rotation);
        }
    }
}
