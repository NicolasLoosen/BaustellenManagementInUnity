using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    private GameObject objectToClick;
    private ConstructionSiteUiManager constructionSiteUiManager;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            constructionSiteUiManager.CreateRightClickMenu(Input.mousePosition, objectToClick);
        }
    }

    public void setObjectToClick(GameObject objectToClick)
    {
        this.objectToClick = objectToClick;
    }

    public void setConstructionSiteManager(ConstructionSiteUiManager manager)
    {
        constructionSiteUiManager = manager;
    }
}
