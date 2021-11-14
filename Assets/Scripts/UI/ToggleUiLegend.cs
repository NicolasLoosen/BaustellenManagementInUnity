using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleUiLegend : MonoBehaviour
{
    public GameObject LegendPanel;

    public void togglePanel()
    {
       if(LegendPanel != null )
        {
            bool isActive = LegendPanel.activeSelf;
            LegendPanel.SetActive(!isActive);
        }
    }
}
