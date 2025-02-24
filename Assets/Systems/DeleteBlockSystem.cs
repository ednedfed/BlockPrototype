using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class DeleteBlockSystem : SystemBase
{
    HitObject _hitObject;
    BlockFactory _blockFactory;

    public DeleteBlockSystem(HitObject hitObject, BlockFactory blockFactory)
    {
        _hitObject = hitObject;
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _hitObject.isCube)
        {
            var block = _hitObject.raycastHit.collider.transform.root.gameObject;

            var blockId = block.GetComponent<BlockIdComponent>();

            _blockFactory.RemoveBlock(blockId);
        }
    }
}
