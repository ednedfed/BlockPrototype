using UnityEngine;

public class DeleteBlock : MonoBehaviour
{
    public HitObject hitObject;
    public BlockFactory blockFactory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && hitObject.isCube)
        {
            var block = hitObject.raycastHit.collider.transform.root.gameObject;

            var blockId = block.GetComponent<BlockIdComponent>();

            blockFactory.RemoveBlock(blockId);
        }
    }
}
