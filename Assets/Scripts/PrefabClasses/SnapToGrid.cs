using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGrid : MonoBehaviour
{

    void start ()    {
        SnapOption();
    }

    private void SnapOption()
    {
        float y = ConstructionSiteUiManager.GROUNDDIST;
        float gridSize = ConstructionSiteUiManager.snapValue;

        if (this.transform.position.y > ConstructionSiteUiManager.GROUNDDIST)
        {
            y = this.transform.position.y;
        }
        var position = new Vector3(
            Mathf.Round(this.transform.position.x / gridSize) * gridSize, 
            y, 
            Mathf.Round(this.transform.position.z / gridSize) * gridSize
        );

        this.transform.position = position;
    }
}
