using UnityEngine;

public class PlaceBlock : MonoBehaviour
{
    public GameObject ghost;
    public HitObject hitObject;
    public GhostBlockData ghostBlockData;
    public BlockFactory blockFactory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && hitObject.isOverlapping == false)
        {
            blockFactory.InstantiateBlock(ghostBlockData.blockType, ghost.transform.position, ghost.transform.rotation);
        }
    }
}
