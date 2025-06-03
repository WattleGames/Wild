using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // The player inventory script. Used to hold all the items the player will come across
    // A new inventory list using arrays
   [SerializeField] private InventorySlot[] _inventorySlots;
   [SerializeField] private GameObject _inventoryItemPrefab;

    private void OnEnable()
    {
        Item.onItemPickedUp +=  AddItemToInventory;
    }

    private void OnDisable()
    {
        Item.onItemPickedUp -= AddItemToInventory;
    }

    //Used when the player adds an item to inventory
    public void AddItemToInventory(ItemData item)
    {
        //first check for an empty slot
        for (int i = 0; i < _inventorySlots.Length; ++i)
        {
            InventorySlot slot = _inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();

            if (itemInSlot == null)
            {
                Debug.Log("add" + item.name + " to inventory");
                SpawnNewItem(item, slot);
                return;
                
            }
        } 
    }

    public void SpawnNewItem(ItemData itemData, InventorySlot slot)
    {
        GameObject newItemGO = Instantiate(_inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItemGO.GetComponent<InventoryItem>();
        inventoryItem.SetItem(itemData);
    }

    //Used when the player "throws away" item in inventory
    public void RemoveItemFromInventory(ItemData itemData, InventorySlot slot)
    {
        //iterate through the inventory
        for (int i = 0; i < _inventorySlots.Length; ++i)
        {
            //if the item given is in the inventory
            if(itemData.name == slot.GetComponentInChildren<InventoryItem>().name)
            {
                //remove the item
                slot.GetComponent<InventoryItem>().RemoveItem(itemData);
               
            }
        }
    }

    public bool CheckIfItemIsInInventory(string requestedItem)
    {
        //iterate throug the inventory
        for (int i = 0; i < _inventorySlots.Length; ++i)
        {
            //if the given string is an item in the inventory
            if (requestedItem == _inventorySlots[i].GetComponentInChildren<InventoryItem>().ItemData.ItemName)
            {
                Debug.Log("The player has this item");
                return true;
            }
            
        }
        return false;
    }
}
