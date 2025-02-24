using UnityEngine;

class GhostRotation
{
    GhostBlockData _ghostBlockData;

    public GhostRotation(GhostBlockData ghostBlockData)
    {
        _ghostBlockData = ghostBlockData;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            _ghostBlockData.direction++;

        if(Input.GetKeyDown(KeyCode.E))
            _ghostBlockData.direction--;

        _ghostBlockData.direction += BlockGameConstants.GhostBlock.NumDirections;
        _ghostBlockData.direction %= BlockGameConstants.GhostBlock.NumDirections;
    }
}
