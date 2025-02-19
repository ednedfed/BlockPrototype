using UnityEngine;

public class DeleteBlock : InjectableBehaviour
{
    HitObject _hitObject;
    BlockFactory _blockFactory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && _hitObject.isCube)
        {
            var block = _hitObject.raycastHit.collider.transform.root.gameObject;

            var blockId = block.GetComponent<BlockIdComponent>();

            _blockFactory.RemoveBlock(blockId);
        }
    }
}
