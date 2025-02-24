using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
partial class DeleteBlockSystem : SystemBase
{
    BlockFactory _blockFactory;

    public DeleteBlockSystem(BlockFactory blockFactory)
    {
        _blockFactory = blockFactory;
    }

    protected override void OnUpdate()
    {
        foreach (var hitObject in SystemAPI.Query<HitObjectComponent>())
        {
            if (Input.GetKeyDown(KeyCode.Mouse1) && hitObject.isCube)
            {
                var block = hitObject.raycastHit.collider.transform.root.gameObject;

                var blockId = block.GetComponent<BlockIdComponent>();

                _blockFactory.RemoveBlock(blockId);
            }
        }
    }
}
