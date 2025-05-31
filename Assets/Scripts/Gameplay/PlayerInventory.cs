using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //The player inventory script. Used to hold all the items the player will come across

   

    //A list of strings for the inventory
    [SerializeField] private List<ItemData> _inventory;


    private void OnEnable()
    {
        Item.onItemPickedUp += AddItemToInventory;
    }

    private void OnDisable()
    {
        Item.onItemPickedUp -= AddItemToInventory;
    }



    public void GetInventoryItems()
    {
        //get the list of all the inventory items and print them on the screen

        foreach (ItemData item in _inventory)
        {
            Debug.Log(item);
        }
    }

    //Used when the player adds an item to inventory
    public void AddItemToInventory(Item item)
    {
        //adds the given item to the list
        _inventory.Add(item.ItemData);
        
    }

    //Used when the player "throws away" item in inventory
    public void RemoveItemFromInventory(string itemToRemove)
    {
        //iterate through the inventory
        for (int i = 0; i < _inventory.Count; i++)
        {
            //if the item given is in the inventory
            if(itemToRemove == _inventory[i].ItemName)
            {
                //remove the item
                _inventory.RemoveAt(i);
            }
        }

        
    }

    //Used when the player uses an item in the inventory
    public void UseItemInInventory(string itemToUse)
    {
        //first iterate through the inventory list
        for (int i = 0; i < _inventory.Count; i++)
        {
            //if the string given is in the inventory
            if(itemToUse == _inventory[i].ItemName)
            {
                Debug.Log("Use the item in the inventory");

                //Use the item
                _inventory[i].UseItem();

                //remove the item from the inventory
                _inventory.RemoveAt(i);
                
            }

            //else if the string given is not in the inventory
            else if(itemToUse != _inventory[i].ItemName) 
            {
                Debug.Log("Inventory does not have" + itemToUse + "!");
            }
        }

    }

}
