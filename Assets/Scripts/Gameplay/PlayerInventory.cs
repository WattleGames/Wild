using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //The player inventory script. Used to hold all the items the player will come across

   

    //A list of strings for the inventory
    [SerializeField] private List<string> _inventory;


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

        foreach (string item in _inventory)
        {
            Debug.Log(item);
        }
    }


    public void AddItemToInventory(Item item)
    {
        //adds the given item to the list
        _inventory.Add(item.ItemName);
        
    }

    public void RemoveItemFromInventory(Item item)
    {
        //removes the given item from the list
        _inventory.Remove(item.ItemName);

    }

}
