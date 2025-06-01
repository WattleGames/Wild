using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    //A script defining an item. Add to all items in the game
    //Requires items to have a trigger

    public delegate void ItemEventHandler(Item item);

    //an event used whenever the player picks an item up
    public static event ItemEventHandler onItemPickedUp;

    [SerializeField] private ItemData _itemData;

    public ItemData ItemData {  get { return _itemData; } }

    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if the player picks up the item
        if(collision.gameObject.GetComponent<PlayerInventory>())
        {
            //invoke the onItemPickedUp event
            onItemPickedUp?.Invoke(this);
            Destroy(this.gameObject);
        }
    }
}
