using UnityEngine;

public class DeleteBlock : MonoBehaviour
{
    public HitObject hitObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && hitObject.isCube)
        {
            GameObject.Destroy(hitObject.raycastHit.collider.transform.root.gameObject);
        }
    }
}
