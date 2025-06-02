using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIInventory : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool _inventoryOpened;
    [SerializeField] private GameObject _toggleButton;

    public void OnPointerClick(PointerEventData eventData)
    {
        // The UI element that was clicked
        GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;

        //if the clicked object was the toggle button
        if(clickedObject == _toggleButton)
        {
            //toggle the inventory
            ToggleInventoryUI();
        }
    }

    public void ToggleInventoryUI()
    {
        if (!_inventoryOpened)
        {
            //open the inventory
            GetComponent<RectTransform>().DOAnchorPosY(-69f, 1f);
            _inventoryOpened = true;
        }

        //else if the inventory is open
        else if (_inventoryOpened)
        {
            //close the inventory
            GetComponent<RectTransform>().DOAnchorPosY(-128f, 1f);
            _inventoryOpened = false;
        }
    }
}
