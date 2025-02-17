using Unity.Mathematics;
using UnityEngine;

public class GhostRotation : MonoBehaviour
{
    public GameObject cursor;
    public GameObject ghost;

    public int direction;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            direction++;

        if(Input.GetKeyDown(KeyCode.E))
            direction--;

        direction += 4;
        direction %= 4;
    }
}
