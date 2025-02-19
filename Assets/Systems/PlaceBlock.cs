using UnityEngine;

public class PlaceBlock : InjectableBehaviour
{
    public GameObject ghost;

    HitObject _hitObject;
    GhostBlockData _ghostBlockData;
    BlockFactory _blockFactory;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _hitObject.isOverlapping == false)
        {
            _blockFactory.InstantiateBlock(_ghostBlockData.blockType, ghost.transform.position, ghost.transform.rotation);
        }
    }
}
