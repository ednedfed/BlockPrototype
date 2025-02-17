using UnityEngine;

public class GhostBlockType : MonoBehaviour
{
    public GameObject ghost;
    public GhostBlockData ghostBlockData;
    public BlockTypes blockTypes;

    void Update()
    {
        uint desiredBlockType = ghostBlockData.blockType;
        for (int i = 0; i <= 10; ++i)
        {
            if (Input.GetKeyDown((KeyCode)(i + KeyCode.Alpha0)))
            {
                desiredBlockType = (uint)i-1;
                break;
            }
        }

        if (desiredBlockType >= blockTypes.ghostBlockPrefabs.Length)
            return;

        //sync block type
        if (desiredBlockType != ghostBlockData.blockType)
        {
            ghostBlockData.blockType = desiredBlockType;

            //update ghost
            UpdateGhostPrefab(desiredBlockType);
        }
    }

    void UpdateGhostPrefab(uint desiredBlockType)
    {
        if (ghost.transform.childCount > 0)
        {
            var child = ghost.transform.GetChild(0);

            GameObject.Destroy(child.gameObject);
        }

        //parent type to ghost
        GameObject.Instantiate(blockTypes.ghostBlockPrefabs[desiredBlockType], ghost.transform);
    }
}
