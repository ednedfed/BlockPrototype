using UnityEngine;

public partial class PlaceBlock : MonoBehaviour
{
    public GameObject ghost;
    public uint blockType;
    public HitObject hitObject;
    public BlockFactory blockFactory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && hitObject.isOverlapping == false)
        {
            blockFactory.InstantiateBlock(blockType, ghost.transform.position, ghost.transform.rotation);
        }
    }
}
