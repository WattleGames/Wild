using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        //use the item
        this.GetComponentInChildren<InventoryItem>().ItemData.UseItem();

        //remove the item after being used
        this.GetComponentInChildren<InventoryItem>().RemoveItem(this.GetComponentInChildren<InventoryItem>().ItemData);

    }

    
}
