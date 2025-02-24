using UnityEngine;

class DeleteBlock
{
    HitObject _hitObject;
    BlockFactory _blockFactory;

    public DeleteBlock(HitObject hitObject, BlockFactory blockFactory)
    {
        _hitObject = hitObject;
        _blockFactory = blockFactory;
    }

    // Update is called once per frame
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _hitObject.isCube)
        {
            var block = _hitObject.raycastHit.collider.transform.root.gameObject;

            var blockId = block.GetComponent<BlockIdComponent>();

            _blockFactory.RemoveBlock(blockId);
        }
    }
}
