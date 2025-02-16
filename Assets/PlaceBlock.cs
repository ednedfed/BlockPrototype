using UnityEngine;

public class PlaceBlock : MonoBehaviour
{
    public GameObject ghost;
    public GameObject block;

    public HitObject hitObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && hitObject.isOverlapping == false)
        {
            //for now use all one material
            GameObject placedBlock = GameObject.Instantiate(block, ghost.transform.position, ghost.transform.rotation);
        }
    }
}
