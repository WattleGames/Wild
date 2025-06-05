using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    //Thte item in the inventory used for UI

    public ItemData ItemData;

    [Header("UI")]
    public Image itemImage;



    
    public void SetItem(ItemData newItem)
    {
        //sets the item data to be the new item
        ItemData = newItem;

        //sets the item image to be the new item image
        itemImage.sprite = newItem.ItemImage;
    }

    public void RemoveItem(ItemData currentItem)
    {
        //set the item data to null
        ItemData = null;

        //destroy the item
        Destroy(this.gameObject);
    }
}
